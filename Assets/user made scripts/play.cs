using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "GameScene";
    
    private Button button;
    private Text buttonText;
    
    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnPlayButtonClick);
        }
        
        buttonText = GetComponentInChildren<Text>();
        UpdateButtonText();
    }
    
    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            bool hasSave = GameStateManager.Instance != null && GameStateManager.Instance.HasSaveData();
            buttonText.text = hasSave ? "Resume" : "Play";
        }
    }
    
    void OnPlayButtonClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
        
        if (GameStateManager.Instance != null)
        {
            if (GameStateManager.Instance.HasSaveData())
            {
                GameStateManager.Instance.ResumeFromSave();
                string sceneName = GameStateManager.Instance.GetSavedSceneName();
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                GameStateManager.Instance.StartNewGame();
                SceneManager.LoadScene(gameSceneName);
            }
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}