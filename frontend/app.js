'use strict';

// ── Config ───────────────────────────────────────────────────────
const API = (window.API_BASE_URL || 'http://localhost:5000').replace(/\/$/, '');

// ── State ────────────────────────────────────────────────────────
let currentSessionId = null;
let isLoading = false;

// ── DOM refs ─────────────────────────────────────────────────────
const messageInput    = document.getElementById('messageInput');
const sendBtn         = document.getElementById('sendBtn');
const messagesList    = document.getElementById('messagesList');
const typingIndicator = document.getElementById('typingIndicator');
const welcomeScreen   = document.getElementById('welcomeScreen');
const sessionsList    = document.getElementById('sessionsList');
const newChatBtn      = document.getElementById('newChatBtn');
const sidebarToggle   = document.getElementById('sidebarToggle');
const sidebar         = document.getElementById('sidebar');
const chatTitle       = document.getElementById('chatTitle');

// ── Init ─────────────────────────────────────────────────────────
(async function init() {
  await loadSessions();
  await startNewSession();

  messageInput.addEventListener('input', onInputChange);
  messageInput.addEventListener('keydown', onKeyDown);
  sendBtn.addEventListener('click', sendMessage);
  newChatBtn.addEventListener('click', startNewSession);
  sidebarToggle.addEventListener('click', toggleSidebar);

  // Suggestion chips
  document.querySelectorAll('.suggestion-chip').forEach(btn => {
    btn.addEventListener('click', () => {
      messageInput.value = btn.dataset.msg;
      onInputChange();
      sendMessage();
    });
  });
})();

// ── Session Management ────────────────────────────────────────────
async function startNewSession() {
  try {
    const res = await fetch(`${API}/api/chat/session`, { method: 'POST' });
    if (!res.ok) throw new Error('Failed to create session');
    const data = await res.json();
    currentSessionId = data.sessionId;

    messagesList.innerHTML = '';
    welcomeScreen.style.display = 'flex';
    chatTitle.textContent = 'CryptoSage';
    await loadSessions();
  } catch (err) {
    showToast('Could not create session. Check backend connection.');
  }
}

async function loadSessions() {
  try {
    const res = await fetch(`${API}/api/chat/sessions`);
    if (!res.ok) return;
    const sessions = await res.json();
    renderSessionsList(sessions);
  } catch { /* ignore */ }
}

function renderSessionsList(sessions) {
  const label = sessionsList.querySelector('.sessions-label');
  sessionsList.innerHTML = '';
  if (label) sessionsList.appendChild(label);
  else {
    const lbl = document.createElement('div');
    lbl.className = 'sessions-label';
    lbl.textContent = 'Recent Chats';
    sessionsList.appendChild(lbl);
  }

  sessions.forEach(s => {
    const item = document.createElement('div');
    item.className = 'session-item' + (s.sessionId === currentSessionId ? ' active' : '');
    item.innerHTML = `
      <span class="session-title" title="${escHtml(s.title)}">${escHtml(s.title)}</span>
      <button class="session-delete" title="Delete" data-id="${s.sessionId}">&#215;</button>
    `;
    item.querySelector('.session-title').addEventListener('click', () => loadSession(s.sessionId, s.title));
    item.querySelector('.session-delete').addEventListener('click', e => {
      e.stopPropagation();
      deleteSession(s.sessionId, item);
    });
    sessionsList.appendChild(item);
  });
}

async function loadSession(sessionId, title) {
  currentSessionId = sessionId;
  chatTitle.textContent = title;

  // Mark active
  document.querySelectorAll('.session-item').forEach(el => el.classList.remove('active'));
  document.querySelectorAll('.session-item').forEach(el => {
    if (el.querySelector(`[data-id="${sessionId}"]`)) el.classList.add('active');
  });

  try {
    const res = await fetch(`${API}/api/chat/history/${sessionId}`);
    const messages = await res.json();
    messagesList.innerHTML = '';
    welcomeScreen.style.display = 'none';
    messages.forEach(m => appendMessage(m.role, m.content, []));
    scrollToBottom();
  } catch {
    showToast('Could not load chat history.');
  }
}

