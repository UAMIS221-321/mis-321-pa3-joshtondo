[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/oI_Prc_P)

# CryptoSage — AI Cryptocurrency Assistant (MIS 321 PA3)

A full-stack AI chatbot specializing in cryptocurrency using:
- **LLM**: Claude (Anthropic API) for intelligent responses
- **RAG**: MySQL FULLTEXT search over a crypto knowledge base
- **Function Calling**: Live prices via CoinGecko API

## Stack
| Layer    | Tech                          |
|----------|-------------------------------|
| Frontend | HTML, CSS, JavaScript         |
| Backend  | C# ASP.NET Core 8             |
| Database | MySQL (Heroku JawsDB)         |
| LLM      | Anthropic Claude (Haiku)      |
| Live Data| CoinGecko API (free)          |

---

## Local Development Setup

### Prerequisites
- .NET 8 SDK
- MySQL (local instance)
- Anthropic API key → https://console.anthropic.com

### 1. Configure the database
Create a local MySQL database:
```sql
CREATE DATABASE cryptobot;
```

Update `backend/appsettings.json` with your MySQL credentials.

### 2. Set your Anthropic API key
```bash
# Windows (PowerShell)
$env:ANTHROPIC_API_KEY="sk-ant-..."

# Mac/Linux
export ANTHROPIC_API_KEY="sk-ant-..."
```

### 3. Run the backend
```bash
cd backend
dotnet restore
dotnet run
# API available at http://localhost:5000
```

### 4. Open the frontend
Open `frontend/index.html` in your browser (or use VS Code Live Server).

---

## Deployment

### Backend → Heroku

```bash
heroku create your-app-name
heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack
heroku addons:create jawsdb:kitefin
heroku config:set ANTHROPIC_API_KEY=sk-ant-...
git push heroku main
```

### Frontend → Vercel

1. Push repo to GitHub
2. Go to vercel.com → New Project → import repo
3. Vercel auto-detects `vercel.json` and deploys `frontend/`
4. Update `window.API_BASE_URL` in `frontend/index.html` with your Heroku URL
5. Redeploy

---

## Features

### LLM (Claude)
- Natural language understanding for crypto questions
- Contextual multi-turn conversation history stored in MySQL

### RAG (Retrieval Augmented Generation)
- 12 pre-seeded knowledge documents (Bitcoin, Ethereum, DeFi, NFTs, Solana, trading, wallets, stablecoins, regulation...)
- MySQL FULLTEXT search retrieves the top 3 relevant docs per query
- Context injected into Claude's system prompt for grounded answers

### Function Calling (Tool Use)
| Tool | Description |
|------|-------------|
| `get_crypto_price` | Current USD price, 24h change, market cap |
| `get_coin_market_data` | Full detail: 7d change, ATH, volume, high/low |
| `get_trending_coins` | Top 7 trending coins on CoinGecko |

Results render as live data cards in the UI.

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/chat` | Send a message |
| POST | `/api/chat/session` | Create new chat session |
| GET | `/api/chat/sessions` | List recent sessions |
| GET | `/api/chat/history/{id}` | Get session history |
| DELETE | `/api/chat/session/{id}` | Delete a session |
| GET | `/health` | Health check |
