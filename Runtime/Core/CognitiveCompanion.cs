using System;
using System.Threading.Tasks;
using UnityEngine;

namespace AdaptiveNPC
{
    /// <summary>
    /// Core component for AI-powered NPCs with memory and adaptive behavior
    /// </summary>
    [AddComponentMenu("Adaptive NPC/Cognitive Companion")]
    [HelpURL("https://github.com/shashankcm95/AdaptiveNPC/wiki")]
    public class CognitiveCompanion : MonoBehaviour
    {
        #region Configuration
        
        [Header("Identity")]
        [SerializeField] private string companionName = "Companion";
        [SerializeField] [TextArea(2,4)] private string personality = "Friendly and observant";
        
        [Header("Response Settings")]
        [SerializeField] private ResponseMode responseMode = ResponseMode.Hybrid;
        [SerializeField] private float aiUsageThreshold = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float responseFrequency = 0.4f;
        
        [Header("Memory Settings")]
        [SerializeField] private bool persistMemory = true;
        [SerializeField] private int maxMemories = 50;
        
        #endregion
        
        #region Components
        
        private IMemorySystem memory;
        private IPatternRecognizer patterns;
        private IResponseProvider responseProvider;
        private ISaveProvider saveProvider;
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Fired when companion generates a response
        /// </summary>
        public event Action<string> OnResponse;
        
        /// <summary>
        /// Fired when a significant pattern is recognized
        /// </summary>
        public event Action<string, int> OnPatternRecognized;
        
        /// <summary>
        /// Fired when memory is saved/loaded
        /// </summary>
        public event Action OnMemorySaved;
        public event Action OnMemoryLoaded;
        
        #endregion
        
        #region Properties
        
        public string CompanionName => companionName;
        public bool IsInitialized { get; private set; }
        
        #endregion
        
        #region Unity Lifecycle
        
        protected virtual void Awake()
        {
            InitializeSystems();
        }
        
        protected virtual void Start()
        {
            if (persistMemory)
            {
                LoadMemory();
            }
            IsInitialized = true;
        }
        
        protected virtual void OnDestroy()
        {
            if (persistMemory && IsInitialized)
            {
                SaveMemory();
            }
        }
        
        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && persistMemory && IsInitialized)
            {
                SaveMemory();
            }
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Main method to observe player actions
        /// </summary>
        public async void ObserveAction(string action, string context = "")
        {
            if (!IsInitialized) return;
            
            // Update memory
            memory.RecordAction(action, context);
            
            // Check patterns
            var pattern = patterns.AnalyzeAction(action);
            if (pattern != null && pattern.Count >= 3)
            {
                OnPatternRecognized?.Invoke(pattern.Action, pattern.Count);
            }
            
            // Decide if we should respond
            if (ShouldRespond(action, pattern))
            {
                await GenerateResponse(action, context, pattern);
            }
        }
        
        /// <summary>
        /// Get relationship summary with player
        /// </summary>
        public string GetRelationshipSummary()
        {
            return memory.GetSummary();
        }
        
        /// <summary>
        /// Clear all memories and patterns
        /// </summary>
        public void ResetCompanion()
        {
            memory?.Clear();
            patterns?.Clear();
            saveProvider?.Delete(GetSaveKey());
            Debug.Log($"{companionName}: Memory cleared, starting fresh!");
        }
        
        /// <summary>
        /// Set custom save provider for integration
        /// </summary>
        public void SetSaveProvider(ISaveProvider provider)
        {
            saveProvider = provider;
        }
        
        #endregion
        
        #region Private Methods
        
        private void InitializeSystems()
        {
            // Initialize memory
            memory = new MemorySystem(maxMemories);
            
            // Initialize pattern recognition
            patterns = new PatternRecognizer();
            
            // Initialize save provider
            saveProvider = new PlayerPrefsSaveProvider();
            
            // Initialize response provider based on mode
            responseProvider = responseMode switch
            {
                ResponseMode.AI => new OpenAIProvider(personality),
                ResponseMode.Template => new TemplateProvider(),
                ResponseMode.Hybrid => new HybridProvider(personality, aiUsageThreshold),
                _ => new TemplateProvider()
            };
        }
        
        private bool ShouldRespond(string action, Pattern pattern)
        {
            // Don't respond to every single action
            if (UnityEngine.Random.value > responseFrequency)
                return false;
            
            // Always respond to important patterns
            if (pattern != null && (pattern.Count == 3 || pattern.Count == 10))
                return true;
            
            // Response chance based on action importance
            return GetActionImportance(action) > UnityEngine.Random.value;
        }
        
        private float GetActionImportance(string action)
        {
            if (action.Contains("gift") || action.Contains("help")) return 0.8f;
            if (action.Contains("insult") || action.Contains("attack")) return 0.9f;
            if (action.Contains("talk")) return 0.3f;
            return 0.2f;
        }
        
        private async Task GenerateResponse(string action, string context, Pattern pattern)
        {
            try
            {
                var request = new ResponseRequest
                {
                    Action = action,
                    Context = context,
                    Pattern = pattern,
                    Memory = memory.GetSummary(),
                    Personality = personality,
                    CompanionName = companionName
                };
                
                string response = await responseProvider.GenerateResponse(request);
                
                if (!string.IsNullOrEmpty(response))
                {
                    OnResponse?.Invoke(response);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[CognitiveCompanion] Response generation failed: {e.Message}");
            }
        }
        
        private void SaveMemory()
        {
            try
            {
                var data = new SaveData
                {
                    memories = memory.Serialize(),
                    patterns = patterns.Serialize(),
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                
                string json = JsonUtility.ToJson(data);
                saveProvider.Save(GetSaveKey(), json);
                OnMemorySaved?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[CognitiveCompanion] Save failed: {e.Message}");
            }
        }
        
        private void LoadMemory()
        {
            try
            {
                string json = saveProvider.Load(GetSaveKey());
                if (!string.IsNullOrEmpty(json))
                {
                    var data = JsonUtility.FromJson<SaveData>(json);
                    memory.Deserialize(data.memories);
                    patterns.Deserialize(data.patterns);
                    OnMemoryLoaded?.Invoke();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[CognitiveCompanion] Load failed: {e.Message}");
            }
        }
        
        private string GetSaveKey()
        {
            return $"AdaptiveNPC_{companionName}_{Application.productName}";
        }
        
        #endregion
        
        #region Helper Classes
        
        public enum ResponseMode
        {
            Template,   // No AI, just templates
            AI,         // Always use AI
            Hybrid      // Smart mix of both
        }
        
        [Serializable]
        private class SaveData
        {
            public string memories;
            public string patterns;
            public string timestamp;
        }
        
        #endregion
    }
}
