using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AdaptiveNPC;

namespace AdaptiveNPC.Tests
{
    public class OpenAIProviderTests
    {
        private OpenAIProvider provider;
        
        [SetUp]
        public void Setup()
        {
            provider = new OpenAIProvider("test personality");
        }
        
        [Test]
        public void IsConfigured_NoAPIKey_ReturnsFalse()
        {
            // Clear any existing keys
            PlayerPrefs.DeleteKey("AdaptiveNPC_OpenAI_Key");
            System.Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
            
            // Create fresh provider
            var testProvider = new OpenAIProvider("test");
            
            // Assert
            Assert.IsFalse(testProvider.IsConfigured());
        }
        
        [Test]
        public void IsConfigured_WithAPIKey_ReturnsTrue()
        {
            // Set test key
            PlayerPrefs.SetString("AdaptiveNPC_OpenAI_Key", "test-key-123");
            
            // Create provider that should find the key
            var testProvider = new OpenAIProvider("test");
            
            // Assert
            Assert.IsTrue(testProvider.IsConfigured());
            
            // Cleanup
            PlayerPrefs.DeleteKey("AdaptiveNPC_OpenAI_Key");
        }
        
        [UnityTest]
        public IEnumerator GenerateResponse_NoAPIKey_ReturnsNull()
        {
            // Ensure no API key
            PlayerPrefs.DeleteKey("AdaptiveNPC_OpenAI_Key");
            var testProvider = new OpenAIProvider("test");
            
            var request = new ResponseRequest
            {
                Action = "test action",
                Context = "test",
                CompanionName = "TestBot"
            };
            
            var task = testProvider.GenerateResponse(request);
            
            yield return new WaitUntil(() => task.IsCompleted);
            
            Assert.IsNull(task.Result);
        }
        
        [UnityTest]
        [Category("RequiresAPIKey")]
        public IEnumerator GenerateResponse_WithValidKey_ReturnsResponse()
        {
            // Skip if no real API key available
            if (!provider.IsConfigured())
            {
                Assert.Ignore("Skipping - No API key configured for integration test");
                yield break;
            }
            
            var request = new ResponseRequest
            {
                Action = "gave gift",
                Context = "interaction",
                CompanionName = "TestCompanion",
                Personality = "friendly"
            };
            
            var stopwatch = Stopwatch.StartNew();
            var task = provider.GenerateResponse(request);
            
            yield return new WaitUntil(() => task.IsCompleted);
            stopwatch.Stop();
            
            // Assert response
            Assert.IsNotNull(task.Result);
            Assert.IsNotEmpty(task.Result);
            
            // Log timing
            UnityEngine.Debug.Log($"OpenAI response time: {stopwatch.ElapsedMilliseconds}ms");
            
            // Check reasonable response time (under 10 seconds)
            Assert.Less(stopwatch.ElapsedMilliseconds, 10000, "Response took too long");
        }
        
        [UnityTest]
        [Category("RequiresAPIKey")]
        public IEnumerator ResponseTime_MultipleCalls_MeasureLatency()
        {
            if (!provider.IsConfigured())
            {
                Assert.Ignore("Skipping - No API key configured");
                yield break;
            }
            
            var timings = new System.Collections.Generic.List<long>();
            
            // Make 3 calls to measure consistency
            for (int i = 0; i < 3; i++)
            {
                var request = new ResponseRequest
                {
                    Action = $"test action {i}",
                    Context = "test",
                    CompanionName = "TestBot",
                    Personality = "brief"
                };
                
                var stopwatch = Stopwatch.StartNew();
                var task = provider.GenerateResponse(request);
                
                yield return new WaitUntil(() => task.IsCompleted);
                stopwatch.Stop();
                
                timings.Add(stopwatch.ElapsedMilliseconds);
                
                yield return new WaitForSeconds(0.5f); // Rate limiting
            }
            
            // Calculate statistics
            long average = timings.Sum() / timings.Count;
            long min = timings.Min();
            long max = timings.Max();
            
            UnityEngine.Debug.Log($"OpenAI Response Times - Avg: {average}ms, Min: {min}ms, Max: {max}ms");
            
            // Assert reasonable times
            Assert.Less(average, 5000, "Average response time too high");
            Assert.Less(max - min, 3000, "Response time variance too high");
        }
    }
}
