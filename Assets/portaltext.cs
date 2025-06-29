namespace EJETAGame
{
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;

    // RENAMED!
    public class InteractionPromptUI : MonoBehaviour
    {
        // The instance type must also be renamed.
        public static InteractionPromptUI instance { get; private set; }

        public TextMeshProUGUI textAppear;

        private void Awake()
        {
            // The logic remains the same, but now it references the new class name.
            if(instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

               public void SetText(string text)
        {
            textAppear.SetText(text);
        }

    }
}