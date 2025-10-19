namespace AdaptiveNPC
{
    public interface IPatternRecognizer
    {
        Pattern AnalyzeAction(string action);
        void Clear();
        string Serialize();
        void Deserialize(string data);
    }
    
    public class Pattern
    {
        public string Action { get; set; }
        public int Count { get; set; }
        public float Weight { get; set; }
    }
}
