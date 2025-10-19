# Code Examples

## Basic Examples

### Simple Follower Companion
```csharp
public class FollowerCompanion : MonoBehaviour
{
    public Transform player;
    public float followDistance = 3f;
    private CognitiveCompanion companion;
    
    void Start()
    {
        companion = GetComponent<CognitiveCompanion>();
        companion.OnResponse += Speak;
    }
    
    void Update()
    {
        // Follow player
        if (Vector3.Distance(transform.position, player.position) > followDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                player.position, 
                Time.deltaTime * 2f
            );
        }
        
        // Observe environment
        CheckSurroundings();
    }
    
    void CheckSurroundings()
    {
        if (Physics.OverlapSphere(transform.position, 5f, LayerMask.GetMask("Enemy")).Length > 0)
        {
            companion.ObserveAction("encountered enemies", "exploration");
        }
    }
    
    void Speak(string text)
    {
        // Show speech bubble
        Debug.Log($"{name}: {text}");
    }
}
```

### Shopkeeper with Memory
```csharp
public class Shopkeeper : MonoBehaviour
{
    private CognitiveCompanion companion;
    private int purchaseCount = 0;
    
    void Start()
    {
        companion = GetComponent<CognitiveCompanion>();
    }
    
    public void OnPurchase(Item item)
    {
        purchaseCount++;
        companion.ObserveAction($"bought {item.name}", "shopping");
        
        // Discount for regular customers
        if (purchaseCount > 5)
        {
            ApplyDiscount(0.1f);
        }
    }
    
    public void OnBrowse()
    {
        companion.ObserveAction("browsing items", "shopping");
    }
    
    public void OnHaggle()
    {
        companion.ObserveAction("tried to haggle", "shopping");
    }
}
```

### Quest Giver with Relationship
```csharp
public class QuestGiver : MonoBehaviour
{
    private CognitiveCompanion companion;
    public Quest[] availableQuests;
    
    void Start()
    {
        companion = GetComponent<CognitiveCompanion>();
    }
    
    public Quest GetQuest()
    {
        string relationship = companion.GetRelationshipSummary();
        
        // Better quests for trusted players
        if (relationship.Contains("generous") || relationship.Contains("helpful"))
        {
            return GetSpecialQuest();
        }
        
        return GetNormalQuest();
    }
    
    public void OnQuestComplete(Quest quest, bool success)
    {
        if (success)
        {
            companion.ObserveAction($"completed {quest.name} successfully", "quest");
        }
        else
        {
            companion.ObserveAction($"failed {quest.name}", "quest");
        }
    }
}
```

## Advanced Examples

### Multi-NPC Conversation System
```csharp
public class NPCConversationManager : MonoBehaviour
{
    public CognitiveCompanion[] npcs;
    
    public void PlayerDidAction(string action)
    {
        // All NPCs observe but react differently
        foreach (var npc in npcs)
        {
            npc.ObserveAction(action, "witnessed");
        }
        
        // NPCs might discuss what they saw
        StartCoroutine(NPCDiscussion());
    }
    
    IEnumerator NPCDiscussion()
    {
        yield return new WaitForSeconds(2f);
        
        // NPCs comment to each other
        Debug.Log("Guard: Did you see what they did?");
        Debug.Log("Merchant: Yes, quite surprising!");
    }
}
```

### Emotional State System
```csharp
public class EmotionalNPC : MonoBehaviour
{
    private CognitiveCompanion companion;
    
    public enum Mood { Happy, Neutral, Sad, Angry }
    private Mood currentMood = Mood.Neutral;
    
    void Start()
    {
        companion = GetComponent<CognitiveCompanion>();
        companion.OnPatternRecognized += UpdateMood;
    }
    
    void UpdateMood(string action, int count)
    {
        if (action == "gift" && count > 3)
            currentMood = Mood.Happy;
        else if (action == "insult" && count > 2)
            currentMood = Mood.Angry;
            
        UpdateAnimations();
    }
    
    void UpdateAnimations()
    {
        // Change NPC animations based on mood
        GetComponent<Animator>().SetInteger("Mood", (int)currentMood);
    }
}
```
