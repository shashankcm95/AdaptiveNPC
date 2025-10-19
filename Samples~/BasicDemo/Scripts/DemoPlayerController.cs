using UnityEngine;
using AdaptiveNPC;

namespace AdaptiveNPC.Demo
{
    public class DemoPlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float interactionRange = 3f;
        
        private CognitiveCompanion nearestNPC;
        private CognitiveCompanion recruitedCompanion;
        private DemoSceneController sceneController;
        
        void Start()
        {
            sceneController = FindObjectOfType<DemoSceneController>();
        }
        
        void Update()
        {
            HandleMovement();
            FindNearestNPC();
            HandleInteractions();
        }
        
        void HandleMovement()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            Vector3 movement = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
            transform.position += movement;
            
            // Make companion follow
            if (recruitedCompanion != null)
            {
                Vector3 followPos = transform.position - transform.forward * 2f;
                recruitedCompanion.transform.position = Vector3.Lerp(
                    recruitedCompanion.transform.position,
                    followPos,
                    Time.deltaTime * 3f
                );
            }
        }
        
        void FindNearestNPC()
        {
            CognitiveCompanion[] npcs = FindObjectsOfType<CognitiveCompanion>();
            float closestDistance = interactionRange;
            nearestNPC = null;
            
            foreach (var npc in npcs)
            {
                float distance = Vector3.Distance(transform.position, npc.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestNPC = npc;
                }
            }
        }
        
        void HandleInteractions()
        {
            if (nearestNPC == null) return;
            
            // Talk
            if (Input.GetKeyDown(KeyCode.E))
            {
                sceneController?.SimulatePlayerAction("talked to NPC", nearestNPC);
            }
            
            // Give gift
            if (Input.GetKeyDown(KeyCode.G))
            {
                sceneController?.SimulatePlayerAction("gave a gift", nearestNPC);
            }
            
            // Insult
            if (Input.GetKeyDown(KeyCode.Q))
            {
                sceneController?.SimulatePlayerAction("insulted", nearestNPC);
            }
            
            // Recruit
            if (Input.GetKeyDown(KeyCode.R))
            {
                recruitedCompanion = nearestNPC;
                Debug.Log($"Recruited {nearestNPC.CompanionName}!");
            }
            
            // Test pattern (3x quickly)
            if (Input.GetKeyDown(KeyCode.T))
            {
                for (int i = 0; i < 3; i++)
                {
                    nearestNPC.ObserveAction("test pattern action", "testing");
                }
            }
            
            // Debug memory
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Debug.Log($"=== {nearestNPC.CompanionName} Memory ===");
                Debug.Log(nearestNPC.GetRelationshipSummary());
            }
        }
        
        void OnGUI()
        {
            if (nearestNPC != null)
            {
                GUI.Box(new Rect(Screen.width - 210, 10, 200, 60), "Nearby NPC");
                GUI.Label(new Rect(Screen.width - 200, 30, 180, 30), 
                    $"{nearestNPC.CompanionName}\nPress E/G/Q to interact");
            }
        }
    }
}
