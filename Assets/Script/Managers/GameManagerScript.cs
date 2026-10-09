using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    [Header("Game State")]
    public bool gameStarted = false;
    public bool gameOver = false;

    [Header("Game UI")]
    [SerializeField] private UI ui;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        if (gameStarted)
            return;

        gameStarted = true;

        ui.GameStart();
    }

    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;

        ui.GameOver();
    }
}