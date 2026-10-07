using System.Collections;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;


    public bool canSpawnEnemy = false;


    public int totalEnemies;
    public int enemiesLeftToKill = 0;

    public int waveCount = 0;

    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject waveChange;
    [SerializeField] private Enemy_Respawner enemyRespawner;
    [SerializeField] private GameObject Alina;
    [SerializeField] private GameObject Mia;
    [SerializeField] private UI ui;

    public bool waveFinished = false;
    public bool gameOver = false;
    public bool gameStarted = false;
    public int DifficultyLevel;
    private Entity alina;
    private Entity mia;



    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DifficultyLevel = GameData.Instance.difficultyLevel;
        
    }
    public void Update()
    {
        if (!gameStarted) {  return; }
        HealAlina();
        HealMia();
    }
    public void StartGame()
    {
        if (gameStarted)
            return;

        gameStarted = true;
        Alina.SetActive(true);
        Mia.SetActive(true);
        alina = Alina.GetComponent<Entity>();
        mia = Mia.GetComponent<Entity>();
        WaveChange();
        enemyRespawner.gameObject.SetActive(true);
    }

    public void WaveChange()
    {
        waveFinished = false;

        waveCount++;

        if (waveCount == 5)
        {
            UI.instance.GameClear();
            return;
        }
        else if (waveCount == 4)
        {
            UI.instance.UpdateSoulsCount(5);
            totalEnemies = 1;
        }
        else
        {
            totalEnemies = waveCount * DifficultyLevel;
        }

        enemiesLeftToKill = totalEnemies;

        enemyRespawner.StartWave();

        StartCoroutine(ShowWaveText());
    }

    public void EnemyDied()
    {
        enemiesLeftToKill--;

        Debug.Log("Enemies left: " + enemiesLeftToKill);

        if (enemiesLeftToKill <= 0 && !waveFinished)
        {
            waveFinished = true;
            WaveChange();
        }
    }


    private IEnumerator ShowWaveText()
    {

        canSpawnEnemy = false;
        waveChange.SetActive(true);

        waveChange.GetComponent<TextMeshProUGUI>().text =
            "THE DEAD ARE COMING\nWAVE " + waveCount;

        yield return new WaitForSeconds(3f);

        waveChange.SetActive(false);

        canSpawnEnemy = true;
    }

    private void HealAlina()
    {
        if (Input.GetKeyDown(KeyCode.E) && UI.instance.soulsCount > 0 && alina.currentHealth < alina.maxHealth && !UI.instance.GameOverState)
        {
            UI.instance.UpdateSoulsCount(-1);
            alina.Heal(1);
        }
    }
    private void HealMia()
    {
        if (Input.GetKeyDown(KeyCode.Q) && (UI.instance.soulsCount > 0 || alina.currentHealth > 0) && mia.currentHealth < mia.maxHealth && !UI.instance.GameOverState)
        {
            if (alina.currentHealth >= 2 && UI.instance.soulsCount == 0)
            {
                alina.Heal(-1);
                mia.Heal(1);
            }
            else
            {
                UI.instance.UpdateSoulsCount(-1);
                mia.Heal(1);
            }


        }
    }
}