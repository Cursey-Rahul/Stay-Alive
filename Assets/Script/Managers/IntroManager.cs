using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


[System.Serializable]
public class TimedDialogue
{
    [TextArea(2, 5)]
    public string text;

    public float startTime;
}
public class IntroManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TimedDialogue[] dialogues;
    [SerializeField] private TextMeshProUGUI dialogueText;
    private bool isLoadingGame = false;
    private int currentDialogue = 0;

    private void Start()
    {
        videoPlayer.loopPointReached += IntroFinished;
    }

    private void Update()
    {
        CheckDialogue();
        if (Input.GetKeyDown(KeyCode.F))
        {
            LoadGame();
        }
    }
    private void CheckDialogue()
    {
        if (currentDialogue >= dialogues.Length)
            return;

        if (videoPlayer.time >= dialogues[currentDialogue].startTime)
        {
            dialogueText.text = dialogues[currentDialogue].text;
           

            currentDialogue++;
        }
    }

    private void IntroFinished(VideoPlayer video)
    {
        LoadGame();
    }

    private void LoadGame()
    {
        if (isLoadingGame)
            return;

        isLoadingGame = true;

        SceneManager.LoadScene("Game");
    }
}