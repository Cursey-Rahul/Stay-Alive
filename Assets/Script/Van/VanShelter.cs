using UnityEngine;

public class VanShelter : MonoBehaviour
{
    [SerializeField] private VanManager van;

    private bool playerInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered Van trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("PLAYER ENTERED VAN TRIGGER");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Something exited Van trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("PLAYER LEFT VAN TRIGGER");
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            van.AddPerson();
        }
    }
}