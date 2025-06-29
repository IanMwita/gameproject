// GameManager.cs

using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private int score = 0;
    [SerializeField] private float gameTime = 0f;
    
    // Public properties for other scripts to read the values
    public int Score => score;
    public float GameTime => gameTime;
    
    void Start()
    {
        // Register this GameManager with the GameStateManager so it can be saved and loaded.
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.RegisterGameManager(this);
        }
    }

    void Update()
    {
        // THIS IS THE FIX:
        // Instead of checking GameStateManager.Instance.IsGamePaused,
        // we check the new PauseManager's static property.
        if (!PauseManager.IsPaused)
        {
            gameTime += Time.deltaTime;
        }
    }
    
    // --- Public Methods to Modify State ---
    
    public void SetScore(int newScore) => score = newScore;
    public void SetGameTime(float newTime) => gameTime = newTime;
    public void AddScore(int points) => score += points;
}