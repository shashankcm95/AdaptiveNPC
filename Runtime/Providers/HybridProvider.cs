using System;
using System.Collections.Generic;
using System.Threading.Tasks;  // ADD THIS
using UnityEngine;

namespace AdaptiveNPC
{
    public class HybridProvider : IResponseProvider
    {
        private readonly string personality;
        private readonly float aiThreshold;
        private readonly TemplateProvider templateProvider;
        private readonly OpenAIProvider aiProvider;
        private readonly Queue<string> recentResponses;
        
        public HybridProvider(string personality = "friendly", float aiThreshold = 0.7f)
        {
            this.personality = personality;
            this.aiThreshold = Mathf.Clamp01(aiThreshold);
            this.templateProvider = new TemplateProvider();
            this.aiProvider = new OpenAIProvider(personality);  // NOW THIS WORKS!
            this.recentResponses = new Queue<string>(5);
        }
        
        public async Task<string> GenerateResponse(ResponseRequest request)
        {
            try
            {
                bool useAI = ShouldUseAI(request);
                string response = null;
                
                if (useAI && aiProvider.IsConfigured())
                {
                    try
                    {
                        response = await aiProvider.GenerateResponse(request);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"[HybridProvider] AI failed, using template: {e.Message}");
                    }
                }
                
                // Fallback to template if AI failed or not configured
                if (string.IsNullOrEmpty(response))
                {
                    response = await templateProvider.GenerateResponse(request);
                }
                
                // Track recent responses
                if (!string.IsNullOrEmpty(response))
                {
                    if (recentResponses.Count >= 5)
                        recentResponses.Dequeue();
                    recentResponses.Enqueue(response);
                }
                
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"[HybridProvider] Critical error: {e.Message}");
                return "..."; // Silent fallback
            }
        }
        
        private bool ShouldUseAI(ResponseRequest request)
        {
            // Important patterns always use AI
            if (request.Pattern != null && request.Pattern.IsSignificant)
                return true;
                
            // Random chance based on threshold
            return UnityEngine.Random.value < aiThreshold;
        }
    }
}
