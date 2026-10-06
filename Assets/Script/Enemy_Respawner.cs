
using UnityEngine;

public class Enemy_Respawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] respawnPoints;

    private int index;

    [SerializeField] private float cooldown;
    [SerializeField] private float timer;
    [SerializeField] private float cooldownDecreaserate = .05f;
    [SerializeField] private float cooldownCap = 0.5f;

    public int enemiesLeftToSpawn;

    private Transform player;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>().transform;
    }

    private void Update()
    {
        if (!GameManagerScript.instance.canSpawnEnemy)
            return;

        timer -= Time.deltaTime;

        if (timer < 0 && enemiesLeftToSpawn > 0)
        {
            timer = cooldown;

            CreateNewEnemy();

            cooldown = Mathf.Max(
                cooldownCap,
                cooldown - cooldownDecreaserate
            );
        }
    }

    public void StartWave()
    {
        enemiesLeftToSpawn =
            GameManagerScript.instance.totalEnemies;

        timer = 0;
    }

    private void CreateNewEnemy()
    {
        enemiesLeftToSpawn--;

        if (enemiesLeftToSpawn <= 0)
        {
            GameManagerScript.instance.canSpawnEnemy = false;
        }

        int wave = GameManagerScript.instance.waveCount;

        if (wave == 4)
        {
            index = 2;
        }
        else if (wave >= 2)
        {
            int randomChance = Random.Range(0, 100);

            if (randomChance < 50)
            {
                index = Random.Range(
                    1,
                    enemyPrefabs.Length - 1
                );
            }
            else
            {
                index = 0;
            }
        }
        else
        {
            index = 0;
        }

        int respawnRandomIndex =
            Random.Range(0, respawnPoints.Length);

        Vector3 spawnpoint =
            respawnPoints[respawnRandomIndex].position;

        GameObject enemy =
            Instantiate(
                enemyPrefabs[index],
                spawnpoint,
                Quaternion.identity
            );

        bool createdOnRight =
            enemy.transform.position.x >
            player.transform.position.x;

        if (createdOnRight)
            enemy.GetComponent<Enemy>().Flip();
    }
}