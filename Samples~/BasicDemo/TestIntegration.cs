using UnityEngine;
using AdaptiveNPC;

namespace AdaptiveNPC.Samples
{
    /// <summary>
    /// Minimal test to verify the library works
    /// </summary>
    public class TestIntegration : MonoBehaviour
    {
        void Start()
        {
            // Create test NPC
            GameObject npc = new GameObject("Test NPC");
            
            // Add the component - this tests if the library is properly imported
            CognitiveCompanion companion = npc.AddComponent<CognitiveCompanion>();
            
            // Test basic functionality
            companion.ObserveAction("test action", "test context");
            
            // Listen for responses
            companion.OnResponse += (response) => {
                Debug.Log($"✓ Library working! NPC responded: {response}");
            };
            
            // Test pattern recognition
            companion.OnPatternRecognized += (pattern, count) => {
                Debug.Log($"✓ Pattern system working! Detected: {pattern} x{count}");
            };
            
            // Trigger pattern
            for (int i = 0; i < 3; i++)
            {
                companion.ObserveAction("test pattern", "test");
            }
            
            Debug.Log("✓ AdaptiveNPC library successfully integrated!");
        }
    }
}
