# API Reference

## CognitiveCompanion

Main component for adaptive NPCs.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `CompanionName` | string | The NPC's name |
| `IsInitialized` | bool | Whether the component is ready |

### Methods

#### ObserveAction
```csharp
public void ObserveAction(string action, string context = "")
```
Records a player action for the NPC to observe and potentially respond to.

**Parameters:**
- `action`: Description of what the player did
- `context`: Optional context (e.g., "combat", "social")

**Example:**
```csharp
companion.ObserveAction("helped villager", "quest");
```

#### GetRelationshipSummary
```csharp
public string GetRelationshipSummary()
```
Returns a summary of the NPC's relationship with the player.

#### ResetCompanion
```csharp
public void ResetCompanion()
```
Clears all memories and patterns, starting fresh.

#### SetSaveProvider
```csharp
public void SetSaveProvider(ISaveProvider provider)
```
Sets a custom save provider for integration with existing save systems.

### Events

#### OnResponse
```csharp
public event Action<string> OnResponse
```
Fired when the NPC generates a response.

#### OnPatternRecognized
```csharp
public event Action<string, int> OnPatternRecognized
```
Fired when a behavior pattern is recognized (action performed 3+ times).

## Interfaces

### ISaveProvider
Implement to integrate with custom save systems.
```csharp
public interface ISaveProvider
{
    void Save(string key, string data);
    string Load(string key);
    void Delete(string key);
    bool HasKey(string key);
}
```

### IMemorySystem
Core interface for memory management.
```csharp
public interface IMemorySystem
{
    void RecordAction(string action, string context);
    string GetSummary();
    void Clear();
    string Serialize();
    void Deserialize(string data);
}
```

### IPatternRecognizer
Interface for pattern recognition system.
```csharp
public interface IPatternRecognizer
{
    Pattern AnalyzeAction(string action);
    void Clear();
    string Serialize();
    void Deserialize(string data);
}
```

## Configuration

### Response Modes

| Mode | Description | Cost |
|------|-------------|------|
| `Template` | Pre-written responses only | Free |
| `AI` | Always use AI (requires API key) | ~$0.002 per response |
| `Hybrid` | Smart mix of both | ~$0.0006 per response |

### Memory Settings

| Setting | Default | Description |
|---------|---------|-------------|
| `persistMemory` | true | Save memories between sessions |
| `maxMemories` | 50 | Maximum memories per NPC |
| `responseFrequency` | 0.4 | Chance to respond (0-1) |
| `aiUsageThreshold` | 0.7 | When to use AI in hybrid mode |
