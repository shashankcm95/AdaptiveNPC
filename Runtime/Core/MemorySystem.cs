using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdaptiveNPC
{
    [Serializable]
    public class MemorySystem : IMemorySystem
    {
        [Serializable]
        private class MemoryEntry
        {
            public string action;
            public string context;
            public long timestamp;
            public float importance;
            
            public MemoryEntry(string action, string context)
            {
                this.action = action;
                this.context = context;
                this.timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                this.importance = CalculateImportance(action);
            }
            
            private static float CalculateImportance(string action)
            {
                if (action.Contains("save") || action.Contains("kill")) return 0.9f;
                if (action.Contains("gift") || action.Contains("insult")) return 0.7f;
                if (action.Contains("help") || action.Contains("attack")) return 0.8f;
                if (action.Contains("talk")) return 0.3f;
                return 0.5f;
            }
        }
        
        private List<MemoryEntry> memories;
        private Dictionary<string, float> playerTraits;
        private readonly int maxMemories;
        
        public MemorySystem(int maxMemories = 50)
        {
            this.maxMemories = maxMemories;
            memories = new List<MemoryEntry>();
            playerTraits = new Dictionary<string, float>();
        }
        
        public void RecordAction(string action, string context)
        {
            memories.Add(new MemoryEntry(action, context));
            
            // Maintain memory limit
            if (memories.Count > maxMemories)
            {
                memories.RemoveAt(0);
            }
            
            UpdatePlayerTraits(action);
        }
        
        private void UpdatePlayerTraits(string action)
        {
            string actionLower = action.ToLower();
            
            if (actionLower.Contains("gift") || actionLower.Contains("help"))
                ModifyTrait("generous", 0.1f);
                
            if (actionLower.Contains("attack") || actionLower.Contains("insult"))
                ModifyTrait("aggressive", 0.15f);
                
            if (actionLower.Contains("talk") || actionLower.Contains("chat"))
                ModifyTrait("social", 0.1f);
                
            if (actionLower.Contains("steal") || actionLower.Contains("sneak"))
                ModifyTrait("sneaky", 0.2f);
        }
        
        private void ModifyTrait(string trait, float delta)
        {
            if (!playerTraits.ContainsKey(trait))
                playerTraits[trait] = 0;
                
            playerTraits[trait] = Mathf.Clamp01(playerTraits[trait] + delta);
        }
        
        public string GetSummary()
        {
            if (memories.Count == 0)
                return "First time meeting";
                
            var topTraits = playerTraits
                .Where(t => t.Value > 0.3f)
                .OrderByDescending(t => t.Value)
                .Take(3)
                .Select(t => $"{t.Key} ({t.Value:P0})");
                
            return $"Interactions: {memories.Count}, Traits: {string.Join(", ", topTraits)}";
        }
        
        public void Clear()
        {
            memories.Clear();
            playerTraits.Clear();
        }
        
        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }
        
        public void Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            JsonUtility.FromJsonOverwrite(data, this);
        }
    }
}
