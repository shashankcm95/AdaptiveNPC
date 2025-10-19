# AdaptiveNPC
Unit# Adaptive NPC - Cognitive Companion Framework

AI-powered NPCs that remember players and adapt their behavior through pattern recognition and contextual responses.

## ✨ Features

- **Pattern Recognition** - NPCs learn and recognize player behavior patterns
- **Contextual AI Responses** - Natural dialogue using GPT-3.5 or smart templates
- **Persistent Memory** - Companions remember across game sessions
- **Hybrid System** - Optimized mix of AI and template responses for cost efficiency
- **Easy Integration** - Single component, minimal setup

## 📦 Installation

### Via Unity Package Manager

1. Open Package Manager (Window → Package Manager)
2. Click "+" → "Add package from git URL"
3. Enter: `https://github.com/shashankcm95/AdaptiveNPC.git`

### Via OpenUPM
```bash
openupm add com.shashankcm.adaptivenpc
```

## 🚀 Quick Start
```csharp
using AdaptiveNPC;

// Add to any NPC
var companion = npc.AddComponent<CognitiveCompanion>();

// Configure
companion.SetPersonality("Friendly bartender with a gossipy nature");

// NPC observes player actions
companion.ObserveAction("gave gift", "interaction");

// NPC responds based on patterns
// "You're always so generous! Third gift today!"
```

## 📖 Documentation

- [Getting Started](Documentation~/GettingStarted.md)
- [API Reference](Documentation~/APIReference.md)
- [Integration Guide](Documentation~/Integration.md)

## 🎮 Example Usage
```csharp
public class NPCInteraction : MonoBehaviour
{
    private CognitiveCompanion companion;
    
    void Start()
    {
        companion = GetComponent<CognitiveCompanion>();
        companion.OnResponse += HandleCompanionResponse;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            companion.ObserveAction("approached", "movement");
        }
    }
    
    void HandleCompanionResponse(string response)
    {
        // Display speech bubble, play audio, etc.
        Debug.Log($"Companion says: {response}");
    }
}
```

## 🧪 Tested With

- Unity 2021.3 LTS and above
- OpenAI GPT-3.5-turbo
- Tested with 100+ NPCs simultaneously

## 💡 Performance

- **Hybrid Mode**: ~70% fewer API calls
- **Cost**: ~$0.02 per hour of gameplay
- **Memory**: < 1KB per NPC
- **Response Time**: < 100ms (template), < 1s (AI)

## 📄 License

MIT License - see [LICENSE](LICENSE)

## 🤝 Contributing

Contributions welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) first.

## 📮 Support

- [Report Issues](https://github.com/shashankcm95/AdaptiveNPC/issues)
- [Discussions](https://github.com/shashankcm95/AdaptiveNPC/discussions)

---
Made with ❤️ for game developers who want NPCs that feel alivey framework for NPCs that remember players and build relationships
