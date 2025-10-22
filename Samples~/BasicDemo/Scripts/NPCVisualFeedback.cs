using UnityEngine;
using System.Collections;
using AdaptiveNPC;

namespace AdaptiveNPC.Samples
{
    public class NPCVisualFeedback : MonoBehaviour
    {
        private CognitiveCompanion companion;
        private string currentResponse = "";
        private float responseTimer = 0f;
        private bool showingPattern = false;
        
        // Visual elements
        private GameObject speechBubble;
        private TextMesh speechText;
        private Light npcLight;
        
        void Start()
        {
            companion = GetComponent<CognitiveCompanion>();
            
            if (companion != null)
            {
                companion.OnResponse += ShowResponse;
                companion.OnPatternRecognized += ShowPattern;
            }
            
            CreateVisualElements();
        }
        
        void CreateVisualElements()
        {
            // Create speech bubble
            speechBubble = GameObject.CreatePrimitive(PrimitiveType.Quad);
            speechBubble.name = "SpeechBubble";
            speechBubble.transform.SetParent(transform);
            speechBubble.transform.localPosition = new Vector3(0, 3, 0);
            speechBubble.transform.localScale = new Vector3(3, 1, 1);
            
            // Remove collider
            Destroy(speechBubble.GetComponent<Collider>());
            
            // Setup material
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = Color.white;
            speechBubble.GetComponent<Renderer>().material = mat;
            
            // Add text
            GameObject textObj = new GameObject("SpeechText");
            textObj.transform.SetParent(speechBubble.transform);
            textObj.transform.localPosition = new Vector3(0, 0, -0.1f);
            
            speechText = textObj.AddComponent<TextMesh>();
            speechText.text = "";
            speechText.characterSize = 0.05f;
            speechText.anchor = TextAnchor.MiddleCenter;
            speechText.alignment = TextAlignment.Center;
            speechText.fontSize = 18;
            speechText.color = Color.black;
            
            // Create light for feedback
            GameObject lightObj = new GameObject("NPCLight");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = Vector3.up;
            
            npcLight = lightObj.AddComponent<Light>();
            npcLight.type = LightType.Point;
            npcLight.range = 3f;
            npcLight.intensity = 0;
            npcLight.color = Color.white;
            
            // Hide initially
            speechBubble.SetActive(false);
        }
        
        void ShowResponse(string response)
        {
            StartCoroutine(DisplayResponse(response, Color.white));
        }
        
        void ShowPattern(string action, int count)
        {
            string message = $"Pattern: {action} x{count}!";
            StartCoroutine(DisplayResponse(message, Color.yellow));
            showingPattern = true;
        }
        
        IEnumerator DisplayResponse(string text, Color bubbleColor)
        {
            // Show speech bubble
            speechBubble.SetActive(true);
            speechBubble.GetComponent<Renderer>().material.color = bubbleColor;
            
            // Animate text
            speechText.text = "";
            foreach (char c in text)
            {
                speechText.text += c;
                yield return new WaitForSeconds(0.03f);
            }
            
            // Flash light
            if (showingPattern)
            {
                StartCoroutine(FlashLight(Color.yellow, 3));
                showingPattern = false;
            }
            else
            {
                StartCoroutine(FlashLight(Color.white, 1));
            }
            
            // Keep visible for a bit
            yield return new WaitForSeconds(2f);
            
            // Fade out
            float fadeTime = 0.5f;
            float elapsed = 0;
            
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = 1 - (elapsed / fadeTime);
                
                var mat = speechBubble.GetComponent<Renderer>().material;
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
                speechText.color = new Color(0, 0, 0, alpha);
                
                yield return null;
            }
            
            speechBubble.SetActive(false);
        }
        
        IEnumerator FlashLight(Color color, int flashes)
        {
            npcLight.color = color;
            
            for (int i = 0; i < flashes; i++)
            {
                npcLight.intensity = 2f;
                yield return new WaitForSeconds(0.1f);
                npcLight.intensity = 0;
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        void Update()
        {
            // Make speech bubble face camera
            if (speechBubble.activeSelf && Camera.main != null)
            {
                speechBubble.transform.LookAt(Camera.main.transform);
                speechBubble.transform.Rotate(0, 180, 0);
            }
        }
    }
}
