using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject levelMenu; // New level menu reference

    void Start()
    {
        ReturnToMainMenu(); // Start in the main menu
    }

    // Called when the Play button is pressed
    public void Play()
    {
        startMenu.SetActive(false);
        levelMenu.SetActive(true);
    }

    public void LoadTutorialScene()
    {
        SceneManager.LoadScene(1);
    }
    
    public void LoadStoryScene()
    {
        SceneManager.LoadScene(2);
    }

    // Called when the Back button is pressed in the Settings or Level menu
    public void ReturnToMainMenu()
    {
        startMenu.SetActive(true);
        levelMenu.SetActive(false); // Disable the level menu
    }

    // Called when the Quit button is pressed
    public void Quit()
    {
        Application.Quit();
    }
}
