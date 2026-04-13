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

        var existingTitles = db.KnowledgeDocuments.Select(d => d.Title).ToHashSet();

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
            },
            new()
            {
                Title = "Layer 2 Solutions: Scaling Ethereum",
                Category = "Technology",
                Keywords = "layer 2, L2, Polygon, Arbitrum, Optimism, rollups, scaling, gas fees, zkEVM, Base, zkSync, Ethereum scaling",
                Content = """
Layer 2 (L2) solutions are networks built on top of Ethereum (Layer 1) that process transactions off-chain and periodically post compressed data back to Ethereum's mainnet. They inherit Ethereum's security while dramatically reducing gas fees and increasing throughput.

The two main L2 types are Optimistic Rollups (Arbitrum, Optimism, Base) and ZK (Zero-Knowledge) Rollups (zkSync, Polygon zkEVM, StarkNet). Optimistic rollups assume transactions are valid by default and use a fraud-proof system for challenges; they have a ~7-day withdrawal delay to mainnet. ZK rollups use cryptographic proofs to verify every transaction batch instantly, enabling faster finality and stronger security guarantees.

Popular L2s include Arbitrum (largest TVL among L2s), Optimism (backed by Coinbase's Base chain), Polygon (originally a sidechain, now building zkEVM), and zkSync. These networks have grown enormously as Ethereum's high gas fees pushed users and developers to seek cheaper alternatives while retaining EVM compatibility.
"""
            },
            new()
            {
                Title = "Cardano: Peer-Reviewed Blockchain",
                Category = "Altcoins",
                Keywords = "Cardano, ADA, Charles Hoskinson, Ouroboros, proof of stake, Haskell, smart contracts, IOHK, Plutus, academic",
                Content = """
Cardano is a third-generation proof-of-stake blockchain founded by Charles Hoskinson, co-founder of Ethereum. It is distinguished by its academic, peer-reviewed approach — all protocol changes and research papers are published and reviewed before implementation. The platform is developed by IOHK, EMURGO, and the Cardano Foundation.

Cardano uses the Ouroboros consensus protocol, the first provably secure PoS consensus mechanism. Its development is broken into eras (Byron, Shelley, Goguen, Basho, Voltaire) each adding new capabilities. Smart contracts were introduced in the Goguen era via the Plutus programming language (based on Haskell).

ADA is Cardano's native token, used for transactions, staking, and governance. The ecosystem includes DeFi protocols, NFT marketplaces, and identity solutions — particularly focused on developing nations for financial inclusion. Cardano is known for its deliberate, methodical development pace.
"""
            },
            new()
            {
                Title = "Polkadot and Cross-Chain Interoperability",
                Category = "Altcoins",
                Keywords = "Polkadot, DOT, parachain, relay chain, Gavin Wood, cross-chain, interoperability, Substrate, Kusama, Web3 Foundation",
                Content = """
Polkadot is a multi-chain network designed to enable different blockchains to interoperate and share security. Founded by Gavin Wood (co-founder of Ethereum), it consists of a central Relay Chain that provides shared security and consensus, and Parachains — specialized blockchains that connect to it and benefit from its security.

Projects bid for parachain slots by locking DOT tokens in a crowdloan auction. Each parachain can be optimized for its specific use case (DeFi, NFTs, identity, etc.) while communicating with others via the Cross-Chain Message Passing (XCMP) protocol. Kusama is Polkadot's "canary network" — a faster, less restricted version used for testing new features.

The Substrate framework allows developers to build custom blockchains that can connect to Polkadot. DOT is used for governance, staking, and bonding for parachain slots. Polkadot addresses a key problem in crypto — blockchain isolation — by allowing assets and data to flow across chains trustlessly.
"""
            },
            new()
            {
                Title = "Chainlink: Decentralized Oracle Network",
                Category = "DeFi",
                Keywords = "Chainlink, LINK, oracle, price feed, smart contract, real-world data, off-chain, VRF, CCIP, decentralized oracle",
                Content = """
Chainlink is the leading decentralized oracle network that connects smart contracts with real-world data, APIs, and off-chain computation. Since smart contracts cannot natively access external data, oracles act as bridges — Chainlink's decentralized approach prevents single points of failure or manipulation.

Chainlink's core products include Price Feeds (used by most major DeFi protocols for asset prices), Verifiable Random Function (VRF) for provably fair randomness in gaming/NFTs, Automation (previously Keepers) for triggering contract functions, and CCIP (Cross-Chain Interoperability Protocol) for cross-chain communication.

LINK is the native token used to pay node operators who retrieve and deliver data. Chainlink secures tens of billions in DeFi TVL across Ethereum, Avalanche, Polygon, and other chains. It is deeply embedded in DeFi infrastructure — protocols like Aave, Compound, and Synthetix depend on Chainlink price feeds for accurate, manipulation-resistant pricing.
"""
            },
            new()
            {
                Title = "Avalanche: High-Speed Smart Contract Platform",
                Category = "Altcoins",
                Keywords = "Avalanche, AVAX, subnets, C-Chain, X-Chain, P-Chain, consensus, Ava Labs, DeFi, fast finality",
                Content = """
Avalanche is a high-performance smart contract platform built by Ava Labs, designed for DeFi and enterprise applications. It achieves near-instant transaction finality (under 2 seconds) through its novel Avalanche consensus protocol — a probabilistic consensus that queries random subsets of validators repeatedly until agreement is reached.

The Avalanche network consists of three built-in blockchains: the X-Chain (for creating and trading assets), C-Chain (EVM-compatible chain for smart contracts), and P-Chain (for validators and subnet coordination). Subnets are custom blockchain networks that can define their own rules, tokens, and virtual machines while benefiting from Avalanche's validator set.

AVAX is the native token used for transaction fees, staking, and subnet creation. The C-Chain's EVM compatibility means Ethereum dApps can easily deploy on Avalanche. The ecosystem includes Trader Joe (DEX), Aave, Benqi (lending), and numerous gaming/NFT projects. Avalanche competes directly with Ethereum as a smart contract platform.
"""
            },
            new()
            {
                Title = "Meme Coins: Dogecoin, Shiba Inu, and the Culture of Crypto",
                Category = "Altcoins",
                Keywords = "meme coin, Dogecoin, DOGE, Shiba Inu, SHIB, Elon Musk, community, speculative, PEPE, meme culture, viral",
                Content = """
Meme coins are cryptocurrencies that originated from internet memes, jokes, or social media trends rather than solving a specific technical problem. Dogecoin (DOGE), launched in 2013 as a joke based on the Shiba Inu dog meme, became the original meme coin and achieved a $90 billion market cap in 2021 — driven largely by Elon Musk's tweets and Reddit communities.

Shiba Inu (SHIB) launched in 2020 as a "Dogecoin killer" and became a massive speculative asset. The SHIB ecosystem expanded to include a DEX (ShibaSwap), NFTs (Shiboshi), and a Layer 2 (Shibarium). PEPE coin launched in 2023 based on the Pepe the Frog meme and quickly reached billions in market cap.

Meme coins are characterized by massive communities, viral marketing, extreme price volatility, and limited fundamental utility. They often lack hard caps on supply and depend on social momentum rather than technology. While some holders have made fortunes, many more have lost money — meme coins are considered among the highest-risk crypto assets. They represent the speculative and community-driven side of crypto culture.
"""
            },
            new()
            {
                Title = "Crypto Exchanges: CEX vs DEX",
                Category = "Trading",
                Keywords = "exchange, CEX, DEX, Binance, Coinbase, Kraken, Uniswap, order book, KYC, custody, centralized, decentralized exchange",
                Content = """
Cryptocurrency exchanges fall into two main categories: Centralized Exchanges (CEX) and Decentralized Exchanges (DEX). CEXs like Binance, Coinbase, and Kraken are operated by companies that hold user funds, require KYC identity verification, and use traditional order books. They offer high liquidity, fiat on/off ramps, and customer support — but users don't control their private keys ("not your keys, not your coins").

DEXs like Uniswap, Curve, and dYdX operate via smart contracts without a central authority. Users trade directly from their wallets, maintaining custody of their funds. Most DEXs use Automated Market Makers (AMMs) with liquidity pools instead of order books. The trade-off: no KYC, no custodial risk, but typically lower liquidity for smaller tokens and higher complexity.

Hybrid approaches include DEX aggregators (1inch, Paraswap) that route orders across multiple DEXs for best price, and CEXs offering on-chain settlement. After the collapse of FTX in 2022 — a major CEX that lost billions in customer funds — there was significant growth in DEX usage as users prioritized self-custody.
"""
            },
            new()
            {
                Title = "Web3: The Decentralized Internet",
                Category = "Technology",
                Keywords = "Web3, decentralization, dApps, self-sovereign identity, token ownership, ENS, IPFS, MetaMask, read-write-own, internet",
                Content = """
Web3 refers to a vision of the internet built on decentralized protocols — primarily blockchains — where users own their data, digital assets, and identity rather than ceding control to centralized platforms. It contrasts with Web1 (read-only static pages) and Web2 (read-write but platform-owned, e.g., Facebook, Google).

Core Web3 concepts include: self-sovereign identity (own your credentials via wallets like MetaMask), token-based ownership (NFTs for digital assets, tokens for governance rights), decentralized storage (IPFS, Arweave), and censorship-resistant applications. ENS (Ethereum Name Service) provides human-readable wallet addresses (.eth domains).

Critics argue Web3 is still largely theoretical, with most "dApps" relying on centralized infrastructure (AWS, Infura, Alchemy) and most users depending on centralized exchanges. Proponents believe it represents a fundamental shift in the ownership model of the internet. Web3 applications span DeFi, NFT marketplaces, decentralized social media (Lens Protocol, Farcaster), and gaming.
"""
            },
            new()
            {
                Title = "Tokenomics: Understanding Crypto Token Economics",
                Category = "Markets",
                Keywords = "tokenomics, token supply, inflation, deflation, vesting, burn, emission, utility token, governance token, token distribution",
                Content = """
Tokenomics (token + economics) describes the economic design of a cryptocurrency — how tokens are created, distributed, and used within an ecosystem. Strong tokenomics align incentives between developers, investors, and users, while poor tokenomics can doom a project regardless of its technology.

Key tokenomics components include: Total Supply (maximum tokens that will ever exist), Circulating Supply (tokens currently available), Emission Schedule (how new tokens are released — fast inflation dilutes value), Vesting (lock-up periods preventing early investors/team from immediately dumping), Token Utility (what the token is actually used for), and Burn Mechanisms (destroying tokens to reduce supply, like Ethereum's EIP-1559 burning base fees).

Governance tokens give holders voting rights on protocol decisions. Utility tokens are used to pay for services. Good tokenomics examples: Bitcoin's fixed 21M supply with halvings, Ethereum's burn mechanism creating deflationary pressure. Bad tokenomics red flags: large team/investor allocations with short vesting, tokens with no real utility, and high inflation rates that erode value.
"""
            },
            new()
            {
                Title = "Crypto Scams and Security Threats",
                Category = "Security",
                Keywords = "scam, rug pull, phishing, social engineering, honeypot, pump and dump, fake projects, security, fraud, hack, exploit",
                Content = """
The crypto space is unfortunately rife with scams and security threats due to irreversible transactions, pseudonymity, and a largely inexperienced user base. Common attack types include: Rug Pulls (developers abandon a project and drain the liquidity pool, often after aggressively marketing it), Phishing (fake websites or emails mimicking legitimate services to steal private keys or seed phrases), and Pump and Dump schemes (coordinated buying to inflate a low-cap coin's price before insiders sell).

Smart contract exploits are a major risk — bugs in contract code can be exploited for millions. Famous hacks include the Ronin Bridge hack ($625M, 2022), Poly Network ($611M, 2021), and numerous DeFi protocol exploits. Honeypots are tokens where the contract allows buying but not selling, trapping investors. Social engineering attacks impersonate support staff or celebrities to steal funds.

Protection strategies: verify URLs carefully (bookmark official sites), never share seed phrases or private keys with anyone, use hardware wallets for large holdings, check contract audits before investing in DeFi, be skeptical of unsolicited DMs, verify project teams, and remember — if it sounds too good to be true (100x guaranteed returns), it almost certainly is.
"""
            },
            new()
            {
                Title = "Bitcoin ETFs and Institutional Adoption",
                Category = "Markets",
                Keywords = "Bitcoin ETF, institutional, BlackRock, Fidelity, spot ETF, MicroStrategy, grayscale, GBTC, corporate treasury, adoption",
                Content = """
In January 2024, the SEC approved spot Bitcoin ETFs in the United States — a landmark moment for crypto adoption. Products from BlackRock (iShares Bitcoin Trust, IBIT), Fidelity, and others launched, attracting billions in inflows within weeks. ETFs allow traditional investors to gain Bitcoin exposure through regulated brokerage accounts without holding crypto directly.

Institutional adoption has accelerated significantly. MicroStrategy (now Strategy), led by Michael Saylor, holds over 200,000 BTC as a corporate treasury reserve strategy. Tesla, Block, and other public companies have added Bitcoin to their balance sheets. Hedge funds, endowments, and pension funds have begun allocating small percentages to Bitcoin as a portfolio diversifier.

The ETF approval is considered a turning point because it provides regulated, insured exposure that institutional compliance requirements demand. Unlike futures-based ETFs (which existed since 2021), spot ETFs directly hold Bitcoin — creating real buying pressure on the market. The narrative around Bitcoin as "digital gold" and an inflation hedge has driven much of the institutional interest.
"""
            },
            new()
            {
                Title = "Crypto Lending, Borrowing, and Yield",
                Category = "DeFi",
                Keywords = "lending, borrowing, yield, interest, Aave, Compound, collateral, liquidation, APY, APR, staking rewards, passive income",
                Content = """
Crypto lending allows holders to earn interest on their assets by supplying them to lending protocols or centralized platforms. In DeFi, protocols like Aave and Compound use smart contracts — suppliers deposit assets into pools and borrowers take over-collateralized loans (typically 150%+ collateral required). Interest rates adjust algorithmically based on supply/demand.

Yield sources in crypto include: Lending interest (supplying assets to DeFi protocols), Staking rewards (validating transactions in PoS networks, typically 4-15% APY), Liquidity provision (earning trading fees by supplying to DEX pools), and Yield farming (moving assets between protocols to maximize returns).

Critical risks: Smart contract bugs can drain entire protocols. Liquidation risk — if collateral value drops below the threshold, positions are automatically liquidated. Centralized lending platforms (Celsius, BlockFi, Voyager) collapsed in 2022, causing billions in losses for users. Impermanent loss affects liquidity providers when asset prices diverge. High APY (100%+) is usually unsustainable and often indicates token inflation or Ponzi-like mechanics.
"""
            },
            new()
            {
                Title = "Bitcoin Lightning Network: Instant Payments",
                Category = "Bitcoin",
                Keywords = "Lightning Network, Bitcoin payments, payment channels, micropayments, fast, cheap, off-chain, LN, routing, node",
                Content = """
The Lightning Network is a Layer 2 payment protocol built on Bitcoin that enables near-instant, near-free transactions. Since Bitcoin's base layer processes only ~7 transactions per second with 10-minute confirmation times, the Lightning Network solves Bitcoin's scalability problem for everyday payments.

Lightning works through payment channels — two parties lock Bitcoin into a 2-of-2 multisig address and can then transact instantly off-chain, updating their balance sheet without broadcasting to the blockchain. Payments can route through multiple channels to reach any recipient. Only the opening and closing of channels are recorded on-chain.

Key stats: Lightning Network capacity has grown to over 5,000 BTC. Apps like Strike, Cash App, and Wallet of Satoshi make Lightning payments accessible to everyday users. El Salvador's Bitcoin legal tender adoption relies heavily on Lightning for retail payments. The network enables micropayments as small as 1 satoshi (~$0.0003), enabling new use cases like streaming payments, content monetization, and machine-to-machine payments.
"""
            },
            new()
            {
                Title = "Central Bank Digital Currencies (CBDCs)",
                Category = "Regulation",
                Keywords = "CBDC, central bank digital currency, digital dollar, digital yuan, e-CNY, government, monetary policy, privacy, programmable money",
                Content = """
Central Bank Digital Currencies (CBDCs) are digital forms of a country's fiat currency issued and controlled by the central bank. Unlike cryptocurrencies, CBDCs are fully centralized, government-controlled, and maintain the same value as the physical currency. Over 130 countries representing 98% of global GDP are exploring or piloting CBDCs.

China leads with the e-CNY (digital yuan), already deployed to millions of users. The European Central Bank is developing a digital euro. The US Federal Reserve has researched a digital dollar but faces political opposition. The Bahamas launched the Sand Dollar — the world's first live retail CBDC.

CBDCs offer potential benefits: financial inclusion for the unbanked, faster payment settlement, reduced cash handling costs, and better monetary policy transmission. However, critics raise serious privacy concerns — governments could monitor every transaction, set expiration dates on currency, restrict spending categories, or implement negative interest rates programmatically. The tension between CBDC efficiency and surveillance capabilities has made them controversial within the crypto community, which values financial privacy and censorship resistance.
"""
            },
            new()
            {
                Title = "GameFi and Play-to-Earn Gaming",
                Category = "NFTs",
                Keywords = "GameFi, play-to-earn, P2E, Axie Infinity, gaming, NFT, in-game assets, metaverse, blockchain gaming, Illuvium, Gods Unchained",
                Content = """
GameFi combines blockchain technology with gaming, allowing players to earn real economic value through gameplay. In play-to-earn (P2E) games, in-game assets (characters, land, items) exist as NFTs that players truly own and can trade. Earned in-game currencies are often real tokens tradeable on exchanges.

Axie Infinity pioneered the P2E model — at its 2021 peak, players in developing countries earned more from the game than local wages. The game's SLP and AXS tokens reached billions in market cap. However, the model proved unsustainable as token inflation and declining player growth collapsed token prices. The Ronin bridge hack ($625M) further damaged the ecosystem.

Newer models try to balance "fun-first" gameplay with economic incentives. Games like Gods Unchained, Illuvium, and Parallel are focusing on genuine gameplay quality. The metaverse concept — persistent 3D virtual worlds with blockchain-based economies — encompasses platforms like Decentraland and The Sandbox where virtual land sells for millions. Despite hype cycles, sustainable blockchain gaming remains an evolving challenge.
"""
            },
        };

        var newDocs = docs.Where(d => !existingTitles.Contains(d.Title)).ToList();
        if (newDocs.Count > 0)
        {
            db.KnowledgeDocuments.AddRange(newDocs);
            db.SaveChanges();
        }
    }
}
