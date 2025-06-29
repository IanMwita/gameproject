using UnityEngine;
using System.Collections; // Essential for Coroutines
using UnityEngine.SceneManagement; // Optional: If you want to load another scene after the prologue

public class PrologueTextAnimator : MonoBehaviour
{
    // --- Assign these in the Unity Inspector ---
    [Tooltip("Reference to your TextMesh component (the older 3D Text component).")]
    public TextMesh prologueTextMesh; // CHANGED: From TextMeshProUGUI to TextMesh!

    [Tooltip("Enter each line of your prologue text here.")]
    [TextArea(5, 10)] // Makes the text array input box taller in the Inspector
    public string[] prologueLines;        

    [Header("--- Animation Settings ---")]
    [Tooltip("How long it takes for each line of text to fade in.")]
    public float fadeInDuration = 0.3f;   

    [Tooltip("How long each line of text stays fully visible on screen.")]
    public float displayDuration = 1.0f;  

    [Tooltip("How long it takes for each line of text to fade out.")]
    public float fadeOutDuration = 0.3f;  

    [Tooltip("Delay after one line fades out before the next line starts to fade in.")]
    public float delayBetweenLines =0.1f; 

    // Optional: Name of the scene to load after the prologue finishes
    [Header("--- Scene Transition ---")]
    [Tooltip("Enter the name of the scene to load after the prologue finishes. Leave blank if not needed.")]
    public string nextSceneName = ""; 

    // This method is called once when the script starts
    private void Start()
    {
        // --- Basic Error Checking ---
        if (prologueTextMesh == null) // CHANGED: Check for TextMesh instead of TextMeshProUGUI
        {
            Debug.LogError("Prologue TextMesh reference is not set! Please assign your TextMesh component to the 'Prologue Text Mesh' field in the Inspector.");
            return; 
        }
        if (prologueLines == null || prologueLines.Length == 0)
        {
            Debug.LogError("Prologue Lines are empty! Please add your prologue sentences to the 'Prologue Lines' array in the Inspector.");
            return; 
        }

        // --- Start the Text Animation Sequence ---
        StartCoroutine(AnimatePrologue());
    }

    // Coroutine to manage the entire prologue sequence
    IEnumerator AnimatePrologue()
    {
        // Initially set the text to fully transparent so it doesn't just pop in
        prologueTextMesh.color = new Color(prologueTextMesh.color.r, prologueTextMesh.color.g, prologueTextMesh.color.b, 0); // CHANGED

        // Loop through each line of text in our array
        foreach (string line in prologueLines)
        {
            prologueTextMesh.text = line; // Set the current line of text // CHANGED

            // --- Fade In ---
            yield return StartCoroutine(FadeText(prologueTextMesh, 0, 1, fadeInDuration)); 

            // --- Display Duration ---
            yield return new WaitForSeconds(displayDuration); 

            // --- Fade Out ---
            yield return StartCoroutine(FadeText(prologueTextMesh, 1, 0, fadeOutDuration)); 

            // --- Delay Before Next Line ---
            yield return new WaitForSeconds(delayBetweenLines); 
        }

        Debug.Log("Prologue animation finished!");

        // --- Transition to Next Scene (Optional) ---
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName); 
        }
    }

    // Generic coroutine for fading a TextMesh component's alpha value
    IEnumerator FadeText(TextMesh textComponent, float startAlpha, float endAlpha, float duration) // CHANGED: From TextMeshProUGUI to TextMesh
    {
        float timer = 0f; 
        Color currentColor = textComponent.color; 

        while (timer < duration)
        {
            timer += Time.deltaTime; 
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            textComponent.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null; 
        }

        textComponent.color = new Color(currentColor.r, currentColor.g, currentColor.b, endAlpha);
    }
}