async function deleteSession(sessionId, el) {
  try {
    await fetch(`${API}/api/chat/session/${sessionId}`, { method: 'DELETE' });
    el.remove();
    if (sessionId === currentSessionId) await startNewSession();
  } catch {
    showToast('Could not delete session.');
  }
}

// ── Sample Questions (one per RAG doc) ───────────────────────────
const SAMPLE_QUESTIONS = [
  "How does Bitcoin work and what problem does it solve?",
  "What is Ethereum and how does it differ from Bitcoin?",
  "How does Decentralized Finance (DeFi) work?",
  "What are NFTs and how do they work?",
  "What is a crypto wallet and how do I keep it safe?",
  "How does blockchain technology work?",
  "What makes Solana different from other blockchains?",
  "What is the difference between Proof of Work and Proof of Stake?",
  "What are the basics of crypto trading and technical analysis?",
  "What is market cap and why does it matter in crypto?",
  "How is cryptocurrency regulated and taxed?",
  "What are stablecoins and how do they maintain their peg?",
  "What are Layer 2 solutions and why are they needed?",
  "What is Cardano and what makes it unique?",
  "What is Polkadot and how does its parachain system work?",
  "What is Chainlink and what problem does it solve?",
  "What is Avalanche and how does it achieve fast transactions?",
  "What are meme coins and why are they so volatile?",
  "What is the difference between centralized and decentralized exchanges?",
  "What is Web3 and how does it relate to blockchain?",
  "What is tokenomics and why does it matter?",
  "How can I identify and avoid common crypto scams?",
  "What are Bitcoin ETFs and why are they significant?",
  "How does crypto lending and borrowing work?",
  "What is the Bitcoin Lightning Network?",
  "What are Central Bank Digital Currencies (CBDCs)?",
  "What is GameFi and how does play-to-earn work?",
];

function showSampleQuestions() {
  welcomeScreen.style.display = 'none';
  appendMessage('user', '-q', []);

  const listItems = SAMPLE_QUESTIONS.map((q, i) =>
    `<li class="sample-q-item" data-q="${escHtml(q)}">${i + 1}. ${escHtml(q)}</li>`
  ).join('');

  const el = document.createElement('div');
  el.className = 'message assistant';
  el.innerHTML = `
    <div class="avatar">&#9711;</div>
    <div class="bubble">
      <p><strong>Here are all the questions I can answer from my knowledge base:</strong></p>
      <ul class="sample-q-list">${listItems}</ul>
      <p style="font-size:13px;color:var(--text-secondary);margin-top:8px">Click any question to ask it, or type your own.</p>
    </div>
  `;

  el.querySelectorAll('.sample-q-item').forEach(item => {
    item.addEventListener('click', () => {
      messageInput.value = item.dataset.q;
      onInputChange();
      sendMessage();
    });
  });

  messagesList.appendChild(el);
  scrollToBottom();
}

// ── Messaging ─────────────────────────────────────────────────────
async function sendMessage() {
  const text = messageInput.value.trim();
  if (!text || isLoading) return;

  // Intercept -q command
  if (text === '-q') {
    messageInput.value = '';
    messageInput.style.height = 'auto';
    sendBtn.disabled = true;
    showSampleQuestions();
    onInputChange();
    return;
  }

  isLoading = true;
  welcomeScreen.style.display = 'none';
  messageInput.value = '';
  messageInput.style.height = 'auto';
  sendBtn.disabled = true;

  appendMessage('user', text, []);
  showTyping(true);
  scrollToBottom();

  try {
    const res = await fetch(`${API}/api/chat`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ sessionId: currentSessionId, message: text })
    });

    if (!res.ok) {
      const err = await res.text();
      throw new Error(err || 'Request failed');
    }

    const data = await res.json();
    showTyping(false);
    appendMessage('assistant', data.message, data.toolResults || []);
    scrollToBottom();
    await loadSessions();
    // Update active session title
    chatTitle.textContent = document.querySelector(`.session-item.active .session-title`)?.textContent || 'CryptoSage';
  } catch (err) {
    showTyping(false);
    appendMessage('assistant', 'Sorry, I encountered an error. Please check that the backend is running and try again.', []);
    showToast(err.message);
  } finally {
    isLoading = false;
    onInputChange();
  }
}

