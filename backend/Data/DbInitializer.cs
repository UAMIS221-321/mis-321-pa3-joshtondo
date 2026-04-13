using CryptoBot.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoBot.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        // Add FULLTEXT index if not present (MySQL)
        try
        {
            db.Database.ExecuteSqlRaw(
                "ALTER TABLE KnowledgeDocuments ADD FULLTEXT INDEX ft_knowledge (Title, Content, Keywords)");
        }
        catch { /* index may already exist */ }

        if (db.KnowledgeDocuments.Any()) return;

        var docs = new List<KnowledgeDocument>
        {
            new()
            {
                Title = "Bitcoin: Origins and Technology",
                Category = "Bitcoin",
                Keywords = "bitcoin, BTC, satoshi nakamoto, blockchain, peer-to-peer, cryptocurrency, digital currency, mining, proof of work",
                Content = """
Bitcoin is the world's first decentralized cryptocurrency, created in 2008 by an anonymous person or group using the pseudonym Satoshi Nakamoto. The Bitcoin whitepaper, titled "Bitcoin: A Peer-to-Peer Electronic Cash System," proposed a system for electronic transactions without relying on trust or central authorities.

Bitcoin operates on a blockchain — a distributed ledger maintained by a global network of computers (nodes). Transactions are grouped into blocks and cryptographically linked together, making the history immutable and tamper-resistant. The network uses Proof of Work (PoW) consensus, where miners compete to solve complex mathematical puzzles to add new blocks, earning Bitcoin as a reward.

Bitcoin has a fixed supply cap of 21 million coins, making it deflationary by design. The block reward halves approximately every four years in an event called the "halving," which historically has preceded significant price increases. As of 2024, over 19 million BTC have been mined.
"""
            },
            new()
            {
                Title = "Ethereum and Smart Contracts",
                Category = "Ethereum",
                Keywords = "ethereum, ETH, smart contracts, solidity, vitalik buterin, EVM, gas fees, dApps, decentralized applications",
                Content = """
Ethereum, launched in 2015 by Vitalik Buterin, is a programmable blockchain platform that introduced the concept of smart contracts — self-executing code stored on the blockchain that automatically enforces the terms of an agreement without intermediaries.

Smart contracts are written in Solidity, a programming language specific to Ethereum. They power decentralized applications (dApps) across finance, gaming, art, and more. The Ethereum Virtual Machine (EVM) executes these contracts deterministically across all network nodes.

In 2022, Ethereum transitioned from Proof of Work to Proof of Stake (PoS) in an upgrade called "The Merge," reducing energy consumption by ~99.95%. Ethereum uses "gas" fees to compensate validators and prevent spam. ETH is the native currency used to pay these gas fees.
"""
            },
            new()
            {
                Title = "DeFi: Decentralized Finance",
                Category = "DeFi",
                Keywords = "DeFi, decentralized finance, lending, borrowing, yield farming, liquidity pools, DEX, Uniswap, Aave, Compound, AMM",
                Content = """
Decentralized Finance (DeFi) refers to financial services built on public blockchains that operate without traditional intermediaries like banks or brokerages. DeFi protocols enable lending, borrowing, trading, earning interest, and more — all through smart contracts.

Key DeFi primitives include: Decentralized Exchanges (DEXs) like Uniswap, which use Automated Market Makers (AMMs) and liquidity pools instead of order books; lending protocols like Aave and Compound, where users supply assets to earn interest or borrow against collateral; and yield farming, where users move assets between protocols to maximize returns.

DeFi's TVL (Total Value Locked) peaked at over $180 billion in 2021. While offering unprecedented financial access, DeFi carries risks including smart contract bugs, oracle manipulation, and liquidation risk during volatile markets.
"""
            },
            new()
            {
                Title = "NFTs: Non-Fungible Tokens",
                Category = "NFTs",
                Keywords = "NFT, non-fungible token, digital art, OpenSea, ERC-721, ERC-1155, metaverse, collectibles, Bored Ape Yacht Club",
                Content = """
Non-Fungible Tokens (NFTs) are unique digital assets stored on a blockchain that prove ownership of a specific item — digital art, music, videos, game items, or virtual real estate. Unlike fungible tokens (e.g., one Bitcoin equals any other Bitcoin), each NFT is one-of-a-kind with a unique identifier.

NFTs are typically built on the Ethereum ERC-721 or ERC-1155 standards. Marketplaces like OpenSea, Blur, and Magic Eden allow users to buy, sell, and trade NFTs. The 2021 bull run saw massive NFT sales — Beeple's digital artwork sold for $69 million at Christie's, and collections like CryptoPunks and Bored Ape Yacht Club became cultural phenomena.

The NFT market has since matured, with use cases expanding beyond art into gaming (play-to-earn), ticketing, domain names (ENS), and digital identity. Critics argue about environmental impact (mostly addressed by Ethereum's PoS transition) and market speculation.
"""
            },
            new()
            {
                Title = "Crypto Wallets and Security",
                Category = "Security",
                Keywords = "crypto wallet, private key, public key, seed phrase, hardware wallet, Ledger, MetaMask, cold storage, hot wallet, security",
                Content = """
A cryptocurrency wallet stores your private keys — the cryptographic secrets that prove ownership of your crypto assets. There are two main types: hot wallets (connected to the internet, like MetaMask or Coinbase Wallet) and cold wallets (offline storage, like Ledger or Trezor hardware wallets).

Your seed phrase (12 or 24 random words) is the master backup for your wallet. Anyone with your seed phrase can access all your funds — never share it or store it digitally. The common mantra: "Not your keys, not your coins" warns against leaving funds on exchanges.

Best security practices include: using hardware wallets for large holdings, enabling 2FA on exchange accounts, using unique passwords, being wary of phishing sites (always verify URLs), never clicking suspicious links, and understanding that blockchain transactions are irreversible — always double-check recipient addresses.
"""
            },
            new()
            {
                Title = "Blockchain Technology Fundamentals",
                Category = "Technology",
                Keywords = "blockchain, distributed ledger, consensus mechanism, decentralization, immutability, nodes, hash, cryptography, merkle tree",
                Content = """
A blockchain is a distributed database shared across many computers (nodes) where records are organized into blocks and linked chronologically using cryptographic hashes. Once data is recorded, it is extremely difficult to alter — any change would invalidate all subsequent blocks, which the network would reject.

Each block contains: a list of transactions, a timestamp, a reference (hash) to the previous block, and a nonce (for PoW chains). Hash functions like SHA-256 produce a unique fingerprint for any input — even a tiny change produces a completely different hash. Merkle trees efficiently organize transaction hashes within a block.

Consensus mechanisms are the rules by which nodes agree on the valid state of the blockchain. Proof of Work (Bitcoin) uses computational power; Proof of Stake (Ethereum, Cardano) uses staked collateral. Other mechanisms include Delegated PoS (EOS), Proof of History (Solana), and Proof of Authority (some private chains).
"""
            },
            new()
            {
                Title = "Solana: High-Performance Blockchain",
                Category = "Altcoins",
                Keywords = "Solana, SOL, proof of history, high speed, low fees, TPS, validators, Rust, Anchor, Phantom wallet",
                Content = """
Solana is a high-performance blockchain launched in 2020 by Anatoly Yakovenko. It achieves extremely fast transaction speeds — theoretically 65,000 TPS — through its unique Proof of History (PoH) mechanism combined with Proof of Stake. Transaction fees are typically fractions of a cent.

Solana's ecosystem includes a thriving NFT market (Magic Eden), DeFi protocols (Jupiter, Raydium), and a growing developer community. Programs (smart contracts) on Solana are written primarily in Rust, with the Anchor framework simplifying development. Phantom is the most popular Solana wallet.

Solana has faced criticism for network outages and partial centralization (high validator hardware requirements). Despite this, it has become one of the top blockchains by TVL and daily active users, positioning itself as a competitor to Ethereum for high-throughput applications.
"""
            },
            new()
            {
                Title = "Proof of Work vs Proof of Stake",
                Category = "Technology",
                Keywords = "proof of work, proof of stake, PoW, PoS, mining, validators, staking, energy consumption, security, consensus",
                Content = """
Proof of Work (PoW) and Proof of Stake (PoS) are the two dominant blockchain consensus mechanisms. In PoW (used by Bitcoin), miners spend computational energy competing to solve cryptographic puzzles. The winner adds the next block and earns the block reward. PoW is battle-tested and highly secure but energy-intensive.

Proof of Stake (used by Ethereum, Cardano, Solana) selects validators based on the amount of cryptocurrency they "stake" as collateral. Validators are chosen pseudo-randomly proportional to their stake, then attest to new blocks. Malicious validators can have their stake "slashed." PoS uses 99%+ less energy than PoW.

Trade-offs: PoW offers stronger security guarantees with a longer track record; PoS achieves faster finality and lower energy use but has questions around wealth concentration and slashing risks. Both systems align incentives to discourage attacks since bad actors would destroy the value of their own holdings.
"""
            },
            new()
            {
                Title = "Crypto Trading Basics",
                Category = "Trading",
                Keywords = "trading, spot trading, futures, leverage, long, short, market order, limit order, stop loss, technical analysis, HODL, DCA",
                Content = """
Cryptocurrency trading involves buying and selling digital assets to profit from price movements. Spot trading means buying the actual asset; futures trading allows speculating on price with leverage (amplifying both gains and losses). Major exchanges include Binance, Coinbase, Kraken, and Bybit.

Key order types: Market orders execute immediately at current price; limit orders execute only at your specified price; stop-loss orders automatically sell if price drops below a threshold. DCA (Dollar Cost Averaging) — regularly buying fixed amounts regardless of price — is a popular long-term strategy that reduces timing risk.

Common concepts: HODL (holding long-term), Bull/Bear markets (rising/falling trends), Support/Resistance levels (price zones where buying/selling pressure concentrates), RSI (Relative Strength Index) for overbought/oversold signals, and FOMO/FUD (Fear of Missing Out / Fear Uncertainty Doubt) — emotional drivers that often lead to poor decisions.
"""
            },
            new()
            {
                Title = "Market Cap and Crypto Market Structure",
                Category = "Markets",
                Keywords = "market cap, market capitalization, circulating supply, total supply, volume, liquidity, altseason, dominance, crypto market",
                Content = """
Market capitalization = current price × circulating supply. It measures the relative size of a cryptocurrency. Bitcoin typically dominates at 40-60% of total crypto market cap. Ethereum is usually second. Coins ranked 3-10 are "large-caps"; 11-100 are "mid-caps"; below 100 are "small-caps" with higher risk/reward.

Trading volume indicates how much of an asset changes hands in 24 hours — high volume during a price move confirms strength; low volume suggests weakness. Liquidity refers to how easily an asset can be bought/sold without significantly impacting price. Low-liquidity assets are vulnerable to price manipulation.

The crypto market tends to move in cycles correlated with Bitcoin's halving events (approximately every 4 years). Bitcoin usually leads moves; "altseason" refers to periods when altcoins outperform Bitcoin. Market sentiment is tracked via the Fear & Greed Index (0 = extreme fear, 100 = extreme greed).
"""
            },
            new()
            {
                Title = "Crypto Regulation and Taxes",
                Category = "Regulation",
                Keywords = "crypto regulation, SEC, CFTC, taxes, capital gains, KYC, AML, compliance, legal, IRS, stablecoin regulation",
                Content = """
Cryptocurrency regulation varies significantly by country. In the United States, the SEC and CFTC share oversight — the SEC treats many tokens as securities, while the CFTC classifies Bitcoin and Ethereum as commodities. The regulatory landscape continues to evolve, with the EU's MiCA framework providing clearer rules for European markets.

In the US, the IRS treats cryptocurrency as property. This means: selling, trading, or using crypto to buy goods/services triggers a taxable event. Short-term capital gains (held <1 year) are taxed as ordinary income; long-term gains (held >1 year) receive preferential rates. Mining and staking rewards are typically taxed as ordinary income when received.

KYC (Know Your Customer) and AML (Anti-Money Laundering) requirements apply to centralized exchanges — users must verify identity to use these platforms. DeFi and self-custody wallets currently face fewer regulatory requirements, though governments are working on frameworks. Tax software like Koinly or CoinTracker helps track crypto tax obligations.
"""
            },
            new()
            {
                Title = "Stablecoins: Stability in Crypto",
                Category = "Stablecoins",
                Keywords = "stablecoin, USDT, USDC, DAI, Tether, algorithmic stablecoin, USD-pegged, collateralized, Terra Luna, BUSD",
                Content = """
Stablecoins are cryptocurrencies designed to maintain a stable value relative to a reference asset, usually the US dollar. They serve as a "safe haven" within crypto — traders use them to exit volatile positions without converting to fiat, and DeFi protocols rely on them for lending and liquidity.

Types of stablecoins: Fiat-backed (USDT, USDC) are backed 1:1 by dollars in bank accounts; Crypto-collateralized (DAI) are backed by excess crypto collateral managed by smart contracts; Algorithmic stablecoins attempt to maintain peg through supply adjustments, though these have failed catastrophically (e.g., Terra's UST collapse in 2022, wiping out ~$40 billion).

USDT (Tether) is the largest stablecoin by volume and is crucial for crypto market liquidity. USDC (Circle) is considered more transparent and regulated. Regulators worldwide are focused on stablecoin oversight due to their systemic importance — large stablecoin failures could have broad financial market implications.
"""
            }
        };

        db.KnowledgeDocuments.AddRange(docs);
        db.SaveChanges();
    }
}
