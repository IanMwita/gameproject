using UnityEngine;
using UnityEngine.SceneManagement;

// It's good practice to place your custom interfaces in the same namespace or a parent one.
public interface IInteractable
{
    /// <summary>
    /// Called by the player's interaction system when the player is in range and presses the interact key.
    /// </summary>
    void Interact();
    
    /// <summary>
    /// Called when the player enters the interaction trigger zone. Used to show prompts.
    /// </summary>
    void OnInteractEnter();

    /// <summary>
    /// Called when the player exits the interaction trigger zone. Used to hide prompts.
    /// </summary>
    void OnInteractExit();
}


namespace EJETAGame
{
    /// <summary>
    /// An interactable object that loads the next scene when the player interacts with it.
    /// </summary>
    public class Interactionportal : MonoBehaviour, IInteractable
    {
        [Header("Anger & Bargaining")]
        [Tooltip("The name of the scene to load when interacting. Leave empty to load the next scene in build order.")]
        [SerializeField] private string sceneToLoad = "";
        
        [Tooltip("(Optional) Delay in seconds before loading the scene.")]
        [SerializeField] private float loadDelay = 2f;

        #region Unity Lifecycle
        
        private void Awake()
        {
            // Any initialization if needed
        }

        #endregion

        #region IInteractable Implementation

        public void OnInteractEnter()
        {
            // Let the player know they can interact.
            InteractionPromptUI.instance.SetText(" [E] to Escape"); 
        }

        public void Interact()
        {
            // When the player presses the interact key, load the next scene.
            Debug.Log("Interaction triggered. Loading scene...");
            
            if (loadDelay > 0f)
            {
                // Load scene after delay
                Invoke(nameof(LoadScene), loadDelay);
            }
            else
            {
                // Load scene immediately
                LoadScene();
            }
        }

        public void OnInteractExit()
        {
            // Clear the interaction prompt when player leaves
            InteractionPromptUI.instance.SetText("");
        }

        #endregion

        #region Scene Loading Logic

        private void LoadScene()
        {
            try
            {
                if (!string.IsNullOrEmpty(sceneToLoad))
                {
                    // Load specific scene by name
                    Debug.Log($"Loading scene: {sceneToLoad}");
                    SceneManager.LoadScene(sceneToLoad);
                }
                else
                {
                    // Load next scene in build order
                    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                    int nextSceneIndex = currentSceneIndex + 1;
                    
                    if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                    {
                        Debug.Log($"Loading next scene (index {nextSceneIndex})");
                        SceneManager.LoadScene(nextSceneIndex);
                    }
                    else
                    {
                        Debug.LogWarning("No next scene available in build settings!");
                        // Optionally loop back to first scene or handle end of game
                        // SceneManager.LoadScene(0); // Uncomment to loop back to first scene
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load scene: {e.Message}");
            }
        }

        #endregion
    }
}