// ── Render Messages ───────────────────────────────────────────────
function appendMessage(role, content, toolResults) {
  const el = document.createElement('div');
  el.className = `message ${role}`;

  const avatarHtml = role === 'user'
    ? `<div class="avatar">U</div>`
    : `<div class="avatar">&#9711;</div>`;

  const toolHtml = toolResults.length > 0 ? renderToolResults(toolResults) : '';

  el.innerHTML = `
    ${role === 'assistant' ? avatarHtml : ''}
    <div class="bubble">
      ${formatMessage(content)}
      ${toolHtml}
    </div>
    ${role === 'user' ? avatarHtml : ''}
  `;

  messagesList.appendChild(el);
}

function renderToolResults(toolResults) {
  if (!toolResults.length) return '';
  const cards = toolResults.map(tr => renderToolCard(tr)).join('');
  return `<div class="tool-results">${cards}</div>`;
}

function renderToolCard(toolResult) {
  try {
    const data = JSON.parse(toolResult.result);
    const toolName = toolResult.toolName;

    if (toolName === 'get_crypto_price') {
      return renderPriceCard(data, false);
    }
    if (toolName === 'get_coin_market_data') {
      return renderMarketCard(data);
    }
    if (toolName === 'get_trending_coins') {
      return renderTrendingCard(data);
    }
  } catch { /* not JSON, fall through */ }

  // Fallback: raw text
  return `
    <div class="tool-card">
      <div class="tool-card-header">
        <span class="tool-label">${escHtml(toolResult.toolName)}</span>
      </div>
      <pre style="font-size:12px;color:var(--text-secondary);white-space:pre-wrap">${escHtml(toolResult.result)}</pre>
    </div>
  `;
}

function renderPriceCard(data, detailed) {
  const change = data.price_change_24h_percent ?? 0;
  const changeClass = change >= 0 ? 'change-positive' : 'change-negative';
  const changeSign = change >= 0 ? '+' : '';

  return `
    <div class="tool-card">
      <div class="tool-card-header">
        <span class="tool-label">&#128200; Live Price</span>
      </div>
      <div class="price-grid">
        <div class="price-item">
          <span class="price-label">${escHtml(data.coin_id?.toUpperCase() || 'COIN')}</span>
          <span class="price-value main-price">${formatUSD(data.price_usd)}</span>
        </div>
        <div class="price-item">
          <span class="price-label">24h Change</span>
          <span class="price-value ${changeClass}">${changeSign}${change.toFixed(2)}%</span>
        </div>
        ${data.market_cap_usd ? `
        <div class="price-item">
          <span class="price-label">Market Cap</span>
          <span class="price-value">${formatLargeNum(data.market_cap_usd)}</span>
        </div>` : ''}
      </div>
    </div>
  `;
}

function renderMarketCard(data) {
  const change24h = data.price_change_24h_percent ?? 0;
  const change7d  = data.price_change_7d_percent ?? 0;

  return `
    <div class="tool-card">
      <div class="tool-card-header">
        <span class="tool-label">&#128202; Market Data — ${escHtml(data.name)} (${escHtml(data.symbol)})</span>
      </div>
      <div class="price-grid">
        <div class="price-item">
          <span class="price-label">Price</span>
          <span class="price-value main-price">${formatUSD(data.current_price_usd)}</span>
        </div>
        <div class="price-item">
          <span class="price-label">Rank</span>
          <span class="price-value">#${data.market_cap_rank}</span>
        </div>
        <div class="price-item">
          <span class="price-label">24h Change</span>
          <span class="price-value ${change24h >= 0 ? 'change-positive' : 'change-negative'}">${change24h >= 0 ? '+' : ''}${change24h.toFixed(2)}%</span>
        </div>
        <div class="price-item">
          <span class="price-label">7d Change</span>
          <span class="price-value ${change7d >= 0 ? 'change-positive' : 'change-negative'}">${change7d >= 0 ? '+' : ''}${change7d.toFixed(2)}%</span>
        </div>
        <div class="price-item">
          <span class="price-label">Market Cap</span>
          <span class="price-value">${formatLargeNum(data.market_cap_usd)}</span>
        </div>
        <div class="price-item">
          <span class="price-label">24h Volume</span>
          <span class="price-value">${formatLargeNum(data.total_volume_usd)}</span>
        </div>
        <div class="price-item">
          <span class="price-label">24h High</span>
          <span class="price-value">${formatUSD(data.high_24h_usd)}</span>
        </div>
        <div class="price-item">
          <span class="price-label">24h Low</span>
          <span class="price-value">${formatUSD(data.low_24h_usd)}</span>
        </div>
        <div class="price-item">
          <span class="price-label">All-Time High</span>
          <span class="price-value">${formatUSD(data.all_time_high_usd)}</span>
        </div>
      </div>
    </div>
  `;
}

