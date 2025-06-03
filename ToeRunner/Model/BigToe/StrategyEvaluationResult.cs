namespace ToeRunner.Model.BigToe;

public class StrategyEvaluationResult {
    public List<PlaybackSegmentDetails> SegmentDetails { get; set; }
    public TradeContainerConfig TradeContainerConfig { get; set; }
    public List<ExecutorEvaluationResult> ExecutorEvaluationResults { get; set; }
}
