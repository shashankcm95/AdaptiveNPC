# Adaptive NPC Framework - Complete User Manual

## Table of Contents
1. [What is Adaptive NPC?](#what-is-adaptive-npc)
2. [System Requirements](#system-requirements)
3. [Installation Guide](#installation-guide)
4. [Running Your First Demo](#running-your-first-demo)
5. [Understanding the Framework](#understanding-the-framework)
6. [Setting Up OpenAI (Optional)](#setting-up-openai-optional)
7. [Troubleshooting](#troubleshooting)

---

## What is Adaptive NPC?

Adaptive NPC is a Unity framework that creates NPCs (Non-Player Characters) with:
- **Memory**: NPCs remember player actions and interactions
- **Pattern Recognition**: NPCs notice when players repeat behaviors
- **Contextual Responses**: NPCs respond based on their relationship with the player
- **Persistence**: NPC memories save between game sessions

Think of it as giving your NPCs actual brains that learn and adapt to each player's unique playstyle.

---

## System Requirements

- **Unity Version**: 2021.3 LTS or newer
- **Operating System**: Windows, Mac, or Linux
- **RAM**: 4GB minimum
- **Optional**: OpenAI API key for AI-powered responses (works without it too!)

---

## Installation Guide

### Step 1: Create or Open a Unity Project

1. Open Unity Hub
2. Click "New Project" or open an existing project
3. Ensure you're using Unity 2021.3 or newer

### Step 2: Install from GitHub

#### Method A: Unity Package Manager (Recommended)

1. In Unity, go to **Window** → **Package Manager**
2. Click the **+** button in the top-left corner
3. Select **"Add package from git URL..."**
4. Enter this URL:
```
   https://github.com/shashankcm95/AdaptiveNPC.git
```
5. Click **Add** and wait for import to complete

#### Method B: Download and Import

1. Go to https://github.com/shashankcm95/AdaptiveNPC
2. Click the green **Code** button → **Download ZIP**
3. Extract the ZIP file
4. In Unity: **Assets** → **Import Package** → **Custom Package**
5. Navigate to extracted folder and import

---

## Running Your First Demo

### Quick Demo Setup (Automatic)

1. After importing, go to Unity menu bar
2. Click **AdaptiveNPC** → **Setup Demo Scene**
3. Press **Play** button (▶️)
4. You're running the demo!

### Manual Demo Setup

1. Navigate to: `Samples~/BasicDemo/Scenes/`
2. Double-click **DemoScene.unity** to open it
3. Press **Play** button (▶️)

### Demo Controls

Once the demo is running:

| Key | Action |
|-----|--------|
| **WASD** or **Arrow Keys** | Move player |
| **E** | Talk to nearby NPC |
| **G** | Give gift to NPC |
| **Q** | Insult NPC |
| **R** | Recruit NPC as companion |
| **T** | Test pattern recognition (triggers 3x) |
| **Y** | Show NPC memory debug info |

### What to Try in the Demo

1. **Test Memory**: 
   - Talk to the Merchant (green NPC)
   - Give them a gift
   - Talk again - they'll remember the gift!

2. **Test Patterns**:
   - Give gifts to the same NPC 3 times
   - The NPC will comment on your generosity pattern

3. **Test Relationships**:
   - Insult the Guard (blue NPC) multiple times
   - Notice how their responses become less friendly

4. **Test Persistence**:
   - Interact with NPCs
   - Stop play mode
   - Play again - NPCs remember you!

---

## Understanding the Framework

### Core Components

#### 1. CognitiveCompanion Component
This is the main component you add to any GameObject to make it an adaptive NPC.
```csharp
// Simple setup
GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
CognitiveCompanion companion = npc.AddComponent<CognitiveCompanion>();
```

#### 2. Memory System
NPCs remember what players do:
- Actions (what happened)
- Context (when/where it happened)  
- Patterns (repeated behaviors)
- Traits (player personality assessment)

#### 3. Response Modes

| Mode | Description | Use When |
|------|-------------|----------|
| **Template** | Pre-written responses | No API key, want free operation |
| **AI** | OpenAI-powered responses | Want most natural dialogue |
| **Hybrid** | Mix of both | Want natural dialogue with lower cost |

### Architecture Overview
```
AdaptiveNPC/
├── Runtime/
│   ├── Core/           # Main systems
│   │   ├── CognitiveCompanion.cs
│   │   ├── MemorySystem.cs
│   │   └── PatternRecognizer.cs
│   ├── Interfaces/     # For extensibility
│   ├── Providers/      # Response generation
│   └── SaveSystems/    # Persistence
├── Samples~/           # Demo scenes
├── Documentation~/     # Guides
└── Tests/             # Unit tests
```

---

## Setting Up OpenAI (Optional)

The framework works without AI, but adding OpenAI makes responses more natural.

### Step 1: Get an API Key

1. Go to https://platform.openai.com
2. Sign up or log in
3. Navigate to API Keys
4. Click "Create new secret key"
5. Copy the key (starts with `sk-`)

### Step 2: Add Key to Unity

Choose one method:

#### Option A: Environment Variable (Recommended)
```bash
# Windows (Command Prompt)
setx OPENAI_API_KEY "sk-your-key-here"

# Mac/Linux (Terminal)
export OPENAI_API_KEY="sk-your-key-here"
```

#### Option B: Unity Config File
1. Create folder: `Assets/StreamingAssets/`
2. Create file: `adaptivenpc_config.json`
3. Add:
```json
{
    "openai_api_key": "sk-your-key-here"
}
```

#### Option C: In Code
```csharp
PlayerPrefs.SetString("AdaptiveNPC_OpenAI_Key", "sk-your-key-here");
```

### Cost Estimates

- **Hybrid Mode**: ~$0.02 per hour of gameplay
- **AI Mode**: ~$0.06 per hour of gameplay
- **Template Mode**: Free

---

## Troubleshooting

### Issue: "No NPCs responding"

**Solution**: 
- Check Console for errors (Window → General → Console)
- Ensure CognitiveCompanion component is added to NPCs
- Verify Response Mode is set in Inspector

### Issue: "AI responses not working"

**Solution**:
- Verify API key is correctly set
- Check you have credits in OpenAI account
- Try Template mode to confirm base system works

### Issue: "NPCs don't remember between sessions"

**Solution**:
- Enable "Persist Memory" in Inspector
- Check PlayerPrefs aren't being cleared
- Verify save system has write permissions

### Issue: "Can't move player in demo"

**Solution**:
- Click inside Game view to focus it
- Ensure Game view is selected, not Scene view
- Check no error messages in Console

### Common Console Messages

| Message | Meaning | Action |
|---------|---------|--------|
| `[AdaptiveNPC] OpenAI API key not configured` | No API key found | Add API key or use Template mode |
| `[CognitiveCompanion] Response generation failed` | API error | Check API key and internet connection |
| `Pattern Recognized: gift (x3)` | Pattern detected successfully | Working as intended! |

---

## Next Steps

1. **Read the Documentation**:
   - [Getting Started Guide](Documentation~/GettingStarted.md)
   - [API Reference](Documentation~/APIReference.md)
   - [Integration Guide](Documentation~/Integration.md)

2. **Try the Examples**:
   - Check `Documentation~/Examples.md` for code samples
   - Modify the demo scene to experiment

3. **Build Your Own**:
   - Add CognitiveCompanion to your NPCs
   - Customize personality and response modes
   - Create unique NPC behaviors

---

## Support

- **Issues**: https://github.com/shashankcm95/AdaptiveNPC/issues
- **Discussions**: https://github.com/shashankcm95/AdaptiveNPC/discussions
- **Documentation**: https://github.com/shashankcm95/AdaptiveNPC/wiki

---

## Quick Reference Card
```csharp
// Basic Setup
using AdaptiveNPC;

// Add to NPC
var companion = npc.AddComponent<CognitiveCompanion>();

// Track player actions
companion.ObserveAction("helped villager", "quest");

// Listen for responses
companion.OnResponse += (response) => {
    ShowDialogue(response);
};

// Check relationship
string summary = companion.GetRelationshipSummary();
```

---

*Thank you for using Adaptive NPC! Your NPCs will never be the same.*
