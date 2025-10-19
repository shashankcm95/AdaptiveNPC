using NUnit.Framework;
using AdaptiveNPC;

namespace AdaptiveNPC.Tests
{
    public class PatternRecognizerTests
    {
        private PatternRecognizer patterns;
        
        [SetUp]
        public void Setup()
        {
            patterns = new PatternRecognizer();
        }
        
        [Test]
        public void AnalyzeAction_RecognizesPattern()
        {
            // Act
            var p1 = patterns.AnalyzeAction("gave gift to merchant");
            var p2 = patterns.AnalyzeAction("gave gift to guard");
            var p3 = patterns.AnalyzeAction("gave another gift");
            
            // Assert
            Assert.AreEqual(1, p1.Count);
            Assert.AreEqual(2, p2.Count);
            Assert.AreEqual(3, p3.Count);
            Assert.AreEqual("gift", p3.Action);
        }
        
        [Test]
        public void AnalyzeAction_IncreasesWeight()
        {
            // Arrange
            Pattern firstPattern = null;
            Pattern lastPattern = null;
            
            // Act
            for (int i = 0; i < 5; i++)
            {
                var p = patterns.AnalyzeAction("attack enemy");
                if (i == 0) firstPattern = p;
                if (i == 4) lastPattern = p;
            }
            
            // Assert
            Assert.Greater(lastPattern.Weight, firstPattern.Weight);
            Assert.AreEqual(5, lastPattern.Count);
        }
        
        [Test]
        public void ExtractActionKey_CategorizesProperly()
        {
            // Act & Assert
            Assert.AreEqual("gift", patterns.AnalyzeAction("gave a gift").Action);
            Assert.AreEqual("attack", patterns.AnalyzeAction("attacked the guard").Action);
            Assert.AreEqual("talk", patterns.AnalyzeAction("talked to merchant").Action);
            Assert.AreEqual("help", patterns.AnalyzeAction("helped the villager").Action);
            Assert.AreEqual("misc", patterns.AnalyzeAction("jumped over fence").Action);
        }
        
        [Test]
        public void Serialize_Deserialize_MaintainsPatterns()
        {
            // Arrange
            patterns.AnalyzeAction("gift one");
            patterns.AnalyzeAction("gift two");
            patterns.AnalyzeAction("attack one");
            
            // Act
            string json = patterns.Serialize();
            var newPatterns = new PatternRecognizer();
            newPatterns.Deserialize(json);
            
            // Assert
            var giftPattern = newPatterns.AnalyzeAction("gift three");
            Assert.AreEqual(3, giftPattern.Count); // Should remember the 2 previous
        }
    }
}
