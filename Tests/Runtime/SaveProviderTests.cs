using NUnit.Framework;
using UnityEngine;
using AdaptiveNPC;

namespace AdaptiveNPC.Tests
{
    public class SaveProviderTests
    {
        private PlayerPrefsSaveProvider saveProvider;
        private string testKey = "TestKey_AdaptiveNPC";
        
        [SetUp]
        public void Setup()
        {
            saveProvider = new PlayerPrefsSaveProvider();
            PlayerPrefs.DeleteKey(testKey); // Clean slate
        }
        
        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey(testKey); // Cleanup
        }
        
        [Test]
        public void Save_Load_RoundTrip()
        {
            // Arrange
            string testData = "{ \"test\": \"data\", \"number\": 42 }";
            
            // Act
            saveProvider.Save(testKey, testData);
            string loaded = saveProvider.Load(testKey);
            
            // Assert
            Assert.AreEqual(testData, loaded);
        }
        
        [Test]
        public void Load_NonExistentKey_ReturnsEmpty()
        {
            // Act
            string result = saveProvider.Load("NonExistentKey_12345");
            
            // Assert
            Assert.IsEmpty(result);
        }
        
        [Test]
        public void Delete_RemovesKey()
        {
            // Arrange
            saveProvider.Save(testKey, "test");
            
            // Act
            saveProvider.Delete(testKey);
            
            // Assert
            Assert.IsFalse(saveProvider.HasKey(testKey));
        }
        
        [Test]
        public void HasKey_DetectsExistence()
        {
            // Act & Assert
            Assert.IsFalse(saveProvider.HasKey(testKey));
            
            saveProvider.Save(testKey, "test");
            Assert.IsTrue(saveProvider.HasKey(testKey));
            
            saveProvider.Delete(testKey);
            Assert.IsFalse(saveProvider.HasKey(testKey));
        }
    }
}
