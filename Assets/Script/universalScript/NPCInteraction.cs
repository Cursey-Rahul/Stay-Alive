
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("NPC Settings")]
    [SerializeField] private string characterName = "NPC";
    [SerializeField] private string[] dialogueLines;

    [Header("Dialogue Choices")]
    [SerializeField]
    private string choicePrompt =
        "Will you let me stay? (Y: Accept / N: Reject)";

    [Header("NPC Outcomes")]
    [SerializeField] private UnityEvent onAccept;
    [SerializeField] private UnityEvent onReject;

    private bool playerNearby;
    private bool dialogueOpen;
    private bool choicesAvailable;
    private int currentLine;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!dialogueOpen)
        {
            if (playerNearby && Input.GetKeyDown(KeyCode.E))
                OpenDialogue();

            return;
        }

        if (!choicesAvailable)
        {
            if (Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                NextDialogue();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Y))
            AcceptNPC();
        else if (Input.GetKeyDown(KeyCode.N))
            RejectNPC();
    }

    private void OpenDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
            return;

        dialogueOpen = true;
        choicesAvailable = false;
        currentLine = 0;

        dialoguePanel.SetActive(true);
        npcName.text = characterName;
        dialogueText.text = dialogueLines[currentLine];

        if (UI.instance != null)
            UI.instance.StartNPCDialogue();
    }

    private void NextDialogue()
    {
        currentLine++;

        if (currentLine >= dialogueLines.Length)
        {
            choicesAvailable = true;
            dialogueText.text = choicePrompt;
            return;
        }

        dialogueText.text = dialogueLines[currentLine];
    }

    public void AcceptNPC()
    {
        if (!dialogueOpen || !choicesAvailable)
            return;

        CloseDialogue();

        onAccept.Invoke();

        Debug.Log(characterName + " accepted.");
    }

    public void RejectNPC()
    {
        if (!dialogueOpen || !choicesAvailable)
            return;

        CloseDialogue();

        onReject.Invoke();

        Debug.Log(characterName + " rejected.");
    }

    private void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueOpen = false;
        choicesAvailable = false;

        if (UI.instance != null)
            UI.instance.EndNPCDialogue();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            playerNearby = false;

            // Keep dialogue open if the player is already talking.
            if (!dialogueOpen && dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }
    }
}
