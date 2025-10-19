# Integration Guide

## Integrating with Existing Projects

### Basic Integration

1. Import the package
2. Add to existing NPCs:
```csharp
// Your existing NPC class
public class MyNPC : MonoBehaviour
{
    private CognitiveCompanion companion;
    
    void Start()
    {
        // Add cognitive abilities
        companion = gameObject.AddComponent<CognitiveCompanion>();
        companion.OnResponse += HandleResponse;
    }
    
    void HandleResponse(string response)
    {
        // Integrate with your dialogue system
        myDialogueSystem.ShowText(response);
    }
}
```

### Custom Save System Integration

Implement ISaveProvider for your save system:
```csharp
public class MySaveProvider : ISaveProvider
{
    public void Save(string key, string data)
    {
        // Your save logic
        MyGameSaveManager.SaveString(key, data);
    }
    
    public string Load(string key)
    {
        return MyGameSaveManager.LoadString(key);
    }
    
    public void Delete(string key)
    {
        MyGameSaveManager.Delete(key);
    }
    
    public bool HasKey(string key)
    {
        return MyGameSaveManager.HasKey(key);
    }
}

// Set it up
companion.SetSaveProvider(new MySaveProvider());
```

### Dialogue System Integration

#### With Dialogue System for Unity
```csharp
using PixelCrushers.DialogueSystem;

public class DialogueSystemIntegration : MonoBehaviour
{
    private CognitiveCompanion companion;
    
    void OnConversationLine(Subtitle subtitle)
    {
        // Track player choices
        if (subtitle.speakerInfo.isPlayer)
        {
            companion.ObserveAction(subtitle.formattedText.text, "dialogue");
        }
    }
    
    public void ShowCompanionResponse(string response)
    {
        DialogueManager.ShowAlert(response);
    }
}
```

#### With Yarn Spinner
```csharp
public class YarnIntegration : MonoBehaviour
{
    [YarnCommand("observe")]
    public void ObserveAction(string action)
    {
        companion.ObserveAction(action, "yarn_dialogue");
    }
}
```

### Combat System Integration
```csharp
public class CombatIntegration : MonoBehaviour
{
    private CognitiveCompanion companion;
    
    void OnEnemyDefeated(Enemy enemy)
    {
        companion.ObserveAction($"defeated {enemy.type}", "combat");
    }
    
    void OnPlayerDamaged(float damage)
    {
        if (damage > 50)
            companion.ObserveAction("took heavy damage", "combat");
    }
}
```

### Quest System Integration
```csharp
public class QuestIntegration : MonoBehaviour
{
    void OnQuestCompleted(Quest quest)
    {
        companion.ObserveAction($"completed quest: {quest.name}", "quest");
        
        // NPCs remember quest outcomes
        if (quest.helpedNPC)
            companion.ObserveAction("helped someone", "moral_choice");
    }
}
```

## Performance Considerations

### For Mobile
- Use Template mode to avoid API calls
- Reduce maxMemories to 20-30
- Lower responseFrequency to 0.2-0.3

### For Multiple NPCs
- Share save providers between NPCs
- Use object pooling for UI elements
- Consider LOD system for distant NPCs

### API Cost Management
- Hybrid mode reduces costs by ~70%
- Cache responses for common actions
- Set monthly budget limits with OpenAI
