using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AdaptiveNPC
{
    /// <summary>
    /// Provides template-based responses without requiring AI
    /// </summary>
    public class TemplateProvider : IResponseProvider
    {
        #region Fields
        
        private readonly Dictionary<string, ResponseTemplate> templates;
        private readonly Queue<string> recentResponses;
        private readonly int maxRecentResponses = 5;
        
        #endregion
        
        #region Constructor
        
        public TemplateProvider()
        {
            recentResponses = new Queue<string>(maxRecentResponses);
            templates = InitializeTemplates();
        }
        
        #endregion
        
        #region IResponseProvider Implementation
        
        public async Task<string> GenerateResponse(ResponseRequest request)
        {
            // Simulate minimal async delay for consistency
            await Task.Yield();
            
            string response = SelectResponse(request);
            
            // Avoid repetition
            if (!string.IsNullOrEmpty(response) && !recentResponses.Contains(response))
            {
                if (recentResponses.Count >= maxRecentResponses)
                    recentResponses.Dequeue();
                recentResponses.Enqueue(response);
            }
            
            return response;
        }
        
        #endregion
        
        #region Response Selection
        
        private string SelectResponse(ResponseRequest request)
        {
            // Priority 1: Pattern-based responses
            if (request.Pattern != null && request.Pattern.IsSignificant)
            {
                string patternResponse = GetPatternResponse(request.Pattern);
                if (!string.IsNullOrEmpty(patternResponse))
                    return PersonalizeResponse(patternResponse, request);
            }
            
            // Priority 2: Context-specific responses
            string contextResponse = GetContextResponse(request.Action, request.Context);
            if (!string.IsNullOrEmpty(contextResponse))
                return PersonalizeResponse(contextResponse, request);
            
            // Priority 3: Action-based responses
            string actionResponse = GetActionResponse(request.Action);
            if (!string.IsNullOrEmpty(actionResponse))
                return PersonalizeResponse(actionResponse, request);
            
            // Fallback: Generic response
            return GetFallbackResponse(request);
        }
        
        private string GetPatternResponse(Pattern pattern)
        {
            string key = $"{pattern.Action}_pattern_{GetPatternLevel(pattern.Count)}";
            
            if (templates.ContainsKey(key))
            {
                return templates[key].GetRandom();
            }
            
            // Generic pattern recognition
            if (pattern.Count >= 5)
                return templates["generic_pattern_high"].GetRandom();
            if (pattern.Count >= 3)
                return templates["generic_pattern_medium"].GetRandom();
            
            return null;
        }
        
        private string GetContextResponse(string action, string context)
        {
            // Combine action and context for specific responses
            string actionKey = ExtractActionKey(action);
            string contextKey = $"{actionKey}_{context}";
            
            if (templates.ContainsKey(contextKey))
            {
                return templates[contextKey].GetRandom();
            }
            
            // Try context alone
            if (templates.ContainsKey($"context_{context}"))
            {
                return templates[$"context_{context}"].GetRandom();
            }
            
            return null;
        }
        
        private string GetActionResponse(string action)
        {
            string actionKey = ExtractActionKey(action);
            
            if (templates.ContainsKey($"action_{actionKey}"))
            {
                return templates[$"action_{actionKey}"].GetRandom();
            }
            
            return null;
        }
        
        private string GetFallbackResponse(ResponseRequest request)
        {
            // Use memory summary to provide context
            if (!string.IsNullOrEmpty(request.Memory) && request.Memory.Contains("generous"))
            {
                return "You're always so kind.";
            }
            
            if (!string.IsNullOrEmpty(request.Memory) && request.Memory.Contains("aggressive"))
            {
                return "Please, no trouble.";
            }
            
            return templates["fallback"].GetRandom();
        }
        
        private string PersonalizeResponse(string template, ResponseRequest request)
        {
            // Replace tokens with actual values
            string result = template;
            
            if (!string.IsNullOrEmpty(request.CompanionName))
                result = result.Replace("{name}", request.CompanionName);
            
            if (request.Pattern != null)
                result = result.Replace("{count}", request.Pattern.Count.ToString());
            
            result = result.Replace("{action}", request.Action);
            
            return result;
        }
        
        #endregion
        
        #region Template Initialization
        
        private Dictionary<string, ResponseTemplate> InitializeTemplates()
        {
            var dict = new Dictionary<string, ResponseTemplate>();
            
            // Pattern responses - by frequency
            dict["gift_pattern_low"] = new ResponseTemplate(
                "You're quite generous today.",
                "That's very kind of you.",
                "Another gift? How thoughtful."
            );
            
            dict["gift_pattern_medium"] = new ResponseTemplate(
                "You really like giving gifts, don't you?",
                "So generous! This is the {count}th gift!",
                "I'm starting to see a pattern here..."
            );
            
            dict["gift_pattern_high"] = new ResponseTemplate(
                "Your generosity knows no bounds!",
                "Are you trying to buy my friendship?",
                "I've lost count of your gifts!"
            );
            
            dict["attack_pattern_low"] = new ResponseTemplate(
                "Why the violence?",
                "That's not very nice!",
                "Please stop that."
            );
            
            dict["attack_pattern_medium"] = new ResponseTemplate(
                "You're quite aggressive today.",
                "Always with the fighting...",
                "This is the {count}th time you've attacked!"
            );
            
            dict["attack_pattern_high"] = new ResponseTemplate(
                "You really have anger issues!",
                "Violence is all you know, isn't it?",
                "I'm tired of your constant aggression."
            );
            
            dict["talk_pattern_medium"] = new ResponseTemplate(
                "You're quite the conversationalist!",
                "Always happy to chat with you.",
                "We do talk a lot, don't we?"
            );
            
            // Context-specific responses
            dict["context_combat"] = new ResponseTemplate(
                "The battle rages on!",
                "Watch out!",
                "Stay alert!"
            );
            
            dict["context_shop"] = new ResponseTemplate(
                "Looking to trade?",
                "See anything you like?",
                "Best prices in town!"
            );
            
            dict["context_quest"] = new ResponseTemplate(
                "An adventure awaits!",
                "This quest won't complete itself.",
                "Ready for action?"
            );
            
            // Action responses
            dict["action_gift"] = new ResponseTemplate(
                "Oh, thank you!",
                "How kind of you!",
                "I appreciate this."
            );
            
            dict["action_attack"] = new ResponseTemplate(
                "Ouch! Why?",
                "Stop that!",
                "That hurt!"
            );
            
            dict["action_talk"] = new ResponseTemplate(
                "Hello there!",
                "Nice to see you.",
                "What brings you here?"
            );
            
            dict["action_help"] = new ResponseTemplate(
                "Thank you for your help!",
                "I couldn't have done it without you.",
                "You're a true friend."
            );
            
            dict["action_insult"] = new ResponseTemplate(
                "That's not very nice.",
                "How rude!",
                "I don't appreciate that."
            );
            
            dict["action_steal"] = new ResponseTemplate(
                "Hey! That's mine!",
                "Thief!",
                "Give that back!"
            );
            
            // Generic pattern responses
            dict["generic_pattern_medium"] = new ResponseTemplate(
                "I'm noticing a pattern here.",
                "You do that a lot.",
                "This is becoming a habit for you."
            );
            
            dict["generic_pattern_high"] = new ResponseTemplate(
                "You really can't help yourself, can you?",
                "Same old behavior from you.",
                "Predictable as always."
            );
            
            // Fallback responses
            dict["fallback"] = new ResponseTemplate(
                "I see.",
                "Interesting.",
                "Hmm.",
                "Noted.",
                "Alright then.",
                "If you say so."
            );
            
            return dict;
        }
        
        private string ExtractActionKey(string action)
        {
            string lower = action.ToLower();
            
            var keywords = new[]
            {
                "gift", "give", "attack", "hit", "fight",
                "talk", "speak", "chat", "help", "assist",
                "insult", "mock", "steal", "take", "trade"
            };
            
            foreach (var keyword in keywords)
            {
                if (lower.Contains(keyword))
                {
                    // Map variations to base actions
                    if (keyword == "give") return "gift";
                    if (keyword == "hit" || keyword == "fight") return "attack";
                    if (keyword == "speak" || keyword == "chat") return "talk";
                    if (keyword == "assist") return "help";
                    if (keyword == "mock") return "insult";
                    if (keyword == "take") return "steal";
                    
                    return keyword;
                }
            }
            
            return "misc";
        }
        
        private string GetPatternLevel(int count)
        {
            if (count >= 10) return "high";
            if (count >= 5) return "medium";
            return "low";
        }
        
        #endregion
        
        #region Helper Classes
        
        private class ResponseTemplate
        {
            private readonly List<string> responses;
            private int lastIndex = -1;
            
            public ResponseTemplate(params string[] responses)
            {
                this.responses = responses.ToList();
            }
            
            public string GetRandom()
            {
                if (responses.Count == 0) return "";
                
                // Avoid repeating the last response if possible
                int index;
                if (responses.Count == 1)
                {
                    index = 0;
                }
                else
                {
                    do
                    {
                        index = Random.Range(0, responses.Count);
                    } while (index == lastIndex && responses.Count > 1);
                }
                
                lastIndex = index;
                return responses[index];
            }
        }
        
        #endregion
    }
}
