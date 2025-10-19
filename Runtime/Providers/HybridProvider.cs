using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AdaptiveNPC
{
    public class HybridProvider : IResponseProvider
    {
        private readonly string personality;
        private readonly float aiThreshold;
        private readonly TemplateProvider templateProvider;
        private readonly OpenAIProvider aiProvider;
        private Queue<string> recentResponses;
        
        public HybridProvider(string personality, float aiThreshold = 0.7f)
        {
            this.personality = personality;
            this.aiThreshold = aiThreshold;
            this.templateProvider = new TemplateProvider();
            this.aiProvider = new OpenAIProvider(personality);
            this.recentResponses = new Queue<string>(5);
        }
        
        public async Task<string> GenerateResponse(ResponseRequest request)
        {
            // Determine if we should use AI
            bool useAI = ShouldUseAI(request);
            
            string response;
            
            if (useAI && aiProvider.IsConfigured())
            {
                try
                {
                    response = await aiProvider.GenerateResponse(request);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"AI generation failed, falling back to template: {e.Message}");
                    response = await templateProvider.GenerateResponse(request);
                }
            }
            else
            {
                response = await templateProvider.GenerateResponse(request);
            }
            
            // Track recent responses to avoid repetition
            if (!recentResponses.Contains(response))
            {
                recentResponses.Enqueue(response);
                if (recentResponses.Count > 5)
                    recentResponses.Dequeue();
            }
            else
            {
                // Get alternative if we've said this recently
                response = await templateProvider.GetAlternativeResponse(request);
            }
            
            return response;
        }
        
        private bool ShouldUseAI(ResponseRequest request)
        {
            // Important events always use AI if available
            if (request.Pattern != null && (request.Pattern.Count == 3 || request.Pattern.Count == 10))
                return true;
            
            // Random chance based on threshold
            return Random.value < aiThreshold;
        }
    }
}
