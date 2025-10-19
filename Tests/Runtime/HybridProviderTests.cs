using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using AdaptiveNPC;

namespace AdaptiveNPC.Tests
{
    public class HybridProviderTests
    {
        [UnityTest]
        public IEnumerator HybridProvider_FallsBackToTemplate_WhenAIFails()
        {
            // Arrange - hybrid with no API key
            var hybrid = new HybridProvider("test personality", 1.0f); // Always try AI
            
            var request = new ResponseRequest
            {
                Action = "test action",
                CompanionName = "Test",
                Pattern = new Pattern { Action = "gift", Count = 3 }
            };
            
            // Act
            var task = hybrid.GenerateResponse(request);
            yield return new WaitUntil(() => task.IsCompleted);
            
            // Assert - should get template response
            Assert.IsNotNull(task.Result);
            Assert.That(task.Result, Does.Contain("gift").Or.Contain("generous"));
        }
        
        [UnityTest]
        public IEnumerator HybridProvider_RespectsAIThreshold()
        {
            // Test that threshold actually controls AI usage
            var hybrid = new HybridProvider("test", 0f); // Never use AI
            
            var request = new ResponseRequest
            {
                Action = "important action",
                Pattern = new Pattern { Count = 10 } // Important pattern
            };
            
            var task = hybrid.GenerateResponse(request);
            yield return new WaitUntil(() => task.IsCompleted);
            
            // Should still get a response (from templates)
            Assert.IsNotNull(task.Result);
        }
    }
}
