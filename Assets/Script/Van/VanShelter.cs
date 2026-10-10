using UnityEngine;

public class VanShelter : MonoBehaviour
{
    [SerializeField] private VanManager van;

    private bool playerInside;
    private bool npcInside;
    private SpriteRenderer playerAndNPCSprite;
    private GameObject character;
    private int index = 0;
    private string IDName;
    private bool permittedToAdd = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC") )
        {
            Debug.Log("SOMEONE CAME NEAR THE VAN");

            npcInside = true;
            playerAndNPCSprite = other.GetComponentInChildren<SpriteRenderer>();
            character = other.gameObject;
            IDName = other.name;
            permittedToAdd = other.GetComponent<NPC>().permitted;
            if (permittedToAdd)
            {
                van.RecordPassengerDetails(character, IDName, playerAndNPCSprite.sprite);
                van.AddPerson(playerAndNPCSprite.sprite);
                character.SetActive(false);
            }
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER CAME NEAR THE VAN");

            playerInside = true;
            playerAndNPCSprite = other.GetComponentInChildren<SpriteRenderer>();
            character = other.gameObject;
            IDName = other.name;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            Debug.Log("SOMEONE LEFT THE VAN");
            playerInside = false;
            playerAndNPCSprite = null;
            character = null;
            IDName = null;

        }

        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("PLAYER LEFT VAN TRIGGER");
            playerAndNPCSprite = null;
            character = null;
            IDName = null;
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            van.RecordPassengerDetails(character, IDName, playerAndNPCSprite.sprite);
            van.AddPerson(playerAndNPCSprite.sprite);
            character.SetActive(false);
           
           
        }
    }
}