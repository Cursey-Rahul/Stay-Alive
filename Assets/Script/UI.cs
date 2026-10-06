using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gamePausedUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameControl;
    [SerializeField] private GameObject gameClear;
    [Space]
    [SerializeField] private TextMeshProUGUI timerValue;
    [SerializeField] private TextMeshProUGUI killCountValue;
    [SerializeField] private TextMeshProUGUI soulsValue;
    [SerializeField] private GameObject waveCountText;
    [SerializeField] private GameObject[] miaHearts;
    [SerializeField] private GameObject[] alinaHearts;
    [SerializeField] private GameObject[] gameElements;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gameStartBGM;
    [SerializeField] private AudioClip fightBGM;
    [SerializeField] private AudioClip gameOverBGM;
    public int killCount;
    public int soulsCount;
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
        audioSource.Stop();
        audioSource.clip = gameStartBGM;
        audioSource.Play();
        gameControl.SetActive(true);
    }

    private void Update()
    {
        FixSpeed();
        TogglePause();
        if (Time.timeScale != 1f)
        {
            GamePausedState = true;
        }
        else
        {
            GamePausedState = false;
        }
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


    public void UpdateKillCount()
    {
        killCount++;
        killCountValue.text = killCount.ToString();
    }
    public void UpdateSoulsCount(int souls)
    {
        soulsCount += souls;
        soulsValue.text = soulsCount.ToString();
    }
    private void LateUpdate()
    {
        if (Time.timeScale < 1f && timer >= restartDeadlineTime)
        {
            Time.timeScale = 0f;
        }
    }
    public void GameOver() {
        GameOverState= true;
        restartDeadlineTime = timer + 0.6f;
        audioSource.Stop();
        audioSource.clip= gameOverBGM;
        audioSource.Play();
        gameOverUI.SetActive(true);
        Player player = FindFirstObjectByType<Player>();
        ObjectToProtect mia = FindFirstObjectByType<ObjectToProtect>();

        if (player != null)
            player.EnableMovement(false);

        if (mia != null)
            mia.EnableMovement(false);

        Time.timeScale = 0.2f;
    }
    public void RestartLevel() { 
        GameOverState = false;
        audioSource.Stop();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
        
    }
    public void AlinaHeath(int health) {
        for (int i = 0; i < alinaHearts.Length; i++)
        {
            if (i < health)
            {
                alinaHearts[i].SetActive(true);
            }
            else
            {
                alinaHearts[i].SetActive(false);
            }
        }
    }
    public void MiaHeath(int health) {
        for (int i = 0; i < miaHearts.Length; i++)
        {
            if (i < health)
            {
                miaHearts[i].SetActive(true);
            }
            else
            {
                miaHearts[i].SetActive(false);
            }
        }
    }
    public void GamePause() {
        gamePausedUI.SetActive(true);
        Time.timeScale = 0f;
        audioSource.Stop();
        audioSource.clip= gameStartBGM;
        audioSource.Play();
        
    }
    public void GameResume() {
        gamePausedUI.SetActive(false);
        Time.timeScale = 1f;
        audioSource.Stop();
        audioSource.clip= fightBGM;
        audioSource.Play();
    }
    public void GameStart() {
        GameOverState= false;
        gameUI.SetActive(true);
        audioSource.Stop();
        audioSource.clip= fightBGM;
        audioSource.Play();
    }
    public void ConfirmControls() {
        gameControl.SetActive(false);
        GameManagerScript.instance.StartGame();
        GameStart();
    }
    public void MainMenu() {
        audioSource.Stop();
        SceneManager.LoadScene("Menu");
    }
    public void GameClear() {
        gameClear.SetActive(true);
        Time.timeScale = 0f;
        audioSource.Stop();
        audioSource.clip= gameStartBGM;
        audioSource.Play();
    }
    public void QuitGame() {
        Application.Quit();
    }
}
