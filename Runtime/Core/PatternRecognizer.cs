using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdaptiveNPC
{
    /// <summary>
    /// Recognizes patterns in player behavior
    /// </summary>
    [Serializable]
    public class PatternRecognizer : IPatternRecognizer
    {
        #region Nested Classes
        
        [Serializable]
        public class PatternData
        {
            public string actionType;
            public int count;
            public float weight;
            public long lastSeen;
            public float confidence;
            
            public PatternData(string action)
            {
                actionType = action;
                count = 1;
                weight = 0.3f;
                lastSeen = DateTimeOffset.Now.ToUnixTimeSeconds();
                confidence = 0.2f;
            }
            
            public void Increment()
            {
                count++;
                weight = Mathf.Min(1f, weight * 1.1f + 0.1f);
                lastSeen = DateTimeOffset.Now.ToUnixTimeSeconds();
                confidence = Mathf.Min(1f, count * 0.2f);
            }
        }
        
        #endregion
        
        #region Fields
        
        [SerializeField] private Dictionary<string, PatternData> patterns;
        [SerializeField] private int maxPatterns = 20;
        [SerializeField] private float decayRate = 0.95f;
        private readonly string[] actionCategories;
        
        #endregion
        
        #region Constructor
        
        public PatternRecognizer()
        {
            patterns = new Dictionary<string, PatternData>();
            
            // Define action categories we track
            actionCategories = new[]
            {
                "gift", "attack", "talk", "trade", "help",
                "insult", "steal", "quest", "explore", "craft"
            };
        }
        
        #endregion
        
        #region IPatternRecognizer Implementation
        
        public Pattern AnalyzeAction(string action)
        {
            string category = CategorizeAction(action);
            
            // Update or create pattern
            if (!patterns.ContainsKey(category))
            {
                patterns[category] = new PatternData(category);
            }
            else
            {
                patterns[category].Increment();
            }
            
            // Apply decay to other patterns
            ApplyDecay(category);
            
            // Clean old patterns if needed
            if (patterns.Count > maxPatterns)
            {
                RemoveOldestPattern();
            }
            
            // Return current pattern state
            var currentPattern = patterns[category];
            return new Pattern
            {
                Action = category,
                Count = currentPattern.count,
                Weight = currentPattern.weight,
                IsSignificant = currentPattern.count >= 3 || currentPattern.weight > 0.7f
            };
        }
        
        public void Clear()
        {
            patterns.Clear();
        }
        
        public string Serialize()
        {
            var list = patterns.Values.ToList();
            return JsonUtility.ToJson(new SerializablePatternData { patterns = list }, true);
        }
        
        public void Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            
            try
            {
                var loaded = JsonUtility.FromJson<SerializablePatternData>(data);
                patterns.Clear();
                
                if (loaded?.patterns != null)
                {
                    foreach (var pattern in loaded.patterns)
                    {
                        patterns[pattern.actionType] = pattern;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[PatternRecognizer] Failed to deserialize: {e.Message}");
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Get the most significant patterns
        /// </summary>
        public List<Pattern> GetTopPatterns(int count = 3)
        {
            return patterns
                .OrderByDescending(p => p.Value.weight)
                .ThenByDescending(p => p.Value.count)
                .Take(count)
                .Select(p => new Pattern
                {
                    Action = p.Key,
                    Count = p.Value.count,
                    Weight = p.Value.weight,
                    IsSignificant = p.Value.count >= 3
                })
                .ToList();
        }
        
        /// <summary>
        /// Check if a specific pattern is emerging
        /// </summary>
        public bool IsPatternEmerging(string actionType)
        {
            if (patterns.ContainsKey(actionType))
            {
                var pattern = patterns[actionType];
                return pattern.count >= 2 && pattern.weight > 0.5f;
            }
            return false;
        }
        
        #endregion
        
        #region Private Methods
        
        private string CategorizeAction(string action)
        {
            string lower = action.ToLower();
            
            // Check each category
            foreach (var category in actionCategories)
            {
                if (lower.Contains(category))
                    return category;
            }
            
            // Special multi-word checks
            if (lower.Contains("give") || lower.Contains("gave"))
                return "gift";
            if (lower.Contains("fight") || lower.Contains("hit"))
                return "attack";
            if (lower.Contains("speak") || lower.Contains("chat"))
                return "talk";
            
            return "misc";
        }
        
        private void ApplyDecay(string exceptCategory)
        {
            foreach (var key in patterns.Keys.ToList())
            {
                if (key != exceptCategory)
                {
                    patterns[key].weight *= decayRate;
                    
                    // Remove patterns that have decayed too much
                    if (patterns[key].weight < 0.1f && patterns[key].count < 2)
                    {
                        patterns.Remove(key);
                    }
                }
            }
        }
        
        private void RemoveOldestPattern()
        {
            var oldest = patterns
                .OrderBy(p => p.Value.lastSeen)
                .ThenBy(p => p.Value.weight)
                .FirstOrDefault();
            
            if (oldest.Key != null)
            {
                patterns.Remove(oldest.Key);
            }
        }
        
        #endregion
        
        #region Helper Classes
        
        [Serializable]
        private class SerializablePatternData
        {
            public List<PatternData> patterns;
        }
        
        #endregion
    }
}
