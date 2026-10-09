using UnityEngine;

public class Enemy_Respawner : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] respawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float cooldown = 3f;
    [SerializeField] private int maxEnemies = 10;

    private float timer;
    private Transform player;
    private int currentEnemies;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>().transform;
    }

    private void Update()
    {
        if (!GameManagerScript.instance.gameStarted)
            return;

        if (currentEnemies >= maxEnemies)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = cooldown;
            CreateNewEnemy();
        }
    }

    private void CreateNewEnemy()
    {
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);

        int respawnIndex = Random.Range(
            0,
            respawnPoints.Length
        );

        Vector3 spawnPoint =
            respawnPoints[respawnIndex].position;

        GameObject enemy = Instantiate(
            enemyPrefabs[enemyIndex],
            spawnPoint,
            Quaternion.identity
        );

        currentEnemies++;

        bool createdOnRight =
            enemy.transform.position.x >
            player.position.x;

        Enemy enemyScript =
            enemy.GetComponent<Enemy>();

        if (createdOnRight)
        {
            enemyScript.Flip();
        }
    }

    public void EnemyDied()
    {
        currentEnemies--;
        currentEnemies = Mathf.Max(0, currentEnemies);
    }
}