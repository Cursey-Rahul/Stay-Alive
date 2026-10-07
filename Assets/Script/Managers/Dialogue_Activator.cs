using UnityEngine;

public class Dialogue_Activator : MonoBehaviour
{
    public static Dialogue_Activator Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ActivateDialogue()
    {
        DialoguesManager.instance.StartDialogue();
    }
}
