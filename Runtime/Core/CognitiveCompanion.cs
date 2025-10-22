// Fix the ObserveAction method with proper error handling
public async void ObserveAction(string action, string context = "")
{
    try
    {
        // Validation
        if (!IsInitialized)
        {
            Debug.LogWarning($"[{companionName}] Not initialized yet");
            return;
        }
        
        if (string.IsNullOrEmpty(action))
        {
            Debug.LogError($"[{companionName}] Cannot observe null/empty action");
            return;
        }
        
        // Core logic with safety checks
        memory?.RecordAction(action, context);
        
        var pattern = patterns?.AnalyzeAction(action);
        if (pattern != null && pattern.Count >= 3)
        {
            OnPatternRecognized?.Invoke(pattern.Action, pattern.Count);
        }
        
        if (ShouldRespond(action, pattern))
        {
            await GenerateResponse(action, context, pattern);
        }
    }
    catch (Exception e)
    {
        Debug.LogError($"[{companionName}] ObserveAction failed: {e.Message}\n{e.StackTrace}");
    }
}

// Fix the GenerateResponse method
private async Task GenerateResponse(string action, string context, Pattern pattern)
{
    try
    {
        if (responseProvider == null)
        {
            Debug.LogError($"[{companionName}] Response provider not initialized");
            return;
        }
        
        var request = new ResponseRequest
        {
            Action = action,
            Context = context,
            Pattern = pattern,
            Memory = memory?.GetSummary() ?? "First encounter",
            Personality = personality,
            CompanionName = companionName
        };
        
        string response = await responseProvider.GenerateResponse(request);
        
        if (!string.IsNullOrEmpty(response))
        {
            // Ensure we're on the main thread for Unity events
            if (UnityEngine.Threading.Dispatcher.Main.CheckAccess())
            {
                OnResponse?.Invoke(response);
            }
            else
            {
                UnityEngine.Threading.Dispatcher.Main.Post(_ => OnResponse?.Invoke(response), null);
            }
        }
    }
    catch (Exception e)
    {
        Debug.LogError($"[{companionName}] Response generation failed: {e.Message}");
    }
}
