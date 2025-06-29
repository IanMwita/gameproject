using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("Settings")]
    public string mainMenuSceneName = "menu";

    [Header("UI Text")]
    public string normalText = "P to Pause";
    public string pausedText = "Paused - Press P to go to Menu";

    private TextMeshProUGUI textMesh;

    // --- THIS IS THE FIX ---
    // The 'private bool isPaused' has been changed to a 'public static' property.
    // 'public' so other scripts (like GameManager) can read it.
    // 'static' so it can be accessed with PauseManager.IsPaused.
    // '{ get; private set; }' means other scripts can GET the value, but only this script can SET it.
    public static bool IsPaused { get; private set; }


    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        
        // Use the new property to reset the state
        IsPaused = false;
        
        if (textMesh != null)
        {
            textMesh.text = normalText;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            OnPausePressed();
        }
    }

    void OnPausePressed()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }

        // Use the new property here
        if (IsPaused)
        {
            GoToMainMenu();
        }
        else
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        // Use the new property here
        IsPaused = true;
        
        // This script now controls the time scale directly, as you intended.
        Time.timeScale = 0f;

        // The new GameStateManager uses SaveGame() for the autosave-on-pause feature.
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SaveGame();
        }

        if (textMesh != null)
        {
            textMesh.text = pausedText;
        }
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    public void ResumeGame()
    {
        // Use the new property here
        IsPaused = false;
        
        // This script is responsible for time scale
        Time.timeScale = 1f;
        
        if (textMesh != null)
        {
            textMesh.text = normalText;
        }
    }
    
    public void ResetToNormalState()
    {
        Time.timeScale = 1f;
        
        // Use the new property here
        IsPaused = false;
        
        if (textMesh != null)
        {
            textMesh.text = normalText;
        }
    }
}