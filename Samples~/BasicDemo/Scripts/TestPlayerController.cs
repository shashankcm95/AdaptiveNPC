using UnityEngine;
using System.Collections;
using AdaptiveNPC;

namespace AdaptiveNPC.Samples
{
    public class TestPlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float rotateSpeed = 120f;
        
        [Header("Interaction")]
        public float interactionRange = 2f;
        private CognitiveCompanion nearestNPC;
        private CognitiveCompanion targetNPC;
        
        [Header("Visual")]
        private Vector3 targetPosition;
        private bool isMovingToTarget = false;
        
        void Update()
        {
            HandleManualControls();
            HandleAutomatedMovement();
            FindNearestNPC();
            HandleInteractions();
        }
        
        void HandleManualControls()
        {
            if (!isMovingToTarget)
            {
                // WASD movement
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                
                Vector3 movement = new Vector3(h, 0, v).normalized;
                transform.position += movement * moveSpeed * Time.deltaTime;
                
                // Face movement direction
                if (movement.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movement);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                        rotateSpeed * Time.deltaTime);
                }
            }
        }
        
        void HandleAutomatedMovement()
        {
            if (isMovingToTarget && targetNPC != null)
            {
                Vector3 direction = (targetPosition - transform.position);
                direction.y = 0;
                
                if (direction.magnitude > interactionRange)
                {
                    transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                    
                    // Face target
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                        rotateSpeed * Time.deltaTime);
                }
                else
                {
                    isMovingToTarget = false;
                }
            }
        }
        
        void FindNearestNPC()
        {
            CognitiveCompanion[] allNPCs = FindObjectsOfType<CognitiveCompanion>();
            float closestDistance = float.MaxValue;
            nearestNPC = null;
            
            foreach (var npc in allNPCs)
            {
                float distance = Vector3.Distance(transform.position, npc.transform.position);
                if (distance < closestDistance && distance < interactionRange * 2)
                {
                    closestDistance = distance;
                    nearestNPC = npc;
                }
            }
        }
        
        void HandleInteractions()
        {
            if (nearestNPC == null) return;
            
            // Manual interaction keys
            if (Input.GetKeyDown(KeyCode.E))
            {
                nearestNPC.ObserveAction("talked to", "manual");
                ShowInteractionFeedback("Talk", nearestNPC.transform.position);
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                nearestNPC.ObserveAction("gave gift", "manual");
                ShowInteractionFeedback("Gift", nearestNPC.transform.position);
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                nearestNPC.ObserveAction("insulted", "manual");
                ShowInteractionFeedback("Insult", nearestNPC.transform.position);
            }
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                nearestNPC.ObserveAction("attacked", "manual");
                ShowInteractionFeedback("Attack", nearestNPC.transform.position);
            }
        }
        
        public void MoveToNPC(Transform npc)
        {
            targetNPC = npc.GetComponent<CognitiveCompanion>();
            targetPosition = npc.position;
            targetPosition.y = transform.position.y;
            isMovingToTarget = true;
        }
        
        void ShowInteractionFeedback(string action, Vector3 position)
        {
            Debug.Log($"Player → {action} → {nearestNPC.CompanionName}");
        }
        
        void OnDrawGizmos()
        {
            // Draw interaction range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
            
            // Draw line to nearest NPC
            if (nearestNPC != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, nearestNPC.transform.position);
            }
        }
    }
}
