using UnityEngine;

public class VanShelter : MonoBehaviour
{
    [SerializeField] private VanManager van;

    private bool playerInside;
    private bool npcInside;
    private SpriteRenderer playerAndNPCSprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC") )
        {
            Debug.Log("SOMEONE CAME NEAR THE VAN");

            npcInside = true;
            playerAndNPCSprite = other.GetComponentInChildren<SpriteRenderer>();
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER CAME NEAR THE VAN");

            playerInside = true;
            playerAndNPCSprite = other.GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            Debug.Log("SOMEONE LEFT THE VAN");
            playerInside = false;
            playerAndNPCSprite = null;
        }

        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("PLAYER LEFT VAN TRIGGER");
            playerAndNPCSprite = null;
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            van.AddPerson(playerAndNPCSprite.sprite);
        }

        if (npcInside && Input.GetKeyDown(KeyCode.P))
        {
            van.AddPerson(playerAndNPCSprite.sprite);
        }
    }
}