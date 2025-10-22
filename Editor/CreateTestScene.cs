using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace AdaptiveNPC.Editor
{
    public static class CreateTestScene
    {
        [MenuItem("AdaptiveNPC/Create Test Scene")]
        public static void CreateDemoScene()
        {
            // Save current scene if needed
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }
            
            // Create new scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Create ground
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(10, 1, 10);
            
            // Create test NPC
            GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = "Test NPC";
            npc.transform.position = new Vector3(0, 1, 0);
            
            // Add the CognitiveCompanion component
            var companion = npc.AddComponent<CognitiveCompanion>();
            
            // Create player simulator
            GameObject player = new GameObject("Player Simulator");
            player.AddComponent<TestPlayerSimulator>();
            
            // Position camera
            Camera.main.transform.position = new Vector3(0, 5, -5);
            Camera.main.transform.rotation = Quaternion.Euler(30, 0, 0);
            
            Debug.Log("Test scene created! Press Play to see the NPC in action.");
        }
    }
}
