using Google.Cloud.Firestore;

namespace ToeRunner.Model.Firebase;

/// <summary>
/// Performance metrics calculated from segment statistics
/// </summary>
[FirestoreData]
public class FirebaseStrategyPerformance
{
    [FirestoreProperty("segmentCount")]
    public int SegmentCount { get; set; }
    
    [FirestoreProperty("winRate")]
    public double WinRate { get; set; }
    
    [FirestoreProperty("meanProfit")]
    public double MeanProfit { get; set; }
    
    [FirestoreProperty("medianProfit")]
    public double MedianProfit { get; set; }
    
    [FirestoreProperty("stdDev")]
    public double StdDevProfit { get; set; }
    
    [FirestoreProperty("cv")]
    public double CoefficientOfVariation { get; set; }
    
    [FirestoreProperty("sharpe")]
    public double SharpeRatio { get; set; }
    
    [FirestoreProperty("profitPerTrade")]
    public double ProfitPerTrade { get; set; }
    
    [FirestoreProperty("totalTrades")]
    public int TotalTrades { get; set; }
    
    [FirestoreProperty("profitAtFees")]
    public double ProfitAtRealisticFees { get; set; }
    
    [FirestoreProperty("maxDrawdown")]
    public double MaxDrawdown { get; set; }
    
    [FirestoreProperty("topTwoContrib")]
    public double TopTwoSegmentContribution { get; set; }
    
    [FirestoreProperty("trimmedMean")]
    public double TrimmedMeanProfit { get; set; }
}
