namespace AdaptiveNPC
{
    public interface ISaveProvider
    {
        void Save(string key, string data);
        string Load(string key);
        void Delete(string key);
        bool HasKey(string key);
    }
}
