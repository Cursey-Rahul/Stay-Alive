using UnityEngine;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager instance;

    [System.Serializable]
    public class Dialogue
    {
        [TextArea(2, 5)]
        public string text;

        public Speaker speaker;
    }

    public enum Speaker
    {
        Alina,
        Mia
    }

    [SerializeField] public Dialogue[] dialogues;

    private int dialogueIndex;
    private bool dialogueActive;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void StartDialogue()
    {
        dialogueIndex = 0;
        dialogueActive = true;

        ShowDialogue();
    }

    private void Update()
    {
        if (dialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            dialogueIndex++;

            if (dialogueIndex >= dialogues.Length)
            {
                EndDialogue();
            }
            else
            {
                ShowDialogue();
            }
        }
    }

    private void ShowDialogue()
    {
        Debug.Log(dialogues[dialogueIndex].speaker + ": " + dialogues[dialogueIndex].text);
    }

    private void EndDialogue()
    {
        dialogueActive = false;

        Debug.Log("Dialogue Ended");
    }
}