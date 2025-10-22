using UnityEngine;
using AdaptiveNPC;

namespace AdaptiveNPC.Samples
{
    /// <summary>
    /// Simulates player interactions for testing
    /// </summary>
    public class TestPlayerSimulator : MonoBehaviour
    {
        private CognitiveCompanion[] npcs;
        private float interactionTimer = 2f;
        private int actionIndex = 0;
        
        private string[] testActions = new string[]
        {
            "talked to",
            "gave gift",
            "gave gift",
            "gave gift",  // Should trigger pattern
            "insulted",
            "attacked",
            "helped with quest"
        };
        
        void Start()
        {
            npcs = FindObjectsOfType<CognitiveCompanion>();
            
            if (npcs.Length == 0)
            {
                Debug.LogError("No NPCs found with CognitiveCompanion component!");
                return;
            }
            
            Debug.Log($"Found {npcs.Length} NPCs. Starting interaction tests...");
            
            // Subscribe to events
            foreach (var npc in npcs)
            {
                npc.OnResponse += (response) => Debug.Log($"[{npc.CompanionName}]: {response}");
                npc.OnPatternRecognized += (action, count) => 
                    Debug.Log($"⚡ Pattern Recognized: {action} x{count}");
            }
        }
        
        void Update()
        {
            interactionTimer -= Time.deltaTime;
            
            if (interactionTimer <= 0 && npcs.Length > 0)
            {
                // Perform next test action
                string action = testActions[actionIndex % testActions.Length];
                var targetNPC = npcs[0];
                
                Debug.Log($"→ Player {action} {targetNPC.CompanionName}");
                targetNPC.ObserveAction(action, "testing");
                
                actionIndex++;
                interactionTimer = 2f; // Reset timer
            }
        }
        
        void OnGUI()
        {
            if (npcs == null || npcs.Length == 0) return;
            
            // Display test info
            GUI.Box(new Rect(10, 10, 250, 100), "AdaptiveNPC Test Runner");
            GUI.Label(new Rect(20, 35, 230, 20), $"NPCs Found: {npcs.Length}");
            GUI.Label(new Rect(20, 55, 230, 20), $"Actions Performed: {actionIndex}");
            GUI.Label(new Rect(20, 75, 230, 20), $"Next Action In: {interactionTimer:F1}s");
        }
    }
}
