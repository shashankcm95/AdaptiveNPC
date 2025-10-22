using System;
using System.Collections.Generic;
using System.Threading.Tasks;  // ADD THIS
using UnityEngine;
using UnityEngine.Networking;

namespace AdaptiveNPC
{
    public class OpenAIProvider : IResponseProvider
    {
        private string apiKey;
        private readonly string endpoint = "https://api.openai.com/v1/chat/completions";
        private readonly string personality;  // ADD THIS FIELD
        
        // FIX: Add personality parameter
        public OpenAIProvider(string personality = "friendly and helpful")
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
            // Try environment variable first
            apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            
            // Try PlayerPrefs
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = PlayerPrefs.GetString("AdaptiveNPC_OpenAI_Key", "");
            }
            
            // Try config file
            if (string.IsNullOrEmpty(apiKey))
            {
                LoadFromConfig();
            }
            
            if (!string.IsNullOrEmpty(apiKey))
            {
                Debug.Log("[OpenAIProvider] API key loaded successfully");
            }
        }
        
        private void LoadFromConfig()
        {
            try
            {
                string configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "adaptivenpc_config.json");
                if (System.IO.File.Exists(configPath))
                {
                    string json = System.IO.File.ReadAllText(configPath);
                    var config = JsonUtility.FromJson<AIConfig>(json);
                    apiKey = config.openai_api_key;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[OpenAIProvider] Could not load config: {e.Message}");
            }
        }
        
        // FIX: Proper async implementation with error handling
        public async Task<string> GenerateResponse(ResponseRequest request)
        {
            if (!IsConfigured())
            {
                Debug.LogWarning("[OpenAIProvider] API key not configured, returning null");
                return null;
            }
            
            try
            {
                string prompt = BuildPrompt(request);
                
                var requestBody = new OpenAIRequest
                {
                    model = "gpt-3.5-turbo",
                    messages = new List<Message>
                    {
                        new Message { role = "system", content = prompt },
                        new Message { role = "user", content = request.Action }
                    },
                    max_tokens = 60,
                    temperature = 0.7f
                };
                
                string jsonBody = JsonUtility.ToJson(requestBody);
                
                using (var webRequest = UnityWebRequest.Post(endpoint, jsonBody, "application/json"))
                {
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    webRequest.disposeDownloadHandlerOnDispose = true;
                    webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                    webRequest.SetRequestHeader("Content-Type", "application/json");
                    webRequest.timeout = 10;
                    
                    var operation = webRequest.SendWebRequest();
                    
                    while (!operation.isDone)
                        await Task.Yield();
                    
                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        var response = JsonUtility.FromJson<OpenAIResponse>(webRequest.downloadHandler.text);
                        return response?.choices?[0]?.message?.content ?? "";
                    }
                    else
                    {
                        Debug.LogError($"[OpenAIProvider] Request failed: {webRequest.error}");
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[OpenAIProvider] Exception: {e.Message}");
                return null;
            }
        }
        
        private string BuildPrompt(ResponseRequest request)
        {
            return $@"You are {request.CompanionName}, {personality}.
Player info: {request.Memory}
The player just: {request.Action}
Context: {request.Context}
Respond briefly and naturally (1-2 sentences):";
        }
        
        [Serializable]
        private class AIConfig
        {
            public string openai_api_key;
        }
        
        [Serializable]
        private class OpenAIRequest
        {
            public string model;
            public List<Message> messages;
            public int max_tokens;
            public float temperature;
        }
        
        [Serializable]
        private class Message
        {
            public string role;
            public string content;
        }
        
        [Serializable]
        private class OpenAIResponse
        {
            public List<Choice> choices;
        }
        
        [Serializable]
        private class Choice
        {
            public Message message;
        }
    }
}