function renderTrendingCard(data) {
  const coins = data.trending_coins || [];
  const items = coins.map((c, i) => `
    <div class="trending-item">
      <span class="trending-rank">#${i + 1}</span>
      <span class="trending-name">${escHtml(c.name)}</span>
      <span class="trending-symbol">${escHtml(c.symbol?.toUpperCase())}</span>
      ${c.market_cap_rank ? `<span class="trending-mcap-rank">MC#${c.market_cap_rank}</span>` : ''}
    </div>
  `).join('');

  return `
    <div class="tool-card">
      <div class="tool-card-header">
        <span class="tool-label">&#128293; Trending Coins</span>
      </div>
      <div class="trending-list">${items}</div>
    </div>
  `;
}

// ── Formatting Helpers ────────────────────────────────────────────
function formatMessage(text) {
  // Simple markdown-to-HTML (bold, italic, code, headers, lists, paragraphs)
  let html = escHtml(text)
    .replace(/^### (.+)$/gm, '<h3>$1</h3>')
    .replace(/^## (.+)$/gm, '<h2>$1</h2>')
    .replace(/^# (.+)$/gm, '<h2>$1</h2>')
    .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
    .replace(/\*(.+?)\*/g, '<em>$1</em>')
    .replace(/`([^`]+)`/g, '<code>$1</code>')
    .replace(/^- (.+)$/gm, '<li>$1</li>')
    .replace(/(<li>.*<\/li>)/gs, '<ul>$1</ul>')
    .replace(/\n\n/g, '</p><p>')
    .replace(/\n/g, '<br>');
  return `<p>${html}</p>`;
}

function escHtml(str) {
  if (!str) return '';
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}

function formatUSD(num) {
  if (!num && num !== 0) return 'N/A';
  if (num < 0.01) return `$${num.toFixed(6)}`;
  if (num < 1)    return `$${num.toFixed(4)}`;
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 2 }).format(num);
}

function formatLargeNum(num) {
  if (!num) return 'N/A';
  if (num >= 1e12) return `$${(num / 1e12).toFixed(2)}T`;
  if (num >= 1e9)  return `$${(num / 1e9).toFixed(2)}B`;
  if (num >= 1e6)  return `$${(num / 1e6).toFixed(2)}M`;
  return formatUSD(num);
}

// ── UI Helpers ────────────────────────────────────────────────────
function onInputChange() {
  const val = messageInput.value.trim();
  sendBtn.disabled = !val || isLoading;

  // Auto-resize textarea
  messageInput.style.height = 'auto';
  messageInput.style.height = Math.min(messageInput.scrollHeight, 150) + 'px';
}

function onKeyDown(e) {
  if (e.key === 'Enter' && !e.shiftKey) {
    e.preventDefault();
    sendMessage();
  }
}

function showTyping(show) {
  typingIndicator.classList.toggle('hidden', !show);
  if (show) scrollToBottom();
}

function scrollToBottom() {
  const container = document.getElementById('messagesContainer');
  container.scrollTop = container.scrollHeight;
}

function toggleSidebar() {
  sidebar.classList.toggle('collapsed');
}

function showToast(msg) {
  const existing = document.querySelector('.toast');
  if (existing) existing.remove();
  const toast = document.createElement('div');
  toast.className = 'toast';
  toast.textContent = msg;
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 3500);
}
