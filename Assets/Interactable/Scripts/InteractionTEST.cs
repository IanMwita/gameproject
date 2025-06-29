using UnityEngine;

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
    /// An interactable object that displays a UI Canvas and waits for a Yes (Y) or No (N) key press from the player.
    /// </summary>
    public class InteractionTEST : MonoBehaviour, IInteractable
    {
        [Header("UI References")]
        [Tooltip("The parent Canvas to show/hide. It should be disabled by default.")]
        [SerializeField] private Canvas targetCanvas;
        
        [Tooltip("(Optional) A specific panel within the canvas to toggle. If null, only the canvas is toggled.")]
        [SerializeField] private GameObject questionPanel;

        // A private flag to control the state of the interaction.
        private bool isWaitingForInput = false;

        #region Unity Lifecycle
        
        private void Awake()
        {
            // Ensure the canvas is hidden at the start of the game.
            if (targetCanvas != null)
            {
                targetCanvas.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            // Only listen for input if the interaction has been initiated.
            if (!isWaitingForInput)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                OnYesPressed();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                OnNoPressed();
            }
        }
        
        #endregion

        #region IInteractable Implementation

        public void OnInteractEnter()
        {
            // Let the player know they can interact.
            // The actual key ("E", "F", etc.) should be managed by the player/interaction system,
            // not this individual object.
            InteractionText.instance.SetText("Press [E] to examine"); 
        }

        public void Interact()
        {
            // When the player presses the interact key, show the dialogue.
            ShowCanvas();
            isWaitingForInput = true;
            Debug.Log("Interaction started. Waiting for Y/N input...");
            
            // Optional: You might want to disable player movement here.
            // e.g., PlayerMovement.instance.CanMove = false;
        }

        public void OnInteractExit()
        {
            // If the player walks away, hide the prompt and cancel any pending interaction.
            InteractionText.instance.SetText(""); // Clear the "Press E" prompt.
            HideCanvas(); // Also hide the main canvas if it was active.
            Debug.Log("Player moved out of range. Interaction ended.");
        }

        #endregion

        #region UI and Response Logic

        private void ShowCanvas()
        {
            if (targetCanvas != null)
            {
                targetCanvas.gameObject.SetActive(true);
            }

            if (questionPanel != null)
            {
                questionPanel.SetActive(true);
            }
        }

        private void HideCanvas()
        {
            if (targetCanvas != null)
            {
                targetCanvas.gameObject.SetActive(false);
            }

            // The question panel is a child of the canvas, so it will be hidden automatically,
            // but explicitly deactivating it is fine too.
            if (questionPanel != null)
            {
                questionPanel.SetActive(false);
            }
            
            // Crucially, reset the state.
            isWaitingForInput = false;
        }

        private void OnYesPressed()
        {
            Debug.Log("Player selected YES.");

            // --- ADD YOUR 'YES' LOGIC HERE ---
            // Example: Add item to inventory, start a quest, open a door...

            HideCanvas();
            
            // Optional: Re-enable player movement.
            // e.g., PlayerMovement.instance.CanMove = true;
        }

        private void OnNoPressed()
        {
            Debug.Log("Player selected NO.");

            // --- ADD YOUR 'NO' LOGIC HERE ---
            // Example: Player character says "Maybe later."

            HideCanvas();
            
            // Optional: Re-enable player movement.
            // e.g., PlayerMovement.instance.CanMove = true;
        }

        #endregion
    }
}