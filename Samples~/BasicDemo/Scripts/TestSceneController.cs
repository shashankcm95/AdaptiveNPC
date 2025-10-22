using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdaptiveNPC;

namespace AdaptiveNPC.Samples
{
    public class TestSceneController : MonoBehaviour
    {
        [Header("Test Settings")]
        public bool autoRunTests = true;
        public float testInterval = 2f;
        
        [Header("References")]
        private CognitiveCompanion[] npcs;
        private TestPlayerController player;
        
        [Header("Test State")]
        private int currentTestIndex = 0;
        private bool isTestRunning = false;
        
        // Test sequence
        private readonly TestAction[] testSequence = new TestAction[]
        {
            new TestAction("talked to", "conversation", 0),
            new TestAction("gave gift", "interaction", 0),
            new TestAction("gave gift", "interaction", 0),
            new TestAction("gave gift", "interaction", 0), // Should trigger pattern
            new TestAction("insulted", "interaction", 1),
            new TestAction("asked about quest", "quest", 2),
            new TestAction("traded with", "shop", 0),
            new TestAction("attacked", "combat", 1),
            new TestAction("attacked", "combat", 1),
            new TestAction("attacked", "combat", 1), // Should trigger pattern
            new TestAction("helped with problem", "quest", 2),
            new TestAction("gave gift", "interaction", 2),
        };
        
        void Start()
        {
            StartCoroutine(InitializeTestScene());
        }
        
        IEnumerator InitializeTestScene()
        {
            yield return new WaitForSeconds(0.5f);
            
            // Find all NPCs
            npcs = FindObjectsOfType<CognitiveCompanion>();
            player = FindObjectOfType<TestPlayerController>();
            
            Debug.Log($"[TEST SCENE] Found {npcs.Length} NPCs");
            
            // Subscribe to events
            foreach (var npc in npcs)
            {
                npc.OnResponse += HandleNPCResponse;
                npc.OnPatternRecognized += HandlePatternRecognized;
            }
            
            // Start automated testing
            if (autoRunTests)
            {
                yield return new WaitForSeconds(1f);
                StartCoroutine(RunAutomatedTests());
            }
        }
        
        IEnumerator RunAutomatedTests()
        {
            isTestRunning = true;
            Debug.Log("=== STARTING AUTOMATED TESTS ===");
            
            while (currentTestIndex < testSequence.Length)
            {
                var test = testSequence[currentTestIndex];
                var targetNPC = npcs[test.npcIndex % npcs.Length];
                
                // Move player to NPC
                if (player != null)
                {
                    player.MoveToNPC(targetNPC.transform);
                }
                
                yield return new WaitForSeconds(1f);
                
                // Execute test action
                Debug.Log($"TEST {currentTestIndex + 1}: Player {test.action} → {targetNPC.CompanionName}");
                targetNPC.ObserveAction(test.action, test.context);
                
                currentTestIndex++;
                yield return new WaitForSeconds(testInterval);
            }
            
            Debug.Log("=== AUTOMATED TESTS COMPLETE ===");
            isTestRunning = false;
            
            // Show summary
            yield return new WaitForSeconds(1f);
            ShowTestSummary();
        }
        
        void HandleNPCResponse(string response)
        {
            Debug.Log($"   └─ Response: {response}");
        }
        
        void HandlePatternRecognized(string action, int count)
        {
            Debug.Log($"   └─ ⚡ PATTERN DETECTED: {action} x{count}");
        }
        
        void ShowTestSummary()
        {
            Debug.Log("=== TEST SUMMARY ===");
            foreach (var npc in npcs)
            {
                string summary = npc.GetRelationshipSummary();
                Debug.Log($"{npc.CompanionName}: {summary}");
            }
        }
        
        // Manual test controls
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTestRunning)
                {
                    StopAllCoroutines();
                    isTestRunning = false;
                    Debug.Log("Tests stopped");
                }
                else
                {
                    StartCoroutine(RunAutomatedTests());
                }
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartTests();
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                ShowTestSummary();
            }
        }
        
        void RestartTests()
        {
            currentTestIndex = 0;
            foreach (var npc in npcs)
            {
                npc.ResetCompanion();
            }
            Debug.Log("Tests reset - press Space to start");
        }
        
        [System.Serializable]
        private class TestAction
        {
            public string action;
            public string context;
            public int npcIndex;
            
            public TestAction(string action, string context, int npcIndex)
            {
                this.action = action;
                this.context = context;
                this.npcIndex = npcIndex;
            }
        }
    }
}
