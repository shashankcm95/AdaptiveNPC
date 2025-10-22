using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AdaptiveNPC;

namespace AdaptiveNPC.Samples
{
    public class TestUIManager : MonoBehaviour
    {
        private Text instructionsText;
        private Text statusText;
        private Text statsText;
        
        void Start()
        {
            CreateUI();
            StartCoroutine(UpdateStats());
        }
        
        void CreateUI()
        {
            // Instructions Panel
            CreatePanel("Instructions", new Vector2(10, -10), new Vector2(300, 200), TextAnchor.UpperLeft, out instructionsText);
            
            instructionsText.text = @"<b>ADAPTIVE NPC TEST SCENE</b>

<b>Automated Test:</b>
Press SPACE to start/stop

<b>Manual Controls:</b>
WASD - Move Player
E - Talk to NPC
G - Give Gift
Q - Insult NPC
F - Attack NPC
R - Reset All NPCs
S - Show Summary

<b>Watch for:</b>
- Speech bubbles
- Pattern detection (3x)
- Different responses";
            
            // Status Panel
            CreatePanel("Status", new Vector2(10, -220), new Vector2(300, 100), TextAnchor.UpperLeft, out statusText);
            
            // Stats Panel
            CreatePanel("Stats", new Vector2(-10, -10), new Vector2(250, 150), TextAnchor.UpperRight, out statsText);
        }
        
        void CreatePanel(string name, Vector2 position, Vector2 size, TextAnchor anchor, out Text textComponent)
        {
            GameObject panel = new GameObject($"{name}Panel");
            panel.transform.SetParent(transform);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            
            if (anchor == TextAnchor.UpperLeft)
            {
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
            }
            else if (anchor == TextAnchor.UpperRight)
            {
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
            }
            
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(panel.transform);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 10);
            textRect.offsetMax = new Vector2(-10, -10);
            
            textComponent = textObj.AddComponent<Text>();
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = 14;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.UpperLeft;
        }
        
        IEnumerator UpdateStats()
        {
            while (true)
            {
                var npcs = FindObjectsOfType<CognitiveCompanion>();
                
                statsText.text = "<b>NPC STATS</b>\n";
                
                foreach (var npc in npcs)
                {
                    string summary = npc.GetRelationshipSummary();
                    statsText.text += $"\n<b>{npc.CompanionName}:</b>\n{summary}\n";
                }
                
                var controller = FindObjectOfType<TestSceneController>();
                if (controller != null)
                {
                    statusText.text = $"<b>Test Status:</b> Running\nTest Progress: Check Console";
                }
                
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
