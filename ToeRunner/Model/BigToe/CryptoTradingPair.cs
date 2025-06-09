using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.BigToe;


/// <summary>
/// Represents a cryptocurrency trading pair on a specific market/exchange
/// </summary>
[FirestoreData]
public class CryptoTradingPair
{
    /// <summary>
    /// If on a market with crypto coins then this is the crypto coin symbol you are trading (e.g., LTC)
    /// If on a dex market then this is the mint address you are trading (SOL e.g., FmMmbH3VGkBRvADe7uqedKtGxqJ2gTK92aHDyqAYur4g)
    /// </summary>
    [FirestoreProperty("a")]
    public string AssetId { get; }

    /// <summary>
    /// What the AssetId is quoted against.
    /// If on a market with crypto coins then this is the crypto coin symbol you paying with (e.g., BTC)
    /// If on a dex market then this is the mint address you are trading (SOL e.g., So11111111111111111111111111111111111111112)
    /// </summary>
    [FirestoreProperty("q")]
    public string QuoteAssetId { get; }
   
    /// <summary>
    /// The exchange or market where this trading pair is listed
    /// </summary>
    [FirestoreProperty("e")]
    public string Exchange { get; }

    /// <summary>
    /// It is the combination of the asset and quote asset with a dash, e.g., "ETH-BTC" 
    /// </summary>
    [FirestoreProperty("tp")]
    public string TradingPair { get; }
    
    /// <summary>
    /// Whether this trading pair is currently active on the exchange
    /// </summary>
    [FirestoreProperty("ia")]
    public bool IsActive { get; }

    /// <summary>
    /// The timestamp when this market data was last updated
    /// </summary>
    [FirestoreProperty("lu", ConverterType = typeof(DateTimeConverter))]
    public DateTime LastUpdated { get; }
    
    /// <summary>
    /// Returns a string that represents the current object
    /// </summary>
    public override string ToString() => $"{TradingPair} on {Exchange}";
}
