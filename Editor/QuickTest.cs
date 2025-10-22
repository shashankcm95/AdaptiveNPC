using UnityEngine;
using UnityEditor;

namespace AdaptiveNPC.Editor
{
    public static class QuickTest
    {
        [MenuItem("AdaptiveNPC/Quick Test")]
        public static void RunQuickTest()
        {
            Debug.Log("=== AdaptiveNPC Quick Test ===");
            
            // Test 1: Create component
            GameObject testObj = new GameObject("TestNPC");
            var companion = testObj.AddComponent<CognitiveCompanion>();
            Debug.Log("✅ Component created");
            
            // Test 2: Observe action
            companion.ObserveAction("test action", "test");
            Debug.Log("✅ ObserveAction called");
            
            // Test 3: Check memory
            string summary = companion.GetRelationshipSummary();
            Debug.Log($"✅ Memory working: {summary}");
            
            // Cleanup
            Object.DestroyImmediate(testObj);
            
            Debug.Log("=== All tests passed! ===");
        }
    }
}
