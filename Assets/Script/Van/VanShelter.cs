using UnityEngine;

public class VanShelter : MonoBehaviour
{
    [SerializeField] private VanManager van;
    [SerializeField] private Transform exitPoint;

    private bool playerNearby;
    private bool playerInside;

    private GameObject player;
    private SpriteRenderer playerSprite;

    private void Update()
    {
        if (playerNearby && !playerInside &&
            Input.GetKeyDown(KeyCode.E))
        {
            EnterVan();
        }
        else if (playerInside &&
                 Input.GetKeyDown(KeyCode.E))
        {
            ExitVan();
        }
        if ((playerNearby || playerInside) &&
    Input.GetKeyDown(KeyCode.X))
        {
            UnloadEveryone();
        }

        van.MoveIfReady(playerInside);
    }
    private int playerSeatIndex = -1;

    private void EnterVan()
    {
        if (player == null || playerSprite == null)
            return;

        if (!van.HasSpace())
        {
            Debug.Log("Van is full!");
            return;
        }

        playerSeatIndex = van.CurrentPeople;

        if (!van.AddPlayerToSeat(playerSprite.sprite))
            return;

        playerInside = true;
        playerNearby = false;

        player.SetActive(false);

        if (MissionManager.instance != null &&
      MissionManager.instance.CanDriveAway)
        {
            van.StartEscape();
        }

        Debug.Log("Player entered the van.");
    }
private void UnloadEveryone()
    {
        Vector3 exitPosition = exitPoint != null
            ? exitPoint.position
            : transform.position + Vector3.right * 2f;

        // Unload NPCs and clear their seat sprites.
        van.UnloadAllPassengers(exitPosition);

        // Clear the player's seat sprite.
        if (playerSeatIndex >= 0)
        {
            van.ClearSeat(playerSeatIndex);
            playerSeatIndex = -1;
        }

        // Restore the player if they were inside.
        if (playerInside && player != null)
        {
            player.transform.position = exitPosition;
            player.SetActive(true);

            playerInside = false;
            playerNearby = true;
        }

        Debug.Log("Everyone has left the van.");
    }

    private void ExitVan()
    {
        if (player == null)
            return;

        playerInside = false;

        van.RemovePerson(playerSeatIndex);
        playerSeatIndex = -1;

        if (exitPoint != null)
            player.transform.position = exitPoint.position;
        else
            player.transform.position =
                transform.position + Vector3.right * 2f;

        player.SetActive(true);

        Debug.Log("Player exited the van.");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") ||
    other.transform.root.CompareTag("Player"))
        {
            player = other.transform.root.gameObject;
            playerSprite =
                player.GetComponentInChildren<SpriteRenderer>();

            playerNearby = true;

            Debug.Log("Player is near the van. Press E to enter.");

            if (MissionManager.instance != null)
            {
                MissionManager.instance.ReachVan();
            }

            return;
        }

        if (other.CompareTag("NPC"))
        {
            NPC npc = other.GetComponent<NPC>();

            if (npc == null || !npc.permitted)
                return;

            GameObject npcObject = other.gameObject;
            SpriteRenderer npcSprite =
                npcObject.GetComponentInChildren<SpriteRenderer>();

            if (npcSprite == null)
                return;

            if (van.TryAddPassenger(
                npcObject, npcObject.name, npcSprite.sprite))
            {
                npcObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            playerNearby = false;
            Debug.Log("Player left the van trigger.");
        }
    }
}
