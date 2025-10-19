using UnityEngine;
using UnityEditor;

namespace AdaptiveNPC.Demo
{
    public class DemoSceneSetup : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void SetupDemoScene()
        {
            // Auto-setup when scene loads
            Debug.Log("[AdaptiveNPC] Demo scene initializing...");
        }
        
        #if UNITY_EDITOR
        [MenuItem("AdaptiveNPC/Setup Demo Scene")]
        public static void CreateDemoScene()
        {
            // Clear scene
            foreach (var obj in FindObjectsOfType<GameObject>())
            {
                if (obj.transform.parent == null && obj.name != "Main Camera" && obj.name != "Directional Light")
                {
                    DestroyImmediate(obj);
                }
            }
            
            // Add ground plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(5, 1, 5);
            ground.GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.3f);
            
            // Add demo controller
            GameObject controller = new GameObject("DemoController");
            controller.AddComponent<DemoSceneController>();
            
            // Position camera
            Camera.main.transform.position = new Vector3(0, 10, -10);
            Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
            
            Debug.Log("[AdaptiveNPC] Demo scene created successfully!");
        }
        #endif
    }
}
