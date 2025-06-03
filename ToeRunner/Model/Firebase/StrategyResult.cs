using ToeRunner.Model.BigToe;

namespace ToeRunner.Model.Firebase;


public class StrategyResult {
    public String Id { get; set; }
    public string Name { get; set; }

    public ExecutorContainerConfig ExecutorContainerConfig { get; set; }
    public ExecutorEvaluationResult ExecutorEvaluationResult { get; set; }
    
    public int TotalTrades { get; set; }
    public int SegmentCount { get; set; }
    
    public double TotalProfit00 { get; set; }
    public double TotalProfit08 { get; set; }
    public double TotalProfit10 { get; set; }
    public double TotalProfit15 { get; set; }
    public double TotalProfit20 { get; set; }
    public double TotalProfit25 { get; set; }
    public double TotalProfit30 { get; set; }
    public double TotalProfit35 { get; set; }
    public double TotalProfit40 { get; set; }
    public double TotalProfit50 { get; set; }
    public double TotalProfit60 { get; set; }
}
