# Adaptive NPC - AI-Powered Companions for Unity

[![Unity 2021.3+](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.0.1-orange.svg)](https://github.com/shashankcm95/AdaptiveNPC/releases)

Bring your NPCs to life with memory, pattern recognition, and contextual responses. No complex AI setup required.

## ✨ Features

- 🧠 **Memory System** - NPCs remember player actions across sessions
- 🎯 **Pattern Recognition** - Detects and responds to repeated behaviors  
- 💬 **Smart Responses** - Template-based or AI-powered (OpenAI) dialogue
- 💾 **Automatic Persistence** - Saves NPC memories between game sessions
- 🎮 **Drop-in Component** - Single component, zero configuration required
- 🔌 **Extensible** - Custom save systems and response providers supported

## 🚀 Quick Start (2 minutes)

### Installation

#### Option 1: Unity Package Manager (Recommended)
1. Open Unity 2021.3 or newer
2. Window → Package Manager → **+** → Add package from git URL
3. Paste: `https://github.com/shashankcm95/AdaptiveNPC.git`
4. Click Add

#### Option 2: Direct Download
1. Download [Latest Release](https://github.com/shashankcm95/AdaptiveNPC/releases)
2. Import the `.unitypackage` into your project

### Basic Usage
```csharp
using UnityEngine;
using AdaptiveNPC;

public class MyNPC : MonoBehaviour
{
    private CognitiveCompanion ai;
    
    void Start()
    {
        // Add AI to any NPC - that's it!
        ai = gameObject.AddComponent<CognitiveCompanion>();
        
        // Listen for responses (optional)
        ai.OnResponse += (response) => Debug.Log($"NPC: {response}");
        
        // Listen for patterns (optional)
        ai.OnPatternRecognized += (action, count) => 
            Debug.Log($"Player did {action} {count} times!");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // NPC observes what the player does
            ai.ObserveAction("approached", "proximity");
        }
    }
}
```

## 🎮 Test the Library

After installation, test everything works:

1. **Menu Bar** → AdaptiveNPC → Quick Test
2. Check console for "✅ All tests passed!"

Or create a test scene:

1. **Menu Bar** → AdaptiveNPC → Create Test Scene
2. Press **Play**
3. Watch the automated demo

## 📖 Core Concepts

### ObserveAction - The Heart of the System
```csharp
companion.ObserveAction(action, context);
```

- **action**: What the player did ("gave gift", "attacked", "talked to")
- **context**: Optional context ("shop", "combat", "quest")

### Memory System

NPCs build a personality profile of the player:
```csharp
// After multiple interactions:
companion.GetRelationshipSummary();
// Returns: "Encounters: 15, Trait: generous (80%), Recent: gave gift, helped, traded"
```

### Pattern Recognition

NPCs notice repeated behaviors:
```csharp
// After player gives 3 gifts:
OnPatternRecognized("gift", 3)  // "You're always so generous!"

// After player attacks 5 times:
OnPatternRecognized("attack", 5)  // "Violence is all you know!"
```

## 🤖 AI Integration (Optional)

The library works great with templates only, but for more natural responses, add OpenAI:

### Setup OpenAI

1. Get API key from [OpenAI](https://platform.openai.com/api-keys)
2. Add key using ONE of these methods:

**Method A: Environment Variable** (Recommended)
```bash
# Windows
setx OPENAI_API_KEY "sk-your-key-here"

# Mac/Linux
export OPENAI_API_KEY="sk-your-key-here"
```

**Method B: In Unity**
```csharp
PlayerPrefs.SetString("AdaptiveNPC_OpenAI_Key", "sk-your-key");
```

**Method C: Config File**
Create `Assets/StreamingAssets/adaptivenpc_config.json`:
```json
{
    "openai_api_key": "sk-your-key-here"
}
```

### Response Modes

| Mode | Description | Cost | Quality |
|------|-------------|------|---------|
| **Template** | Pre-written responses | Free | Good |
| **AI** | Always uses OpenAI | $0.002/response | Excellent |
| **Hybrid** | Smart mix (default) | $0.0006/response | Very Good |

## 📚 Examples

### Shop NPC
```csharp
public class ShopKeeper : MonoBehaviour
{
    private CognitiveCompanion ai;
    
    void Start()
    {
        ai = GetComponent<CognitiveCompanion>();
    }
    
    public void OnPurchase(Item item)
    {
        ai.ObserveAction($"bought {item.name}", "shop");
        // After 3 purchases: "You're one of my best customers!"
    }
    
    public void OnHaggle()
    {
        ai.ObserveAction("tried to haggle", "shop");
        // After 3 haggles: "You always try to negotiate!"
    }
}
```

### Quest Giver
```csharp
public class QuestGiver : MonoBehaviour
{
    private CognitiveCompanion ai;
    
    void Start()
    {
        ai = GetComponent<CognitiveCompanion>();
        ai.OnResponse += ShowDialogue;
    }
    
    public void OnQuestComplete(Quest quest)
    {
        ai.ObserveAction($"completed {quest.type} quest", "quest");
        // Remembers player's quest preferences
    }
}
```

### Combat NPC
```csharp
public class EnemyAI : MonoBehaviour
{
    private CognitiveCompanion ai;
    
    void Start()
    {
        ai = GetComponent<CognitiveCompanion>();
        ai.OnPatternRecognized += AdaptCombatStyle;
    }
    
    void AdaptCombatStyle(string pattern, int count)
    {
        if (pattern == "dodge" && count >= 3)
        {
            // Player dodges a lot, switch tactics
            UseAreaAttacks();
        }
    }
}
```

## 🛠️ Advanced Configuration

### Custom Save System
```csharp
public class MySaveProvider : ISaveProvider
{
    public void Save(string key, string data)
    {
        // Your save logic
        MyGameSaves.Write(key, data);
    }
    
    public string Load(string key)
    {
        return MyGameSaves.Read(key);
    }
}

// Use it
companion.SetSaveProvider(new MySaveProvider());
```

### Component Settings

In the Inspector:
- **Companion Name**: NPC's name for responses
- **Personality**: Brief personality description
- **Response Mode**: Template/AI/Hybrid
- **Persist Memory**: Enable save/load
- **Max Memories**: Memory limit (default: 50)

## 📊 Performance

- **Memory**: ~1KB per NPC
- **CPU**: < 0.1ms per observation
- **Response Time**: 
  - Template: Instant
  - AI: 0.5-2 seconds
- **Persistence**: PlayerPrefs (all platforms)

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| "No response from NPC" | Check Console for errors. Verify component is added. |
| "AI not working" | Templates work without API key. Check key configuration. |
| "Memory not saving" | Ensure "Persist Memory" is enabled in Inspector. |
| "Compilation errors" | Requires Unity 2021.3.0f1 or newer. |

## 📄 API Reference

### CognitiveCompanion

**Methods**
- `ObserveAction(string action, string context = "")` - Record player action
- `GetRelationshipSummary()` - Get memory summary
- `ResetCompanion()` - Clear all memories

**Events**
- `OnResponse(string response)` - Fired when NPC responds
- `OnPatternRecognized(string action, int count)` - Fired at 3+ repetitions

**Properties**
- `CompanionName` - NPC's name
- `IsInitialized` - Ready state

## 🤝 Contributing

Contributions welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) first.

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📜 License

MIT License - see [LICENSE](LICENSE) file

## 💬 Support

- **Issues**: [GitHub Issues](https://github.com/shashankcm95/AdaptiveNPC/issues)
- **Discussions**: [GitHub Discussions](https://github.com/shashankcm95/AdaptiveNPC/discussions)
- **Email**: contact@shashankcm.com

## 🏆 Credits

Created by [Shashank CM](https://github.com/shashankcm95)

Special thanks to contributors and the Unity community.

---

**Making NPCs Memorable, One Interaction at a Time** 🎮
