using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdaptiveNPC
{
    [Serializable]
    public class PatternRecognizer : IPatternRecognizer
    {
        [Serializable]
        private class PatternData
        {
            public string action;
            public int count;
            public float weight;
            public long lastSeen;
        }
        
        private Dictionary<string, PatternData> patterns;
        private const int MAX_PATTERNS = 20;
        
        public PatternRecognizer()
        {
            patterns = new Dictionary<string, PatternData>();
        }
        
        public Pattern AnalyzeAction(string action)
        {
            string key = ExtractActionKey(action);
            
            if (!patterns.ContainsKey(key))
            {
                patterns[key] = new PatternData
                {
                    action = key,
                    count = 0,
                    weight = 0.3f,
                    lastSeen = DateTimeOffset.Now.ToUnixTimeSeconds()
                };
            }
            
            var pattern = patterns[key];
            pattern.count++;
            pattern.weight = Mathf.Min(1f, pattern.weight * 0.9f + 0.3f);
            pattern.lastSeen = DateTimeOffset.Now.ToUnixTimeSeconds();
            
            // Clean up old patterns
            if (patterns.Count > MAX_PATTERNS)
            {
                var oldestKey = patterns
                    .OrderBy(p => p.Value.lastSeen)
                    .First().Key;
                patterns.Remove(oldestKey);
            }
            
            return new Pattern
            {
                Action = key,
                Count = pattern.count,
                Weight = pattern.weight
            };
        }
        
        private string ExtractActionKey(string action)
        {
            string lower = action.ToLower();
            
            if (lower.Contains("gift")) return "gift";
            if (lower.Contains("insult")) return "insult";
            if (lower.Contains("attack")) return "attack";
            if (lower.Contains("help")) return "help";
            if (lower.Contains("talk") || lower.Contains("chat")) return "talk";
            if (lower.Contains("steal")) return "steal";
            
            return "misc";
        }
        
        public void Clear()
        {
            patterns.Clear();
        }
        
        public string Serialize()
        {
            var list = patterns.Values.ToList();
            return JsonUtility.ToJson(new SerializablePatterns { patterns = list });
        }
        
        public void Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            
            var loaded = JsonUtility.FromJson<SerializablePatterns>(data);
            patterns.Clear();
            
            foreach (var p in loaded.patterns)
            {
                patterns[p.action] = p;
            }
        }
        
        [Serializable]
        private class SerializablePatterns
        {
            public List<PatternData> patterns;
        }
    }
}
