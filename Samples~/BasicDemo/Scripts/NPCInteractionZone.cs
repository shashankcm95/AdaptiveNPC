using UnityEngine;
using AdaptiveNPC;

namespace AdaptiveNPC.Samples
{
    public class NPCInteractionZone : MonoBehaviour
    {
        private CognitiveCompanion companion;
        private bool playerInRange = false;
        
        void Start()
        {
            companion = GetComponentInParent<CognitiveCompanion>();
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && companion != null)
            {
                playerInRange = true;
                companion.ObserveAction("player approached", "proximity");
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && companion != null)
            {
                playerInRange = false;
                companion.ObserveAction("player left", "proximity");
            }
        }
    }
}
