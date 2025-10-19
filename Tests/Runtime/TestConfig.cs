using UnityEngine;

namespace AdaptiveNPC.Tests
{
    public static class TestConfig
    {
        // Set to true to run integration tests with real API
        public static bool RUN_API_TESTS = false;
        
        // Test API key (don't commit real keys!)
        public static string TEST_API_KEY = "test-key-for-ci";
        
        public static void SetupTestEnvironment()
        {
            if (RUN_API_TESTS && !string.IsNullOrEmpty(TEST_API_KEY))
            {
                PlayerPrefs.SetString("AdaptiveNPC_OpenAI_Key", TEST_API_KEY);
            }
        }
        
        public static void CleanupTestEnvironment()
        {
            PlayerPrefs.DeleteKey("AdaptiveNPC_OpenAI_Key");
        }
    }
}
