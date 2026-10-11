using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Background Music")]
    [SerializeField] private AudioClip gameStartBGM;
    [SerializeField] private AudioClip gameOverBGM;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip gamePauseSFX;
    [SerializeField] private AudioClip gameResumeSFX;
    [SerializeField] private AudioClip gameFailSFX;
    [SerializeField] private AudioClip gameClearSFX;
    [SerializeField] private AudioClip interactSound;

    private float timer;
    private Coroutine sfxCoroutine;

    public float timeSpeed;
    public bool GameOverState;
    public bool GamePausedState;

    public static UI instance;

    private void Awake()
    {
        instance = this;

   
        Time.timeScale = 0f;
        GamePausedState = true;
        GameOverState = false;

        if (gameUI != null) gameUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gamePausedUI != null) gamePausedUI.SetActive(false);
        if (gameControl != null) gameControl.SetActive(true);
        if (gameClear != null) gameClear.SetActive(false);
        if (inventoryUI != null) inventoryUI.SetActive(false);

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }
    }


    private void Start()
    {
        PlayBGM(gameStartBGM);
    }

    private void Update()
    {
        FixSpeed();

        TogglePause();
        ToggleGameTutorial();

        GamePausedState = Time.timeScale == 0f;

        if (gameUI != null && gameUI.activeSelf &&
            Time.timeScale > 0f)
        {
            timer += Time.deltaTime;

            if (timerValue != null)
                timerValue.text = timer.ToString("F2") + "s";
        }

        UpdateGameUIVisibility();
    }

    public void StartNPCDialogue()
    {
        if (GameOverState)
            return;

        Time.timeScale = 0f;
        GamePausedState = true;

        StopAudio();
        PlaySFX(interactSound);
    }

    public void EndNPCDialogue()
    {
        Time.timeScale = 1f;
        GamePausedState = false;

        StopAudio();
        PlaySFX(gameResumeSFX);
    }


    private void PlayBGM(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        if (audioSource.isPlaying &&
            audioSource.clip == clip &&
            audioSource.loop)
            return;

        if (sfxCoroutine != null)
        {
            StopCoroutine(sfxCoroutine);
            sfxCoroutine = null;
        }

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        if (sfxCoroutine != null)
        {
            StopCoroutine(sfxCoroutine);
            sfxCoroutine = null;
        }

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();

        sfxCoroutine = StartCoroutine(ResumeBGMAfterSFX(clip));
    }
    private IEnumerator ResumeBGMAfterSFX(AudioClip clip)
    {
        yield return null;

        while (audioSource != null &&
               audioSource.isPlaying &&
               audioSource.clip == clip)
        {
            yield return null;
        }

        if (audioSource == null || audioSource.clip != clip)
        {
            sfxCoroutine = null;
            yield break;
        }

        sfxCoroutine = null;

        if (GameOverState)
        {
            PlayBGM(gameOverBGM);
        }
        else if (Time.timeScale > 0f)
        {
            PlayBGM(gameStartBGM);
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    private void StopAudio()
    {
        if (sfxCoroutine != null)
        {
            StopCoroutine(sfxCoroutine);
            sfxCoroutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }
    }

    private void UpdateGameUIVisibility()
    {
        if (gameUI == null)
            return;

        bool showGameUI =
            (gamePausedUI == null || !gamePausedUI.activeSelf) &&
            (gameControl == null || !gameControl.activeSelf) &&
            (inventoryUI == null || !inventoryUI.activeSelf) &&
            (gameOverUI == null || !gameOverUI.activeSelf) &&
            (gameClear == null || !gameClear.activeSelf) &&
            !GameOverState;

        gameUI.SetActive(showGameUI);
    }

    public void FixSpeed()
    {
        timeSpeed = Time.timeScale;
    }

    private bool IsGameplayUIActive()
    {
        return gameUI != null && gameUI.activeSelf
            && (gamePausedUI == null || !gamePausedUI.activeSelf)
            && (gameControl == null || !gameControl.activeSelf)
            && (inventoryUI == null || !inventoryUI.activeSelf)
            && (gameOverUI == null || !gameOverUI.activeSelf)
            && (gameClear == null || !gameClear.activeSelf)
            && !GameOverState;
    }

    private void TogglePause()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (gamePausedUI != null && gamePausedUI.activeSelf)
        {
            GameResume();
            return;
        }

        if (IsGameplayUIActive())
            GamePause();
    }

    private void ToggleGameTutorial()
    {
        if (!Input.GetKeyDown(KeyCode.Tab))
            return;

        if (GameOverState)
            return;

       
        if (gamePausedUI != null && gamePausedUI.activeSelf)
            return;

        if (inventoryUI != null && inventoryUI.activeSelf)
            return;

        if (gameClear != null && gameClear.activeSelf)
            return;

        if (gameControl == null)
            return;

        bool openingTutorial = !gameControl.activeSelf;

        gameControl.SetActive(openingTutorial);

        Time.timeScale = openingTutorial ? 0f : 1f;
        GamePausedState = openingTutorial;

        if (openingTutorial)
        {
            StopAudio();
            PlaySFX(gamePauseSFX);
        }
        else
        {
            
            PlayBGM(gameStartBGM);
        }
    }

    public void ToggleInventory()
    {
        if (inventoryUI == null || GameOverState)
            return;

        if (inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(false);
            Time.timeScale = 1f;

            
            PlayBGM(gameStartBGM);
            return;
        }

        if (!IsGameplayUIActive())
            return;

        inventoryUI.SetActive(true);
        Time.timeScale = 0f;

        StopAudio();
        PlaySFX(gamePauseSFX);
    }
    public void GamePause()
    {
        if (!IsGameplayUIActive() || gamePausedUI == null)
            return;

        gamePausedUI.SetActive(true);
        GamePausedState = true;
        Time.timeScale = 0f;

        StopAudio();
        PlaySFX(gamePauseSFX);
    }

    public void GameResume()
    {
        if (gamePausedUI == null || !gamePausedUI.activeSelf)
            return;

        gamePausedUI.SetActive(false);
        GamePausedState = false;
        Time.timeScale = 1f;

       
        PlayBGM(gameStartBGM);
    }

    public void GameStart()
    {
        GameOverState = false;
        GamePausedState = false;

        if (gameUI != null) gameUI.SetActive(true);
        if (gamePausedUI != null) gamePausedUI.SetActive(false);
        if (gameControl != null) gameControl.SetActive(false);
        if (inventoryUI != null) inventoryUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameClear != null) gameClear.SetActive(false);

        Time.timeScale = 1f;
        PlayBGM(gameStartBGM);
    }

    public void ConfirmControls()
    {
        if (gameControl != null)
            gameControl.SetActive(false);

        Time.timeScale = 1f;

        GameManagerScript.instance.StartGame();
        GameStart();
    }

    public void GameOver()
    {
        if (GameOverState)
            return;

        GameOverState = true;
        GamePausedState = true;

        if (gameUI != null) gameUI.SetActive(false);
        if (gamePausedUI != null) gamePausedUI.SetActive(false);
        if (gameControl != null) gameControl.SetActive(false);
        if (inventoryUI != null) inventoryUI.SetActive(false);
        if (gameClear != null) gameClear.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(true);

        Time.timeScale = 0f;

        PlaySFX(gameFailSFX);
    }

    public void GameClear()
    {
        if (gameClear != null && gameClear.activeSelf)
            return;

        GamePausedState = true;

        if (gameUI != null) gameUI.SetActive(false);
        if (gamePausedUI != null) gamePausedUI.SetActive(false);
        if (gameControl != null) gameControl.SetActive(false);
        if (inventoryUI != null) inventoryUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameClear != null) gameClear.SetActive(true);

        Time.timeScale = 0f;

        PlaySFX(gameClearSFX);
    }


    public void RestartLevel()
    {
        

        Time.timeScale = 1f;
        GameOverState = false;
        GamePausedState = false;

        StopAudio();

        Scene currentScene = SceneManager.GetActiveScene();


        if (currentScene.buildIndex < 0)
        {
            Debug.LogError(
                "Cannot restart: this scene is not in the active Build Profile."
            );
            return;
        }

        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void MainMenu()
    {
        StopAudio();

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        StopAudio();

        Time.timeScale = 1f;
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (sfxCoroutine != null)
        {
            StopCoroutine(sfxCoroutine);
            sfxCoroutine = null;
        }

        if (instance == this)
            instance = null;
    }
}