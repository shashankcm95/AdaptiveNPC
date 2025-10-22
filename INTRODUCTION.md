# Adaptive NPC - Complete Developer Manual

**Version:** 1.0.1  
**Last Updated:** October 2025  
**Reading Time:** 10 minutes

---

## Table of Contents

1. [Welcome](#welcome)
2. [What is Adaptive NPC?](#what-is-adaptive-npc)
3. [Installation Walkthrough](#installation-walkthrough)
4. [Your First NPC in 5 Minutes](#your-first-npc-in-5-minutes)
5. [How It Works](#how-it-works)
6. [Testing & Verification](#testing--verification)
7. [Architecture Overview](#architecture-overview)
8. [Common Integration Patterns](#common-integration-patterns)
9. [Performance & Optimization](#performance--optimization)
10. [Troubleshooting Guide](#troubleshooting-guide)

---

## Welcome

Welcome to Adaptive NPC! This manual will take you from installation to advanced implementation. Whether you're creating a simple RPG shopkeeper or a complex companion system, this guide has you covered.

**What you'll learn:**
- How to install and verify the library
- Creating your first intelligent NPC
- Understanding the memory and pattern systems
- Best practices for production games

---

## What is Adaptive NPC?

Adaptive NPC is a Unity library that gives your NPCs:

### 🧠 **Memory**
NPCs remember what players do, building a personality profile over time.
```
Player gives 3 gifts → NPC thinks: "This player is generous"
Player attacks repeatedly → NPC thinks: "This player is aggressive"
```

### 🎯 **Pattern Recognition**
NPCs notice when players repeat behaviors and respond accordingly.
```
First gift: "Thank you!"
Third gift: "You're always so generous!"
Tenth gift: "Are you trying to buy my friendship?"
```

### 💬 **Contextual Responses**
NPCs respond based on their relationship with the player.
```
New player approaches → "Hello stranger"
Friendly player approaches → "Good to see you again, friend!"
Aggressive player approaches → "Please, no trouble..."
```

### 💾 **Persistence**
Memories save automatically between game sessions.
```
Day 1: Player helps NPC
Day 7: Player returns → "You're the one who helped me last week!"
```

---

## Installation Walkthrough

### Prerequisites

- **Unity Version:** 2021.3.0f1 or newer
- **Platforms:** All Unity platforms supported
- **Dependencies:** None (OpenAI optional)

### Method 1: Package Manager (Recommended)

1. **Open Unity Package Manager**
   - Window → Package Manager
   
2. **Add from Git URL**
   - Click the **+** button (top-left)
   - Select "Add package from git URL"
   
3. **Enter Repository URL**
```
   https://github.com/shashankcm95/AdaptiveNPC.git
```
   
4. **Verify Installation**
   - Package should appear as "Adaptive NPC" v1.0.1
   - No compilation errors in Console

### Method 2: Manual Installation

1. **Download Release**
   - Go to [Releases](https://github.com/shashankcm95/AdaptiveNPC/releases)
   - Download `AdaptiveNPC_v1.0.1.unitypackage`
   
2. **Import Package**
   - Assets → Import Package → Custom Package
   - Select downloaded file
   - Click Import

### Verify Installation

Run the built-in test to ensure everything works:

1. **Menu Bar** → AdaptiveNPC → Quick Test
2. Look for "✅ All tests passed!" in Console

If you see this message, you're ready to go!

---

## Your First NPC in 5 Minutes

### Step 1: Create an NPC GameObject
```csharp
// Create in code
GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
npc.name = "Smart NPC";
```

Or manually in Scene:
1. GameObject → 3D Object → Capsule
2. Rename to "Smart NPC"

### Step 2: Add the Component
```csharp
using UnityEngine;
using AdaptiveNPC;  // Don't forget this!

public class MyFirstNPC : MonoBehaviour
{
    void Start()
    {
        // Add the AI component
        var ai = gameObject.AddComponent<CognitiveCompanion>();
        
        // That's it! Your NPC now has memory and pattern recognition
    }
}
```

Or manually:
1. Select your NPC
2. Add Component → Cognitive Companion

### Step 3: Make it Interactive
```csharp
public class MyFirstNPC : MonoBehaviour
{
    private CognitiveCompanion ai;
    
    void Start()
    {
        ai = gameObject.AddComponent<CognitiveCompanion>();
        
        // Listen for responses
        ai.OnResponse += (response) => {
            Debug.Log($"NPC says: {response}");
        };
    }
    
    // Call this when player interacts
    public void OnPlayerInteraction(string action)
    {
        ai.ObserveAction(action, "interaction");
    }
}
```

### Step 4: Test It
```csharp
void Update()
{
    // Test with keyboard
    if (Input.GetKeyDown(KeyCode.G))
    {
        ai.ObserveAction("gave gift", "test");
    }
    
    if (Input.GetKeyDown(KeyCode.T))
    {
        ai.ObserveAction("talked to", "test");
    }
}
```

### Step 5: See Results

Press G three times, and watch the Console:
1. First: "Thank you for the gift!"
2. Second: "Another gift? How kind!"
3. Third: "⚡ Pattern Recognized: gift x3" + "You're always so generous!"

**Congratulations!** You've created your first adaptive NPC!

---

## How It Works

### The Core Loop
```mermaid
Player Action → ObserveAction() → Memory Update → Pattern Check → Generate Response
```

### Memory System

Each NPC maintains memories about the player:
```csharp
// Internal memory structure
{
    "actions": ["gave gift", "talked", "gave gift", "helped"],
    "traits": {
        "generous": 0.7,
        "friendly": 0.5,
        "helpful": 0.3
    },
    "encounters": 4
}
```

### Pattern Recognition

The system detects repeated behaviors:

| Repetitions | Significance | NPC Reaction |
|------------|--------------|--------------|
| 1-2 | Normal | Standard response |
| 3-4 | Pattern emerging | Acknowledgment |
| 5+ | Strong pattern | Special dialogue |

### Response Generation

Three modes available:
```csharp
// In Inspector or code
companion.responseMode = ResponseMode.Template;  // Free, instant
companion.responseMode = ResponseMode.AI;        // Natural, costs API
companion.responseMode = ResponseMode.Hybrid;    // Best of both (default)
```

---

## Testing & Verification

### Automated Test Scene

Create a full test environment:

1. **Menu** → AdaptiveNPC → Create Test Scene
2. Press **Play**
3. Watch automated interactions

### Manual Testing Checklist

- [ ] Component adds without errors
- [ ] ObserveAction processes immediately
- [ ] Patterns trigger at 3 repetitions
- [ ] Memory persists after stopping Play mode
- [ ] Responses appear in Console

### Unit Tests

Run the test suite:

1. Window → General → Test Runner
2. Click "Run All"
3. All tests should pass

---

## Architecture Overview

### Component Structure
```
CognitiveCompanion (Main Component)
├── MemorySystem (Stores interactions)
├── PatternRecognizer (Detects behaviors)
├── ResponseProvider (Generates dialogue)
│   ├── TemplateProvider
│   ├── OpenAIProvider
│   └── HybridProvider
└── SaveProvider (Persistence)
    └── PlayerPrefsSaveProvider
```

### Key Interfaces
```csharp
// Extend functionality by implementing these
public interface IMemorySystem { }
public interface IPatternRecognizer { }
public interface IResponseProvider { }
public interface ISaveProvider { }
```

### Data Flow
```csharp
// 1. Player does something
playerController.Attack(enemy);

// 2. NPC observes
companion.ObserveAction("attacked enemy", "combat");

// 3. Memory updates
memory.RecordAction("attacked enemy", "combat");
// Updates traits: aggressive +0.1

// 4. Pattern check
pattern = patterns.AnalyzeAction("attacked enemy");
// Returns: Pattern { Action: "attack", Count: 3, IsSignificant: true }

// 5. Response generation
if (pattern.IsSignificant)
    response = "You're quite aggressive today!";
```

---

## Common Integration Patterns

### Pattern 1: Dialogue System Integration
```csharp
using PixelCrushers.DialogueSystem;

public class DialogueIntegration : MonoBehaviour
{
    private CognitiveCompanion ai;
    
    void Start()
    {
        ai = GetComponent<CognitiveCompanion>();
        ai.OnResponse += ShowInDialogueUI;
    }
    
    void ShowInDialogueUI(string response)
    {
        DialogueManager.ShowAlert(response);
    }
}
```

### Pattern 2: Quest System
```csharp
public class QuestNPC : MonoBehaviour
{
    private CognitiveCompanion ai;
    
    public Quest GetAppropriateQuest()
    {
        string relationship = ai.GetRelationshipSummary();
        
        if (relationship.Contains("helpful"))
            return hardQuest;  // Trust player with difficult quest
        else
            return easyQuest;   // Start with simple quest
    }
}
```

### Pattern 3: Combat Adaptation
```csharp
public class AdaptiveEnemy : MonoBehaviour
{
    private CognitiveCompanion ai;
    private CombatAI combat;
    
    void Start()
    {
        ai = GetComponent<CognitiveCompanion>();
        ai.OnPatternRecognized += AdaptStrategy;
    }
    
    void AdaptStrategy(string pattern, int count)
    {
        switch(pattern)
        {
            case "dodge":
                combat.UseAOEAttacks();
                break;
            case "block":
                combat.UseUnblockableAttacks();
                break;
            case "ranged":
                combat.CloseDistance();
                break;
        }
    }
}
```

---

## Performance & Optimization

### Memory Usage

| NPCs | Memory | Save Size |
|------|--------|-----------|
| 1 | ~1KB | ~500B |
| 10 | ~10KB | ~5KB |
| 100 | ~100KB | ~50KB |

### CPU Performance

| Operation | Time |
|-----------|------|
| ObserveAction | <0.1ms |
| Pattern Check | <0.05ms |
| Template Response | <0.01ms |
| AI Response | 500-2000ms |

### Optimization Tips

1. **Use Response Frequency**
```csharp
   companion.responseFrequency = 0.3f; // Only respond 30% of the time
```

2. **Limit Memory**
```csharp
   companion.maxMemories = 20; // Reduce for mobile
```

3. **Use Templates for Background NPCs**
```csharp
   // Main NPCs: Hybrid mode
   importantNPC.responseMode = ResponseMode.Hybrid;
   
   // Background NPCs: Template only
   backgroundNPC.responseMode = ResponseMode.Template;
```

---

## Troubleshooting Guide

### Common Issues & Solutions

#### ❌ "Component not found"
```csharp
// Wrong
using CognitiveCompanion;  

// Correct
using AdaptiveNPC;
```

#### ❌ "No responses from NPC"
1. Check Console for errors
2. Verify component is enabled
3. Try Template mode first (doesn't need API key)

#### ❌ "Memory not persisting"
1. Check "Persist Memory" is enabled in Inspector
2. Verify PlayerPrefs has write permission
3. Don't clear PlayerPrefs in your code

#### ❌ "AI responses not working"
```csharp
// Debug API configuration
void CheckAISetup()
{
    string key = PlayerPrefs.GetString("AdaptiveNPC_OpenAI_Key", "");
    Debug.Log($"API Key configured: {!string.IsNullOrEmpty(key)}");
    
    string envKey = System.Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    Debug.Log($"Environment key exists: {!string.IsNullOrEmpty(envKey)}");
}
```

#### ❌ "Pattern not triggering"
```csharp
// Patterns need exact action category match
companion.ObserveAction("gave a gift", "");     // ✓ Triggers "gift" pattern
companion.ObserveAction("gifted item", "");     // ✓ Triggers "gift" pattern  
companion.ObserveAction("presented offering", ""); // ✗ Won't trigger pattern
```

### Debug Mode

Enable verbose logging:
```csharp
public class DebugHelper : MonoBehaviour
{
    private CognitiveCompanion ai;
    
    void Start()
    {
        ai = GetComponent<CognitiveCompanion>();
        
        // Log everything
        ai.OnResponse += (r) => Debug.Log($"[RESPONSE] {r}");
        ai.OnPatternRecognized += (p, c) => Debug.Log($"[PATTERN] {p} x{c}");
        
        // Check memory
        InvokeRepeating(nameof(LogMemory), 5f, 5f);
    }
    
    void LogMemory()
    {
        Debug.Log($"[MEMORY] {ai.GetRelationshipSummary()}");
    }
}
```

---

## Getting Help

### Resources

- **Documentation:** [GitHub Wiki](https://github.com/shashankcm95/AdaptiveNPC/wiki)
- **Issues:** [GitHub Issues](https://github.com/shashankcm95/AdaptiveNPC/issues)
- **Discussions:** [GitHub Discussions](https://github.com/shashankcm95/AdaptiveNPC/discussions)

### Quick Support Checklist

Before reporting an issue:

1. [ ] Using Unity 2021.3.0f1 or newer?
2. [ ] Ran AdaptiveNPC → Quick Test?
3. [ ] Checked Console for errors?
4. [ ] Tried Template mode without AI?
5. [ ] Verified component is on GameObject?

---

## Next Steps

✅ **Basic:** You can now add adaptive NPCs to your game  
✅ **Intermediate:** You understand patterns and memory  
✅ **Advanced:** You can extend with custom providers

**Recommended Learning Path:**

1. Create a simple NPC that remembers the player
2. Add pattern recognition for 3 different actions
3. Integrate with your dialogue system
4. Implement custom save provider
5. Create adaptive combat AI

---

**Thank you for choosing Adaptive NPC!**

*Making NPCs memorable, one interaction at a time.* 🎮

---

**Manual Version:** 1.0.1  
**Last Updated:** October 2025  
**Author:** Shashank CM
