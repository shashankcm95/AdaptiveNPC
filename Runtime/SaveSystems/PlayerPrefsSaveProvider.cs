using UnityEngine;

namespace AdaptiveNPC
{
    public class PlayerPrefsSaveProvider : ISaveProvider
    {
        public void Save(string key, string data)
        {
            PlayerPrefs.SetString(key, data);
            PlayerPrefs.Save();
        }
        
        public string Load(string key)
        {
            return PlayerPrefs.GetString(key, "");
        }
        
        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}
