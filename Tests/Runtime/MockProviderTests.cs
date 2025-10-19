using System.Threading.Tasks;
using NUnit.Framework;
using AdaptiveNPC;

namespace AdaptiveNPC.Tests
{
    /// <summary>
    /// Mock provider for testing without API calls
    /// </summary>
    public class MockAIProvider : IResponseProvider
    {
        public int CallCount { get; private set; }
        public float SimulatedDelay { get; set; } = 0.1f;
        public string MockResponse { get; set; } = "Mock response";
        
        public async Task<string> GenerateResponse(ResponseRequest request)
        {
            CallCount++;
            await Task.Delay((int)(SimulatedDelay * 1000));
            return MockResponse;
        }
    }
    
    public class MockProviderTests
    {
        [Test]
        public async Task MockProvider_SimulatesDelay()
        {
            // Arrange
            var mock = new MockAIProvider { SimulatedDelay = 0.2f };
            var request = new ResponseRequest { Action = "test" };
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await mock.GenerateResponse(request);
            stopwatch.Stop();
            
            // Assert
            Assert.AreEqual("Mock response", response);
            Assert.GreaterOrEqual(stopwatch.ElapsedMilliseconds, 190); // Allow small variance
        }
        
        [Test]
        public async Task MockProvider_CountsCalls()
        {
            // Arrange
            var mock = new MockAIProvider { SimulatedDelay = 0 };
            
            // Act
            for (int i = 0; i < 5; i++)
            {
                await mock.GenerateResponse(new ResponseRequest());
            }
            
            // Assert
            Assert.AreEqual(5, mock.CallCount);
        }
    }
}
