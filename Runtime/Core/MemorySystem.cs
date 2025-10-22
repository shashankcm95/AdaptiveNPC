using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdaptiveNPC
{
    /// <summary>
    /// Manages NPC memory of player interactions and derives player traits
    /// </summary>
    [Serializable]
    public class MemorySystem : IMemorySystem
    {
        #region Nested Classes
        
        [Serializable]
        public class MemoryEntry
        {
            public string action;
            public string context;
            public long timestamp;
            public float importance;
            
            public MemoryEntry(string action, string context, float importance)
            {
                this.action = action;
                this.context = context;
                this.timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                this.importance = importance;
            }
        }
        
        [Serializable]
        public class PlayerTraits
        {
            public Dictionary<string, float> traits = new Dictionary<string, float>();
            
            public string GetDominantTrait()
            {
                if (traits.Count == 0) return "neutral";
                return traits.OrderByDescending(t => t.Value).First().Key;
            }
        }
        
        #endregion
        
        #region Fields
        
        [SerializeField] private List<MemoryEntry> memories;
        [SerializeField] private PlayerTraits playerTraits;
        [SerializeField] private int maxMemories;
        private readonly Dictionary<string, string[]> actionTraitMap;
        
        #endregion
        
        #region Constructor
        
        public MemorySystem(int maxMemories = 50)
        {
            this.maxMemories = maxMemories;
            this.memories = new List<MemoryEntry>();
            this.playerTraits = new PlayerTraits();
            
            // Map actions to traits
            actionTraitMap = new Dictionary<string, string[]>
            {
                { "gift", new[] { "generous", "friendly" } },
                { "help", new[] { "helpful", "kind" } },
                { "attack", new[] { "aggressive", "hostile" } },
                { "insult", new[] { "rude", "mean" } },
                { "steal", new[] { "dishonest", "sneaky" } },
                { "talk", new[] { "social", "talkative" } },
                { "trade", new[] { "merchant", "trader" } },
                { "quest", new[] { "adventurous", "helpful" } }
            };
        }
        
        #endregion
        
        #region IMemorySystem Implementation
        
        public void RecordAction(string action, string context)
        {
            // Calculate importance
            float importance = CalculateImportance(action, context);
            
            // Add memory
            memories.Add(new MemoryEntry(action, context, importance));
            
            // Maintain memory limit - keep important memories
            if (memories.Count > maxMemories)
            {
                memories = memories
                    .OrderByDescending(m => m.importance)
                    .ThenByDescending(m => m.timestamp)
                    .Take(maxMemories)
                    .ToList();
            }
            
            // Update traits
            UpdatePlayerTraits(action);
        }
        
        public string GetSummary()
        {
            if (memories.Count == 0)
                return "First encounter";
            
            // Build summary
            var recentActions = memories
                .OrderByDescending(m => m.timestamp)
                .Take(5)
                .Select(m => m.action);
            
            var dominantTrait = playerTraits.GetDominantTrait();
            var traitValue = playerTraits.traits.ContainsKey(dominantTrait) 
                ? playerTraits.traits[dominantTrait] 
                : 0;
            
            return $"Encounters: {memories.Count}, " +
                   $"Trait: {dominantTrait} ({traitValue:P0}), " +
                   $"Recent: {string.Join(", ", recentActions.Take(3))}";
        }
        
        public void Clear()
        {
            memories.Clear();
            playerTraits.traits.Clear();
        }
        
        public string Serialize()
        {
            var data = new SerializableMemoryData
            {
                memories = memories,
                traits = playerTraits.traits.ToList()
            };
            return JsonUtility.ToJson(data, true);
        }
        
        public void Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            
            try
            {
                var loadedData = JsonUtility.FromJson<SerializableMemoryData>(data);
                memories = loadedData.memories ?? new List<MemoryEntry>();
                
                playerTraits.traits.Clear();
                if (loadedData.traits != null)
                {
                    foreach (var trait in loadedData.traits)
                    {
                        playerTraits.traits[trait.Key] = trait.Value;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[MemorySystem] Failed to deserialize: {e.Message}");
            }
        }
        
        #endregion
        
        #region Private Methods
        
        private float CalculateImportance(string action, string context)
        {
            float baseImportance = 0.5f;
            
            // Critical actions
            if (action.Contains("save") || action.Contains("rescue"))
                return 1.0f;
            if (action.Contains("kill") || action.Contains("murder"))
                return 0.95f;
            
            // Important interactions
            if (action.Contains("gift") || action.Contains("help"))
                baseImportance = 0.7f;
            if (action.Contains("insult") || action.Contains("attack"))
                baseImportance = 0.8f;
            
            // Context modifiers
            if (context.Contains("quest") || context.Contains("important"))
                baseImportance += 0.2f;
            
            return Mathf.Clamp01(baseImportance);
        }
        
        private void UpdatePlayerTraits(string action)
        {
            string actionKey = ExtractActionKey(action);
            
            if (actionTraitMap.ContainsKey(actionKey))
            {
                foreach (var trait in actionTraitMap[actionKey])
                {
                    if (!playerTraits.traits.ContainsKey(trait))
                        playerTraits.traits[trait] = 0;
                    
                    // Increase trait with diminishing returns
                    float current = playerTraits.traits[trait];
                    float increase = 0.1f * (1f - current * 0.5f);
                    playerTraits.traits[trait] = Mathf.Clamp01(current + increase);
                }
            }
            
            // Decay opposite traits
            DecayOppositeTraits(actionKey);
        }
        
        private void DecayOppositeTraits(string actionKey)
        {
            var opposites = new Dictionary<string, string[]>
            {
                { "generous", new[] { "greedy", "selfish" } },
                { "aggressive", new[] { "peaceful", "calm" } },
                { "helpful", new[] { "selfish", "indifferent" } }
            };
            
            foreach (var trait in playerTraits.traits.Keys.ToList())
            {
                if (opposites.ContainsKey(trait))
                {
                    foreach (var opposite in opposites[trait])
                    {
                        if (playerTraits.traits.ContainsKey(opposite))
                        {
                            playerTraits.traits[opposite] *= 0.9f;
                        }
                    }
                }
            }
        }
        
        private string ExtractActionKey(string action)
        {
            string lower = action.ToLower();
            
            foreach (var key in actionTraitMap.Keys)
            {
                if (lower.Contains(key))
                    return key;
            }
            
            return "misc";
        }
        
        #endregion
        
        #region Helper Classes
        
        [Serializable]
        private class SerializableMemoryData
        {
            public List<MemoryEntry> memories;
            public List<KeyValuePair<string, float>> traits;
        }
        
        #endregion
    }
}
