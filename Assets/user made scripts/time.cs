using UnityEngine;

public class MenuTimeScaleFix : MonoBehaviour
{
    void Awake()
    {
        // Ensure time scale is always normal in menu scenes
        Time.timeScale = 1f;
    }
    
    void Start()
    {
        // Double check time scale is normal
        Time.timeScale = 1f;
    }
}