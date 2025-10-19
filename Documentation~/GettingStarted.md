# Getting Started with Adaptive NPC

## Installation

### Method 1: Unity Package Manager (Recommended)
1. Open Unity 2021.3 or later
2. Window → Package Manager
3. Click **+** → Add package from git URL
4. Enter: `https://github.com/shashankcm95/AdaptiveNPC.git`

### Method 2: Download Package
1. Download the latest release from [GitHub Releases](https://github.com/shashankcm95/AdaptiveNPC/releases)
2. Import the .unitypackage file into your project

## Quick Start

### 1. Basic Setup

Add the CognitiveCompanion component to any GameObject:
```csharp
using AdaptiveNPC;

GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
CognitiveCompanion companion = npc.AddComponent<CognitiveCompanion>();
```

### 2. Configure the NPC

In the Inspector:
- **Companion Name**: Give your NPC a name
- **Personality**: Describe their personality
- **Response Mode**: Choose between Template, AI, or Hybrid
- **Persist Memory**: Enable to save between sessions

### 3. Observe Player Actions
```csharp
// When player does something
companion.ObserveAction("gave gift", "interaction");
companion.ObserveAction("attacked enemy", "combat");
companion.ObserveAction("talked to merchant", "social");
```

### 4. Handle Responses
```csharp
companion.OnResponse += (response) => {
    Debug.Log($"NPC says: {response}");
    // Show in UI, speech bubble, etc.
};

companion.OnPatternRecognized += (action, count) => {
    Debug.Log($"NPC noticed you {action} {count} times!");
};
```

## Running the Demo

1. Open the demo scene: `Samples~/BasicDemo/Scenes/DemoScene.unity`
2. Or create one: Menu → AdaptiveNPC → Setup Demo Scene
3. Press Play
4. Use WASD to move, E/G/Q to interact with NPCs

## OpenAI Integration (Optional)

For AI-powered responses:

1. Get an API key from [OpenAI](https://platform.openai.com)
2. Add it using one of these methods:
   - Environment variable: `OPENAI_API_KEY=your-key`
   - PlayerPrefs: `PlayerPrefs.SetString("AdaptiveNPC_OpenAI_Key", "your-key")`
   - Config file: Create `StreamingAssets/adaptivenpc_config.json`
```json
{
    "openai_api_key": "your-api-key"
}
```

The system works without AI using smart templates, but AI makes responses more natural.

## Next Steps

- Read the [API Reference](APIReference.md) for detailed documentation
- Check [Integration Guide](Integration.md) for existing projects
- See [Examples](Examples.md) for common use cases
