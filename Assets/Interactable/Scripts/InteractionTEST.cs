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
    /// An interactable object that waits for a Yes (Y) or No (N) key press from the player
    /// and changes the material on a monitor screen when Y is pressed.
    /// </summary>
    public class InteractionTEST : MonoBehaviour, IInteractable
    {
        [Header("Monitor References")]
        [Tooltip("The renderer component of the monitor screen (usually a plane or quad).")]
        [SerializeField] private Renderer monitorRenderer;
        
        [Tooltip("The material to apply to the monitor when Y is pressed.")]
        [SerializeField] private Material newMaterial;
        
        [Tooltip("(Optional) The original material to revert to. Leave null to keep the current material as original.")]
        [SerializeField] private Material originalMaterial;

        // A private flag to control the state of the interaction.
        private bool isWaitingForInput = false;

        #region Unity Lifecycle
        
        private void Awake()
        {
            // Store the original material if not explicitly set
            if (monitorRenderer != null && originalMaterial == null)
            {
                originalMaterial = monitorRenderer.material;
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
            InteractionText.instance.SetText(" [E] to Examine"); 
        }

        public void Interact()
        {
            // When the player presses the interact key, start waiting for Y/N input.
            isWaitingForInput = true;
            Debug.Log("Interaction started. Press Y to change monitor material or N to cancel...");
            
            // Optional: You might want to disable player movement here.
            // e.g., PlayerMovement.instance.CanMove = false;
        }

        public void OnInteractExit()
        {
            // If the player walks away, cancel any pending interaction.
            InteractionText.instance.SetText(""); // Clear the "Press E" prompt.
            isWaitingForInput = false;
            Debug.Log("Player moved out of range. Interaction ended.");
        }

        #endregion

        #region Material Change Logic

        private void OnYesPressed()
        {
            Debug.Log("Player selected YES. Changing monitor material...");

            // Change the monitor material
            if (monitorRenderer != null && newMaterial != null)
            {
                monitorRenderer.material = newMaterial;
                Debug.Log("Monitor material changed successfully.");
            }
            else
            {
                Debug.LogWarning("Monitor renderer or new material is not assigned!");
            }

            // Reset the interaction state
            isWaitingForInput = false;
            
            // Optional: Re-enable player movement.
            // e.g., PlayerMovement.instance.CanMove = true;
        }

        private void OnNoPressed()
        {
            Debug.Log("Player selected NO. Keeping original monitor material.");

            // Reset the interaction state without changing anything
            isWaitingForInput = false;
            
            // Optional: Re-enable player movement.
            // e.g., PlayerMovement.instance.CanMove = true;
        }

        #endregion

        #region Public Utility Methods

        /// <summary>
        /// Manually revert the monitor to its original material.
        /// </summary>
        public void RevertToOriginalMaterial()
        {
            if (monitorRenderer != null && originalMaterial != null)
            {
                monitorRenderer.material = originalMaterial;
                Debug.Log("Monitor reverted to original material.");
            }
        }

        /// <summary>
        /// Check if the monitor currently has the new material applied.
        /// </summary>
        public bool HasNewMaterial()
        {
            return monitorRenderer != null && newMaterial != null && 
                   monitorRenderer.material == newMaterial;
        }

        #endregion
    }
}