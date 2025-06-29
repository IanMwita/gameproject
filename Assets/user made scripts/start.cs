using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class startButton : MonoBehaviour
{
    private Button playButton;
    
    void Start()
    {
        // Get the Button component attached to this GameObject
        playButton = GetComponent<Button>();
        
        // Add listener to the button click event
        if (playButton != null)
        {
            playButton.onClick.AddListener(LoadPrologueScene);
        }
        else
        {
            Debug.LogError("No Button component found on " + gameObject.name);
        }
    }
    
    // Method called when the play button is clicked
    void LoadPrologueScene()
    {
        // Load the prologue scene
        SceneManager.LoadScene("PROLOGUE SCENE");
    }
    
    // Alternative method if you want to call it directly from the inspector
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("PROLOGUE SCENE");
    }
}