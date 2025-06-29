using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Required for List

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    // --- References to be registered by other scripts ---
    // This removes the need for slow "Find" calls.
    private GameManager gameManagerRef;
    private Transform playerTransformRef;

    // We make the data class private to encapsulate it.
    // The public methods will handle interaction with it.
    [System.Serializable]
    private class GameSaveData
    {
        public string sceneName;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public int playerScore;
        public float gameTime;

        // CRITICAL FIX: JsonUtility cannot serialize Dictionaries.
        // We use two lists to store keys and values instead.
        public List<string> customDataKeys = new List<string>();
        public List<string> customDataValues = new List<string>();
    }

    private GameSaveData currentSaveData;
    private bool isResuming = false;
    
    // Public property to check if resuming (e.g., for showing a "Loading..." screen)
    public bool IsResuming => isResuming;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameState(); // Load any existing save data on startup
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Subscribe and unsubscribe to the sceneLoaded event
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called automatically when a new scene finishes loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If we are in the process of resuming from a save file...
        if (isResuming && currentSaveData != null)
        {
            // Use a coroutine to wait one frame, ensuring all objects in the new scene are initialized.
            StartCoroutine(RestoreGameStateCoroutine());
        }
    }

    private System.Collections.IEnumerator RestoreGameStateCoroutine()
    {
        // Wait for the end of the frame to ensure all Awake() and Start() methods have been called.
        yield return new WaitForEndOfFrame(); 
        
        RestoreGameState();
        
        // Reset the flag now that we have finished loading.
        isResuming = false; 
        Time.timeScale = 1f; // Ensure the game is unpaused
    }

    #region Public API
    
    // --- Registration Methods ---
    // Called by GameManager in its Start() method.
    public void RegisterGameManager(GameManager gm)
    {
        gameManagerRef = gm;
    }
    // Called by the Player script in its Start() method.
    public void RegisterPlayer(Transform playerTransform)
    {
        playerTransformRef = playerTransform;
    }
    
    public void StartNewGame()
    {
        currentSaveData = null;
        isResuming = false;
        Time.timeScale = 1f;
        ClearSaveData();
    }
    
    public void ResumeFromSave()
    {
        if (HasSaveData())
        {
            isResuming = true;
            // The data is already loaded from Awake(), so we just set the flag
            // and let the scene loading process handle the rest.
        }
    }
    
    public bool HasSaveData()
    {
        // A save exists if we have data loaded and a scene name.
        return currentSaveData != null && !string.IsNullOrEmpty(currentSaveData.sceneName);
    }
    
    public string GetSavedSceneName()
    {
        // Provide a safe default scene name if no save data exists.
        return currentSaveData?.sceneName ?? "GameScene"; 
    }

    // Call this from a "Save Game" button or at checkpoints.
    public void SaveGame()
    {
        CaptureCurrentState();
        SaveStateToFile();
    }

    #endregion

    #region Save/Load Logic

    // Captures the current state of the game from registered objects.
    private void CaptureCurrentState()
    {
        if (currentSaveData == null)
        {
            currentSaveData = new GameSaveData();
        }

        currentSaveData.sceneName = SceneManager.GetActiveScene().name;

        if (playerTransformRef != null)
        {
            currentSaveData.playerPosition = playerTransformRef.position;
            currentSaveData.playerRotation = playerTransformRef.rotation;
        }

        if (gameManagerRef != null)
        {
            currentSaveData.playerScore = gameManagerRef.Score;
            currentSaveData.gameTime = gameManagerRef.GameTime;
        }
        
        // We can add custom data saving logic here if needed
        // e.g., currentSaveData.customDataKeys.Add("LastCheckpoint");
    }
    
    // Restores the game state using the loaded data.
    private void RestoreGameState()
    {
        if (currentSaveData == null) return;
        
        // Note: The player and GameManager must have registered themselves by this point.
        if (playerTransformRef != null)
        {
            playerTransformRef.position = currentSaveData.playerPosition;
            playerTransformRef.rotation = currentSaveData.playerRotation;
        }

        if (gameManagerRef != null)
        {
            gameManagerRef.SetScore(currentSaveData.playerScore);
            gameManagerRef.SetGameTime(currentSaveData.gameTime);
        }
    }

    private void SaveStateToFile()
    {
        if (currentSaveData != null)
        {
            string jsonData = JsonUtility.ToJson(currentSaveData, true); // Use 'true' for pretty print
            PlayerPrefs.SetString("SavedGameData", jsonData);
            PlayerPrefs.Save();
            Debug.Log("Game State Saved!");
        }
    }
    
    private void LoadGameState()
    {
        if (PlayerPrefs.HasKey("SavedGameData"))
        {
            string jsonData = PlayerPrefs.GetString("SavedGameData");
            currentSaveData = JsonUtility.FromJson<GameSaveData>(jsonData);
            Debug.Log("Game State Loaded.");
        }
    }
    
    private void ClearSaveData()
    {
        PlayerPrefs.DeleteKey("SavedGameData");
        Debug.Log("Save Data Cleared.");
    }

    #endregion
}