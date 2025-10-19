using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AdaptiveNPC
{
    public class OpenAIProvider : IResponseProvider
    {
        private readonly string personality;
        private string apiKey;
        private readonly string endpoint = "https://api.openai.com/v1/chat/completions";
        
        public OpenAIProvider(string personality)
        {
            this.personality = personality;
            LoadAPIKey();
        }
        
        public bool IsConfigured()
        {
            return !string.IsNullOrEmpty(apiKey);
        }
        
        private void LoadAPIKey()
        {
            // Try multiple sources
            // 1. Environment variable
            apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            
            // 2. Unity PlayerPrefs (for testing)
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = PlayerPrefs.GetString("AdaptiveNPC_OpenAI_Key", "");
            }
            
            // 3. Config file in StreamingAssets
            if (string.IsNullOrEmpty(apiKey))
            {
                LoadFromConfig();
            }
        }
        
        private void LoadFromConfig()
        {
            string configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "adaptivenpc_config.json");
            if (System.IO.File.Exists(configPath))
            {
                string json = System.IO.File.ReadAllText(configPath);
                var config = JsonUtility.FromJson<AIConfig>(json);
                apiKey = config.openai_api_key;
            }
        }
        
        public async Task<string> GenerateResponse(ResponseRequest request)
        {
            if (!IsConfigured())
            {
                Debug.LogWarning("[AdaptiveNPC] OpenAI API key not configured");
                return null;
            }
            
            string prompt = BuildPrompt(request);
            
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = prompt },
                    new { role = "user", content = request.Action }
                },
                max_tokens = 60,
                temperature = 0.7f
            };
            
            string jsonBody = JsonUtility.ToJson(requestBody);
            
            using (var webRequest = UnityWebRequest.Post(endpoint, jsonBody, "application/json"))
            {
                webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                webRequest.timeout = 10;
                
                var operation = webRequest.SendWebRequest();
                
                while (!operation.isDone)
                    await Task.Yield();
                
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<OpenAIResponse>(webRequest.downloadHandler.text);
                    return response.choices[0].message.content;
                }
                else
                {
                    Debug.LogError($"[AdaptiveNPC] OpenAI request failed: {webRequest.error}");
                    return null;
                }
            }
        }
        
        private string BuildPrompt(ResponseRequest request)
        {
            string prompt = $"You are {request.CompanionName}, {personality}.";
            
            if (!string.IsNullOrEmpty(request.Memory))
            {
                prompt += $"\nPlayer info: {request.Memory}";
            }
            
            if (request.Pattern != null && request.Pattern.Count >= 3)
            {
                prompt += $"\nYou've noticed the player {request.Pattern.Action} frequently ({request.Pattern.Count} times).";
            }
            
            prompt += $"\nThe player just: {request.Action}";
            prompt += "\nRespond naturally in 1-2 short sentences.";
            
            return prompt;
        }
        
        [Serializable]
        private class AIConfig
        {
            public string openai_api_key;
        }
        
        [Serializable]
        private class OpenAIResponse
        {
            public Choice[] choices;
        }
        
        [Serializable]
        private class Choice
        {
            public Message message;
        }
        
        [Serializable]
        private class Message
        {
            public string content;
        }
    }
}
