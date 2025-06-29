using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MenuNavigator : MonoBehaviour
{
    [Header("Navigation Settings")]
    public List<Button> menuButtons = new List<Button>();
    public int startingButtonIndex = 0;
    
    [Header("Controls")]
    public KeyCode selectKey = KeyCode.Return;
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.DownArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    
    private int currentButtonIndex = 0;
    private EventSystem eventSystem;
    
    void Start()
    {
        eventSystem = EventSystem.current;
        
        // Auto-find buttons if list is empty
        if (menuButtons.Count == 0)
        {
            FindAllMenuButtons();
        }
        
        // Set starting selection
        if (menuButtons.Count > 0)
        {
            currentButtonIndex = Mathf.Clamp(startingButtonIndex, 0, menuButtons.Count - 1);
            SelectButton(currentButtonIndex);
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        if (menuButtons.Count == 0) return;
        
        // Navigation
        if (Input.GetKeyDown(upKey) || Input.GetKeyDown(leftKey))
        {
            NavigateUp();
        }
        else if (Input.GetKeyDown(downKey) || Input.GetKeyDown(rightKey))
        {
            NavigateDown();
        }
        
        // Selection
        if (Input.GetKeyDown(selectKey))
        {
            ClickCurrentButton();
        }
        
        // Also handle WASD and controller
        HandleAlternativeInputs();
    }
    
    void HandleAlternativeInputs()
    {
        // WASD keys
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A))
        {
            NavigateUp();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            NavigateDown();
        }
        
        // Space bar for selection
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClickCurrentButton();
        }
        
        // Controller input (if controller is connected)
        float vertical = Input.GetAxis("Vertical");
        if (vertical > 0.5f && !Input.GetKeyDown(upKey))
        {
            NavigateUp();
        }
        else if (vertical < -0.5f && !Input.GetKeyDown(downKey))
        {
            NavigateDown();
        }
        
        // Controller button
        if (Input.GetButtonDown("Submit"))
        {
            ClickCurrentButton();
        }
    }
    
    void NavigateUp()
    {
        if (menuButtons.Count <= 1) return;
        
    
        currentButtonIndex--;
        if (currentButtonIndex < 0)
        {
            currentButtonIndex = menuButtons.Count - 1;
        }
        SelectButton(currentButtonIndex);
    }
    
    void NavigateDown()
    {
        if (menuButtons.Count <= 1) return;
        
       
        currentButtonIndex++;
        if (currentButtonIndex >= menuButtons.Count)
        {
            currentButtonIndex = 0;
        }
        SelectButton(currentButtonIndex);
    }
    
    void SelectButton(int index)
    {
        if (index >= 0 && index < menuButtons.Count)
        {
            Button button = menuButtons[index];
            if (button != null && button.interactable)
            {
                eventSystem.SetSelectedGameObject(button.gameObject);
            }
        }
    }
    
    void ClickCurrentButton()
    {
        if (currentButtonIndex >= 0 && currentButtonIndex < menuButtons.Count)
        {
            Button button = menuButtons[currentButtonIndex];
            if (button != null && button.interactable)
            {
              
                button.onClick.Invoke();
            }
        }
    }
    
    void FindAllMenuButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        menuButtons.Clear();
        
        foreach (Button button in allButtons)
        {
            if (button.interactable)
            {
                menuButtons.Add(button);
            }
        }
        
        // Sort buttons by their Y position (top to bottom)
        menuButtons.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
    }
    
    
    
    }