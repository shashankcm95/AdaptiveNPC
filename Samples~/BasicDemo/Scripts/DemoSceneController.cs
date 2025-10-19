using UnityEngine;
using UnityEngine.UI;
using AdaptiveNPC;

namespace AdaptiveNPC.Demo
{
    public class DemoSceneController : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform player;
        [SerializeField] private CognitiveCompanion[] npcs;
        
        [Header("UI References")]
        [SerializeField] private Text statusText;
        [SerializeField] private Text companionResponseText;
        [SerializeField] private Text instructionsText;
        
        private CognitiveCompanion activeCompanion;
        private int actionCount = 0;
        
        void Start()
        {
            SetupScene();
            ShowInstructions();
        }
        
        void SetupScene()
        {
            // Ensure we have a player
            if (player == null)
            {
                GameObject playerObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                playerObj.name = "Player";
                playerObj.tag = "Player";
                player = playerObj.transform;
                player.position = Vector3.zero;
                
                // Add player controller
                player.gameObject.AddComponent<DemoPlayerController>();
            }
            
            // Create NPCs if not assigned
            if (npcs == null || npcs.Length == 0)
            {
                CreateDemoNPCs();
            }
            
            // Setup UI if missing
            if (statusText == null)
            {
                CreateUI();
            }
            
            // Subscribe to companion events
            foreach (var npc in npcs)
            {
                npc.OnResponse += HandleCompanionResponse;
                npc.OnPatternRecognized += HandlePatternRecognized;
            }
        }
        
        void CreateDemoNPCs()
        {
            npcs = new CognitiveCompanion[3];
            
            // Friendly Merchant
            npcs[0] = CreateNPC(
                "Merchant",
                new Vector3(-5, 0, 0),
                Color.green,
                "Friendly merchant who loves to trade"
            );
            
            // Suspicious Guard
            npcs[1] = CreateNPC(
                "Guard",
                new Vector3(5, 0, 0),
                Color.blue,
                "Stern guard who values order and discipline"
            );
            
            // Gossipy Bartender
            npcs[2] = CreateNPC(
                "Bartender",
                new Vector3(0, 0, 5),
                Color.yellow,
                "Chatty bartender who knows everyone's secrets"
            );
        }
        
        CognitiveCompanion CreateNPC(string name, Vector3 position, Color color, string personality)
        {
            GameObject npcObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            npcObj.name = name;
            npcObj.transform.position = position;
            
            // Visual setup
            var renderer = npcObj.GetComponent<Renderer>();
            renderer.material.color = color;
            
            // Add companion component
            var companion = npcObj.AddComponent<CognitiveCompanion>();
            
            // Add floating name label
            var label = new GameObject($"{name}_Label");
            label.transform.SetParent(npcObj.transform);
            label.transform.localPosition = Vector3.up * 2;
            
            var textMesh = label.AddComponent<TextMesh>();
            textMesh.text = name;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.characterSize = 0.1f;
            
            return companion;
        }
        
        void CreateUI()
        {
            // Create Canvas
            GameObject canvasObj = new GameObject("DemoCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Status Panel
            GameObject panel = CreatePanel(canvas.transform, new Vector2(300, 200), new Vector2(10, -10));
            statusText = CreateText(panel.transform, "Status: Ready", new Vector2(0, 50));
            
            // Response Panel
            GameObject responsePanel = CreatePanel(canvas.transform, new Vector2(400, 100), new Vector2(10, -220));
            companionResponseText = CreateText(responsePanel.transform, "Companion responses appear here", new Vector2(0, 0));
            
            // Instructions
            GameObject instructPanel = CreatePanel(canvas.transform, new Vector2(300, 150), new Vector2(-10, -10), TextAnchor.UpperRight);
            instructionsText = CreateText(instructPanel.transform, "", new Vector2(0, 0));
        }
        
        GameObject CreatePanel(Transform parent, Vector2 size, Vector2 position, TextAnchor anchor = TextAnchor.UpperLeft)
        {
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(parent);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchor == TextAnchor.UpperLeft ? new Vector2(0, 1) : new Vector2(1, 1);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = anchor == TextAnchor.UpperLeft ? new Vector2(0, 1) : new Vector2(1, 1);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.8f);
            
            return panel;
        }
        
        Text CreateText(Transform parent, string content, Vector2 position)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(parent);
            
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(10, 10);
            rect.offsetMax = new Vector2(-10, -10);
            
            Text text = textObj.AddComponent<Text>();
            text.text = content;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 14;
            text.alignment = TextAnchor.UpperLeft;
            
            return text;
        }
        
        void ShowInstructions()
        {
            string instructions = @"<b>Demo Controls:</b>
WASD - Move
E - Talk to NPC
G - Give Gift
Q - Insult
R - Recruit as Companion
T - Test Pattern (x3)
Y - Show Memory Debug

<b>Features:</b>
- Pattern Recognition
- Memory System
- Contextual Responses";
            
            if (instructionsText != null)
                instructionsText.text = instructions;
        }
        
        public void SimulatePlayerAction(string action, CognitiveCompanion targetNPC)
        {
            actionCount++;
            
            if (targetNPC != null)
            {
                targetNPC.ObserveAction(action, "interaction");
                UpdateStatus($"Action #{actionCount}: {action} -> {targetNPC.CompanionName}");
            }
        }
        
        void HandleCompanionResponse(string response)
        {
            if (companionResponseText != null)
            {
                companionResponseText.text = $"<color=cyan>{response}</color>";
                Debug.Log($"[Demo] Companion: {response}");
            }
        }
        
        void HandlePatternRecognized(string action, int count)
        {
            UpdateStatus($"Pattern Recognized: {action} (x{count})");
            Debug.Log($"[Demo] Pattern: {action} performed {count} times");
        }
        
        void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = $"Status: {message}";
            }
        }
    }
}
