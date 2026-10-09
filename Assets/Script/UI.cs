using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gamePausedUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameControl;
    [SerializeField] private GameObject gameClear;
    [SerializeField] private GameObject inventoryUI;

    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI timerValue;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gameStartBGM;
    [SerializeField] private AudioClip gameOverBGM;

    private float timer;
    private float restartDeadlineTime;

    public float timeSpeed;
    public bool GameOverState = false;
    public bool GamePausedState = false;

    public static UI instance;

    private void Awake()
    {
        instance = this;

        Time.timeScale = 1f;

        gameOverUI.SetActive(false);
        gamePausedUI.SetActive(false);
        inventoryUI.SetActive(false);
    }

    private void Update()
    {
        FixSpeed();
        TogglePause();
        

        GamePausedState = Time.timeScale != 1f;

        if (gameUI.activeSelf)
        {
            timer += Time.deltaTime;
            timerValue.text = timer.ToString("F2") + "s";
        }

       
    }

    public void FixSpeed()
    {
        timeSpeed = Time.timeScale;
    }

    private void TogglePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePausedUI.activeSelf)
            {
                GameResume();
            }
            else
            {
                if (gameClear.activeSelf || gameOverUI.activeSelf)
                {
                    return;
                }

                GamePause();
            }
        }
    }

    public void ToggleInventory()
    {
        if (inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            inventoryUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void GameOver()
    {
        GameOverState = true;

        restartDeadlineTime = timer + 0.6f;

        audioSource.Stop();
        audioSource.clip = gameOverBGM;
        audioSource.Play();

        gameOverUI.SetActive(true);

        Time.timeScale = 0.2f;
    }

    public void RestartLevel()
    {
        GameOverState = false;

        audioSource.Stop();

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    public void GamePause()
    {
        gamePausedUI.SetActive(true);

        Time.timeScale = 0f;

        audioSource.Stop();
        audioSource.clip = gameStartBGM;
        audioSource.Play();
    }

    public void GameResume()
    {
        gamePausedUI.SetActive(false);

        Time.timeScale = 1f;

        audioSource.Stop();
        audioSource.clip = gameStartBGM;
        audioSource.Play();
    }

    public void GameStart()
    {
        GameOverState = false;

        gameUI.SetActive(true);

        audioSource.Stop();
        audioSource.clip = gameStartBGM;
        audioSource.Play();
    }

    public void ConfirmControls()
    {
        gameControl.SetActive(false);

        GameManagerScript.instance.StartGame();

        GameStart();
    }

    public void MainMenu()
    {
        audioSource.Stop();

        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }

    public void GameClear()
    {
        gameClear.SetActive(true);

        Time.timeScale = 0f;

        audioSource.Stop();
        audioSource.clip = gameStartBGM;
        audioSource.Play();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LateUpdate()
    {
        if (Time.timeScale < 1f && timer >= restartDeadlineTime)
        {
            Time.timeScale = 0f;
        }
    }
}