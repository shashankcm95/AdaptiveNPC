namespace AdaptiveNPC
{
    public interface IMemorySystem
    {
        void RecordAction(string action, string context);
        string GetSummary();
        void Clear();
        string Serialize();
        void Deserialize(string data);
    }
}
