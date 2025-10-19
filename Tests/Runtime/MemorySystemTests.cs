using NUnit.Framework;
using UnityEngine;
using AdaptiveNPC;

namespace AdaptiveNPC.Tests
{
    public class MemorySystemTests
    {
        private MemorySystem memory;
        
        [SetUp]
        public void Setup()
        {
            memory = new MemorySystem(10); // Small limit for testing
        }
        
        [Test]
        public void RecordAction_StoresMemory()
        {
            // Act
            memory.RecordAction("gave gift", "interaction");
            
            // Assert
            var summary = memory.GetSummary();
            Assert.That(summary, Does.Contain("Interactions: 1"));
        }
        
        [Test]
        public void RecordAction_TracksPlayerTraits()
        {
            // Act
            memory.RecordAction("gave gift", "");
            memory.RecordAction("helped merchant", "");
            memory.RecordAction("gave another gift", "");
            
            // Assert
            var summary = memory.GetSummary();
            Assert.That(summary, Does.Contain("generous"));
        }
        
        [Test]
        public void Memory_RespectsMaxLimit()
        {
            // Act
            for (int i = 0; i < 15; i++)
            {
                memory.RecordAction($"action {i}", "test");
            }
            
            // Assert
            var summary = memory.GetSummary();
            Assert.That(summary, Does.Contain("Interactions: 10")); // Should cap at 10
        }
        
        [Test]
        public void Serialize_Deserialize_PreservesData()
        {
            // Arrange
            memory.RecordAction("test action", "context");
            memory.RecordAction("another action", "different");
            
            // Act
            string json = memory.Serialize();
            var newMemory = new MemorySystem(10);
            newMemory.Deserialize(json);
            
            // Assert
            Assert.AreEqual(memory.GetSummary(), newMemory.GetSummary());
        }
        
        [Test]
        public void Clear_RemovesAllData()
        {
            // Arrange
            memory.RecordAction("test", "test");
            
            // Act
            memory.Clear();
            
            // Assert
            var summary = memory.GetSummary();
            Assert.That(summary, Is.EqualTo("First time meeting"));
        }
    }
}
