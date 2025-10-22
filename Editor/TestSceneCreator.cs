using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using AdaptiveNPC;

namespace AdaptiveNPC.Editor
{
    public static class TestSceneCreator
    {
        private const string SCENE_PATH = "Assets/AdaptiveNPC_TestScene.unity";
        
        [MenuItem("AdaptiveNPC/Create Test Scene", false, 1)]
        public static void CreateTestScene()
        {
            // Save current scene if needed
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                if (!EditorUtility.DisplayDialog("Create Test Scene", 
                    "Save current scene before creating test scene?", 
                    "Save and Continue", "Continue Without Saving"))
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }
            }
            
            // Create new scene
            Scene testScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            Debug.Log("[AdaptiveNPC] Creating test scene...");
            
            try
            {
                CreateEnvironment();
                CreateNPCs();
                CreatePlayer();
                CreateUI();
                CreateTestController();
                ConfigureCamera();
                
                // Save scene
                EditorSceneManager.SaveScene(testScene, SCENE_PATH);
                
                EditorUtility.DisplayDialog("Success!", 
                    $"Test scene created at:\n{SCENE_PATH}\n\nPress Play to start testing!", 
                    "OK");
                    
                Debug.Log($"✅ Test scene created successfully at {SCENE_PATH}");
                Debug.Log("Press Play to see:");
                Debug.Log("- Automated NPC interactions");
                Debug.Log("- Pattern recognition in action");
                Debug.Log("- Memory persistence");
                Debug.Log("- Response generation");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create test scene: {e.Message}");
                EditorUtility.DisplayDialog("Error", 
                    $"Failed to create test scene:\n{e.Message}", 
                    "OK");
            }
        }
        
        [MenuItem("AdaptiveNPC/Open Test Scene", false, 2)]
        public static void OpenTestScene()
        {
            if (System.IO.File.Exists(SCENE_PATH))
            {
                EditorSceneManager.OpenScene(SCENE_PATH);
            }
            else
            {
                if (EditorUtility.DisplayDialog("Test Scene Not Found", 
                    "Test scene doesn't exist. Create it now?", 
                    "Yes", "No"))
                {
                    CreateTestScene();
                }
            }
        }
        
        private static void CreateEnvironment()
        {
            // Ground
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(3, 1, 3);
            
            var groundMat = new Material(Shader.Find("Standard"));
            groundMat.color = new Color(0.2f, 0.3f, 0.2f);
            ground.GetComponent<Renderer>().material = groundMat;
            
            // Add some walls for context
            CreateWall("Wall_North", new Vector3(0, 1, 15), new Vector3(30, 3, 0.5f));
            CreateWall("Wall_South", new Vector3(0, 1, -15), new Vector3(30, 3, 0.5f));
            CreateWall("Wall_East", new Vector3(15, 1, 0), new Vector3(0.5f, 3, 30));
            CreateWall("Wall_West", new Vector3(-15, 1, 0), new Vector3(0.5f, 3, 30));
            
            // Lighting
            GameObject lightObj = GameObject.Find("Directional Light");
            if (lightObj != null)
            {
                lightObj.transform.rotation = Quaternion.Euler(45f, -30f, 0);
                var light = lightObj.GetComponent<Light>();
                light.intensity = 1.2f;
                light.color = new Color(1f, 0.95f, 0.8f);
            }
        }
        
        private static void CreateWall(string name, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = position;
            wall.transform.localScale = scale;
            
            var wallMat = new Material(Shader.Find("Standard"));
            wallMat.color = new Color(0.4f, 0.4f, 0.5f);
            wall.GetComponent<Renderer>().material = wallMat;
        }
        
        private static void CreateNPCs()
        {
            // Create 3 different NPCs with different personalities
            CreateTestNPC(
                "Merchant_NPC",
                new Vector3(-5, 0.5f, 0),
                Color.green,
                "Friendly merchant who loves to trade and gossip",
                "merchant"
            );
            
            CreateTestNPC(
                "Guard_NPC",
                new Vector3(5, 0.5f, 0),
                Color.blue,
                "Stern guard who values law and order",
                "guard"
            );
            
            CreateTestNPC(
                "Scholar_NPC",
                new Vector3(0, 0.5f, 5),
                Color.magenta,
                "Curious scholar interested in player behavior",
                "scholar"
            );
        }
        
        private static GameObject CreateTestNPC(string name, Vector3 position, Color color, 
            string personality, string role)
        {
            // Create NPC body
            GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = name;
            npc.transform.position = position;
            
            // Visual setup
            var mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            npc.GetComponent<Renderer>().material = mat;
            
            // Add the Cognitive Companion component
            var companion = npc.AddComponent<CognitiveCompanion>();
            
            // Add visual feedback component
            var visualizer = npc.AddComponent<NPCVisualFeedback>();
            
            // Add floating name label
            GameObject nameLabel = new GameObject($"{name}_Label");
            nameLabel.transform.SetParent(npc.transform);
            nameLabel.transform.localPosition = Vector3.up * 2.5f;
            
            var textMesh = nameLabel.AddComponent<TextMesh>();
            textMesh.text = $"{role.ToUpper()}\n{name}";
            textMesh.characterSize = 0.1f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 24;
            textMesh.color = Color.white;
            
            // Add interaction zone
            GameObject trigger = new GameObject("InteractionZone");
            trigger.transform.SetParent(npc.transform);
            trigger.transform.localPosition = Vector3.zero;
            
            var collider = trigger.AddComponent<SphereCollider>();
            collider.radius = 2f;
            collider.isTrigger = true;
            
            trigger.AddComponent<NPCInteractionZone>();
            
            return npc;
        }
        
        private static void CreatePlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            player.name = "TestPlayer";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 0.5f, -3);
            
            // Make player stand out
            var mat = new Material(Shader.Find("Standard"));
            mat.color = Color.cyan;
            mat.SetFloat("_Metallic", 0.5f);
            mat.SetFloat("_Glossiness", 0.8f);
            player.GetComponent<Renderer>().material = mat;
            
            // Add player controller
            player.AddComponent<TestPlayerController>();
            
            // Add a light to the player
            GameObject playerLight = new GameObject("PlayerLight");
            playerLight.transform.SetParent(player.transform);
            var light = playerLight.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 5f;
            light.intensity = 0.5f;
            light.color = Color.cyan;
        }
        
        private static void CreateUI()
        {
            // Create Canvas
            GameObject canvasObj = new GameObject("TestUI");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Add UI Manager
            canvasObj.AddComponent<TestUIManager>();
        }
        
        private static void CreateTestController()
        {
            GameObject controller = new GameObject("TestSceneController");
            controller.AddComponent<TestSceneController>();
        }
        
        private static void ConfigureCamera()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = new Vector3(0, 12, -10);
                cam.transform.rotation = Quaternion.Euler(45, 0, 0);
                cam.fieldOfView = 60;
                cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            }
        }
    }
}
