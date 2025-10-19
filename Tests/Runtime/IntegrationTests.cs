using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AdaptiveNPC;

namespace AdaptiveNPC.Tests
{
    public class IntegrationTests
    {
        private GameObject testObject;
        private CognitiveCompanion companion;
        
        [SetUp]
        public void Setup()
        {
            testObject = new GameObject("TestCompanion");
            companion = testObject.AddComponent<CognitiveCompanion>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (testObject != null)
                Object.DestroyImmediate(testObject);
        }
        
        [UnityTest]
        public IEnumerator Companion_Initializes()
        {
            yield return null; // Wait one frame
            
            Assert.IsTrue(companion.IsInitialized);
            Assert.IsNotNull(companion.CompanionName);
        }
        
        [UnityTest]
        public IEnumerator ObserveAction_TriggersPatternEvent()
        {
            bool patternTriggered = false;
            string recognizedAction = "";
            
            companion.OnPatternRecognized += (action, count) =>
            {
                patternTriggered = true;
                recognizedAction = action;
            };
            
            yield return null; // Initialize
            
            // Trigger pattern (3 times)
            companion.ObserveAction("gave gift");
            companion.ObserveAction("gave another gift");
            companion.ObserveAction("gave third gift");
            
            yield return new WaitForSeconds(0.1f);
            
            Assert.IsTrue(patternTriggered);
            Assert.AreEqual("gift", recognizedAction);
        }
        
        [UnityTest]
        public IEnumerator Memory_PersistsBetweenSessions()
        {
            // First session
            companion.ObserveAction("test action", "test context");
            yield return null;
            
            // Simulate session end
            companion.SendMessage("OnDestroy");
            
            // New session
            var newCompanion = testObject.AddComponent<CognitiveCompanion>();
            yield return null;
            
            // Should have loaded memory
            var summary = newCompanion.GetRelationshipSummary();
            Assert.That(summary, Does.Contain("Interactions"));
        }
    }
}
