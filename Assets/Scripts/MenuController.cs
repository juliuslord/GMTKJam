using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour     // This controls all menus/UI
{
    public bool inTutorial = false;

    [SerializeField] private GameObject _winMenu;
    [SerializeField] private GameObject _deadMenu;

    private bool won = false;

    [SerializeField] private GameObject _pauseMenu;
    public GameObject _hudMenu;

    public GameObject tutorialTextObject;
    public Text tutorialText;

    public bool IsPaused;

    public float fadeDuration = 1.0f;
    public Image fadeImage;

    private float currentAlpha = 1.0f;
    private float targetAlpha = 0.0f;
    private float fadeTimer = 0.0f;
    private bool isFading = false;
    private bool changingLevel = false;

    public GameObject playerController;

    public List<GameObject> heartPictures;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        tutorialTextObject.SetActive(false);

        fadeImage.gameObject.SetActive(true);

        fadeImage.color = new Color(0.0f, 0.0f, 0.0f, currentAlpha);

        PauseMode(false);

        StartFadeIn();
    }

    void Update()
    {
        OnWin();
        HudControl();
        OnDeath();

        HandleInput();

        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float normalizedTime = fadeTimer / fadeDuration;
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, normalizedTime);
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, currentAlpha);

            if (normalizedTime >= 1.0f)
            {
                isFading = false;
                fadeTimer = 0.0f;
                currentAlpha = targetAlpha;

                if (changingLevel)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);   // Moving to next level
                }
            }
        }

        
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void OnDeath()
    {
        if (playerController != null && !Object.ReferenceEquals(playerController, null))
        {
            _deadMenu.SetActive(false);
        }

        else
        {
            _hudMenu.SetActive(false);
            _deadMenu.SetActive(true);
        }
    }

    public void YouWin()
    {
        won = true;
    }

    private void OnWin()
    {
        if (won == true)                // Retrieve win status from the goal script "Victory"
        {
            _hudMenu.SetActive(false);
            _winMenu.SetActive(true);
            Time.timeScale = 0;
        }

        else
        {
            _hudMenu.SetActive(true);
            //Cursor.visible = false;
            _winMenu.SetActive(false);
            //Time.timeScale = 1;                           // Uncommenting the rest of these breaks the pause menu
            //Cursor.lockState = CursorLockMode.Locked;     // If it breaks again then dont bother with mouse, just use text
            // saying 'Enter to progress' and add the KeyDown input
        }

    }

    public void NextLevel()
    {
        // Debug.Log("Next Level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Next level is the current +1
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            IsPaused = false;
        }
        else
        {
            IsPaused = true;
        }

        PauseMode(IsPaused);
    }

    public void PauseMode(bool paused)
    {
        IsPaused = paused;
        OnPause(paused);
    }

    private void OnPause(bool paused)       // Pausing activates pause menu and stops game time
    {
        if (paused)
        {
            _pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            _pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void DeleteHeart()
    {
        if (heartPictures.Count > 0)
        {
            GameObject obj = heartPictures[0];
            heartPictures.RemoveAt(0);
            Destroy(obj);
        }
    }

    void HudControl()                   // If paused, no HUD
    {
        if (IsPaused)
        {
            _hudMenu.SetActive(false);
        }
        else
        {
            _hudMenu.SetActive(true);
        }
    }
 
    public void StartFadeIn()
    {
        isFading = true;
        targetAlpha = 0.0f;
        fadeImage.color = new Color(0.0f, 0.0f, 0.0f, currentAlpha);
    }

    public void StartFadeOut()
    {
        isFading = true;
        targetAlpha = 1.0f;
        fadeImage.color = new Color(0.0f, 0.0f, 0.0f, currentAlpha);

        changingLevel = true;
    }
    
    public void TurnOffTutorialText()
    {
        tutorialTextObject.SetActive(false);
    }

    public void AlterTutorialText(string newText)
    {
        tutorialText.text = newText;
        tutorialTextObject.SetActive(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;                // On resume time moves again and the other UI elements are set inactive
        _pauseMenu.SetActive(false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //Application.Quit();
        Debug.Log("Changing scene to main menu");
        SceneManager.LoadScene(0);        // On exit send them back to the main menu
#endif
    }
}
