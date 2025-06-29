using UnityEngine;
using UnityEngine.UI;

public class ScreenFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Range(0f, 1f)]
    public float flickerIntensity = 0.5f;
    
    [Range(0.01f, 1f)]
    public float flickerSpeed = 0.1f;
    
    [Range(0f, 1f)]
    public float flickerChance = 0.3f;
    
    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color flickerColor = Color.cyan;
    
    [Header("Advanced Settings")]
    public bool randomFlicker = true;
    public bool pulseEffect = false;
    
    [Range(0.1f, 2f)]
    public float pulseSpeed = 1f;

    // Private variables
    private Renderer objectRenderer;
    private Image uiImage;
    private Light screenLight;
    private Material originalMaterial;
    private Material flickerMaterial;
    private Color originalColor;
    private Color originalEmission;
    private float flickerTimer;
    private bool isFlickering;
    private bool isUIElement;

    void Start()
    {
        // Check if this is a UI element
        uiImage = GetComponent<Image>();
        if (uiImage != null)
        {
            isUIElement = true;
            originalColor = uiImage.color;
        }
        else
        {
            // Check for 3D object renderer
            objectRenderer = GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                originalMaterial = objectRenderer.material;
                originalColor = originalMaterial.color;
                
                // Store original emission if it exists
                if (originalMaterial.HasProperty("_EmissionColor"))
                {
                    originalEmission = originalMaterial.GetColor("_EmissionColor");
                }
                
                // Create a copy of the material to avoid modifying the original
                flickerMaterial = new Material(originalMaterial);
                objectRenderer.material = flickerMaterial;
            }
        }
        
        // Check for light component (optional)
        screenLight = GetComponent<Light>();
    }

    void Update()
    {
        HandleFlicker();
    }

    void HandleFlicker()
    {
        flickerTimer += Time.deltaTime;

        bool shouldFlicker = false;

        if (randomFlicker)
        {
            // Random flicker based on chance
            if (flickerTimer >= flickerSpeed)
            {
                shouldFlicker = Random.value < flickerChance;
                flickerTimer = 0f;
            }
        }
        else
        {
            // Regular interval flicker
            shouldFlicker = flickerTimer >= flickerSpeed;
            if (shouldFlicker)
                flickerTimer = 0f;
        }

        // Apply pulse effect if enabled
        float pulseValue = 1f;
        if (pulseEffect)
        {
            pulseValue = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            pulseValue = Mathf.Lerp(0.3f, 1f, pulseValue);
        }

        if (shouldFlicker)
        {
            isFlickering = !isFlickering;
        }

        // Apply the flicker effect
        ApplyFlickerEffect(pulseValue);
    }

    void ApplyFlickerEffect(float pulseValue)
    {
        Color targetColor;
        
        if (isFlickering)
        {
            // Create flicker color with intensity
            targetColor = Color.Lerp(normalColor, flickerColor, flickerIntensity);
            
            // Add some random variation for more realistic glitch
            if (randomFlicker)
            {
                float randomVariation = Random.Range(0.8f, 1.2f);
                targetColor *= randomVariation;
            }
        }
        else
        {
            targetColor = normalColor;
        }
        
        // Apply pulse effect
        targetColor *= pulseValue;
        
        // Apply to UI Image
        if (isUIElement && uiImage != null)
        {
            uiImage.color = targetColor;
        }
        
        // Apply to 3D object
        if (objectRenderer != null && flickerMaterial != null)
        {
            flickerMaterial.color = targetColor;
            
            // Apply emission for glowing effect
            if (flickerMaterial.HasProperty("_EmissionColor"))
            {
                Color emissionColor = targetColor * (isFlickering ? flickerIntensity : 0.1f);
                flickerMaterial.SetColor("_EmissionColor", emissionColor);
            }
        }
        
        // Apply to light if present
        if (screenLight != null)
        {
            screenLight.color = targetColor;
            screenLight.intensity = isFlickering ? 2f * flickerIntensity : 0.5f;
        }
    }

    // Public methods to control the effect
    public void StartFlicker()
    {
        enabled = true;
    }

    public void StopFlicker()
    {
        enabled = false;
        ResetToNormal();
    }

    public void SetFlickerIntensity(float intensity)
    {
        flickerIntensity = Mathf.Clamp01(intensity);
    }

    public void SetFlickerSpeed(float speed)
    {
        flickerSpeed = Mathf.Clamp(speed, 0.01f, 1f);
    }

    void ResetToNormal()
    {
        if (isUIElement && uiImage != null)
        {
            uiImage.color = originalColor;
        }
        
        if (objectRenderer != null && flickerMaterial != null)
        {
            flickerMaterial.color = originalColor;
            if (flickerMaterial.HasProperty("_EmissionColor"))
            {
                flickerMaterial.SetColor("_EmissionColor", originalEmission);
            }
        }
        
        if (screenLight != null)
        {
            screenLight.color = originalColor;
            screenLight.intensity = 1f;
        }
    }

    void OnDestroy()
    {
        // Clean up the created material
        if (flickerMaterial != null && flickerMaterial != originalMaterial)
        {
            DestroyImmediate(flickerMaterial);
        }
    }
}