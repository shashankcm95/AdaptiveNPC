using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace AdaptiveNPC
{
    public class TemplateProvider : IResponseProvider
    {
        private Dictionary<string, List<string>> templates;
        
        public TemplateProvider()
        {
            InitializeTemplates();
        }
        
        private void InitializeTemplates()
        {
            templates = new Dictionary<string, List<string>>
            {
                ["gift_first"] = new List<string>
                {
                    "That's kind of you.",
                    "A gift? How thoughtful.",
                    "You're quite generous."
                },
                ["gift_pattern"] = new List<string>
                {
                    "You really like giving gifts, don't you?",
                    "Your generosity knows no bounds.",
                    "Always the charitable one."
                },
                ["attack_first"] = new List<string>
                {
                    "Hey! What was that for?",
                    "Violence isn't the answer!",
                    "That was uncalled for."
                },
                ["attack_pattern"] = new List<string>
                {
                    "You're quite aggressive today.",
                    "Always choosing violence...",
                    "Maybe try talking instead?"
                },
                ["talk_pattern"] = new List<string>
                {
                    "You're quite the conversationalist.",
                    "Always eager for a chat, aren't you?",
                    "I enjoy our talks."
                },
                ["default"] = new List<string>
                {
                    "Interesting...",
                    "I see.",
                    "Hmm, noted."
                }
            };
        }
        
        public Task<string> GenerateResponse(ResponseRequest request)
        {
            string templateKey = DetermineTemplateKey(request);
            
            if (templates.ContainsKey(templateKey))
            {
                var options = templates[templateKey];
                return Task.FromResult(options[Random.Range(0, options.Count)]);
            }
            
            return Task.FromResult(templates["default"][Random.Range(0, templates["default"].Count)]);
        }
        
        public Task<string> GetAlternativeResponse(ResponseRequest request)
        {
            return Task.FromResult("I see what you're doing there.");
        }
        
        private string DetermineTemplateKey(ResponseRequest request)
        {
            if (request.Pattern != null)
            {
                string action = request.Pattern.Action;
                bool isPattern = request.Pattern.Count >= 3;
                
                if (isPattern)
                    return $"{action}_pattern";
                else
                    return $"{action}_first";
            }
            
            return "default";
        }
    }
}
