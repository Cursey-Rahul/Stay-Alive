
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameStartUIManager: MonoBehaviour
{
    [SerializeField] private GameObject gameStartUI;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;


    private void Awake()
    {
        gameStartUI.SetActive(true);
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    public void GameStart() { 
        gameStartUI.SetActive(false);
        audioSource.Stop();
        SceneManager.LoadScene("Game");
    }
    public void QuitGame() { 
        Application.Quit();
        audioSource.Stop();
    }
  

}
