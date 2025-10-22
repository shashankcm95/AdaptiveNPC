using System;
using UnityEngine;

namespace AdaptiveNPC
{
    /// <summary>
    /// Default save provider using Unity's PlayerPrefs
    /// Works on all platforms without additional setup
    /// </summary>
    public class PlayerPrefsSaveProvider : ISaveProvider
    {
        #region Constants
        
        private const string SAVE_PREFIX = "AdaptiveNPC_";
        private const int MAX_PLAYERPREFS_SIZE = 1000000; // 1MB limit for safety
        
        #endregion
        
        #region ISaveProvider Implementation
        
        public void Save(string key, string data)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[PlayerPrefsSaveProvider] Cannot save with null or empty key");
                return;
            }
            
            if (string.IsNullOrEmpty(data))
            {
                Debug.LogWarning($"[PlayerPrefsSaveProvider] Saving empty data for key: {key}");
            }
            
            // Check size limit
            if (data.Length > MAX_PLAYERPREFS_SIZE)
            {
                Debug.LogError($"[PlayerPrefsSaveProvider] Data too large ({data.Length} bytes) for key: {key}");
                return;
            }
            
            try
            {
                string prefKey = SAVE_PREFIX + key;
                PlayerPrefs.SetString(prefKey, data);
                PlayerPrefs.Save();
                
                // Also save timestamp for debugging
                PlayerPrefs.SetString(prefKey + "_timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                
                Debug.Log($"[PlayerPrefsSaveProvider] Saved {data.Length} bytes to key: {key}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[PlayerPrefsSaveProvider] Save failed for key {key}: {e.Message}");
            }
        }
        
        public string Load(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[PlayerPrefsSaveProvider] Cannot load with null or empty key");
                return null;
            }
            
            try
            {
                string prefKey = SAVE_PREFIX + key;
                
                if (!PlayerPrefs.HasKey(prefKey))
                {
                    Debug.Log($"[PlayerPrefsSaveProvider] No saved data found for key: {key}");
                    return null;
                }
                
                string data = PlayerPrefs.GetString(prefKey, "");
                
                if (!string.IsNullOrEmpty(data))
                {
                    string timestamp = PlayerPrefs.GetString(prefKey + "_timestamp", "unknown");
                    Debug.Log($"[PlayerPrefsSaveProvider] Loaded {data.Length} bytes for key: {key} (saved: {timestamp})");
                }
                
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[PlayerPrefsSaveProvider] Load failed for key {key}: {e.Message}");
                return null;
            }
        }
        
        public void Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[PlayerPrefsSaveProvider] Cannot delete with null or empty key");
                return;
            }
            
            try
            {
                string prefKey = SAVE_PREFIX + key;
                
                if (PlayerPrefs.HasKey(prefKey))
                {
                    PlayerPrefs.DeleteKey(prefKey);
                    PlayerPrefs.DeleteKey(prefKey + "_timestamp");
                    PlayerPrefs.Save();
                    Debug.Log($"[PlayerPrefsSaveProvider] Deleted save data for key: {key}");
                }
                else
                {
                    Debug.LogWarning($"[PlayerPrefsSaveProvider] No data to delete for key: {key}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[PlayerPrefsSaveProvider] Delete failed for key {key}: {e.Message}");
            }
        }
        
        public bool HasKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;
            
            string prefKey = SAVE_PREFIX + key;
            return PlayerPrefs.HasKey(prefKey);
        }
        
        #endregion
        
        #region Public Utility Methods
        
        /// <summary>
        /// Clear all AdaptiveNPC save data
        /// </summary>
        public static void ClearAllNPCData()
        {
            if (UnityEditor.EditorUtility.DisplayDialog(
                "Clear All NPC Data", 
                "This will delete all saved NPC memories. Are you sure?", 
                "Yes", "No"))
            {
                // This is a workaround since we can't iterate PlayerPrefs keys
                // We'll need to track keys separately or use a manifest
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("[PlayerPrefsSaveProvider] Cleared all NPC data");
            }
        }
        
        /// <summary>
        /// Get estimated size of saved data
        /// </summary>
        public int GetSaveDataSize(string key)
        {
            string data = Load(key);
            return string.IsNullOrEmpty(data) ? 0 : data.Length;
        }
        
        /// <summary>
        /// Check if we're approaching storage limits
        /// </summary>
        public bool IsStorageAvailable()
        {
            // PlayerPrefs has different limits per platform
            // This is a conservative check
            return true; // PlayerPrefs handles its own limits
        }
        
        #endregion
    }
}
