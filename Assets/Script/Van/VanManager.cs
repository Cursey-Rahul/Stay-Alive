using UnityEngine;

public class VanManager : MonoBehaviour
{
    [Header("Shelter")]
    [SerializeField] private int maxCapacity = 6;
    [SerializeField] private GameObject[] seats;

    [Header("Required Parts")]
    [SerializeField] private bool hasGasoline;
    [SerializeField] private bool hasSparkPlug;

    [Header("Van Driving")]
    [SerializeField] private Animator vanAnimator;
    [SerializeField] private float moveSpeed = 3f;

    private int currentPeople;
    private SpriteRenderer[] seatRenderers;
    private Passenger[] passengers;

    public int CurrentPeople => currentPeople;
    public int MaxCapacity => maxCapacity;

    private class Passenger
    {
        public GameObject obj;
        public string IDName;
        public Sprite sprite;
    }
    [SerializeField] private Transform vanTransform;
    [SerializeField] private float escapeDistance = 10f;

    private Vector3 escapeStartPosition;
    private bool escapeStarted;

    public void StartEscape()
    {
        if (MissionManager.instance == null ||
     !MissionManager.instance.CanDriveAway)
        {
            
            return;
        }

        if (!MissionManager.instance.HasEnoughSurvivors(SurvivorCount))
        {
            
            return;
        }

        if (escapeStarted) return;

        escapeStartPosition = vanTransform.position;
        escapeStarted = true;

        vanAnimator.SetBool("move", true);

        
    }
    public int SurvivorCount
    {
        get
        {
            int count = 0;

            foreach (Passenger passenger in passengers)
            {
                if (passenger != null && passenger.obj != null)
                    count++;
            }

            return count;
        }
    }
    private void Update()
    {
        if (!escapeStarted) return;

        vanTransform.position +=
            Vector3.left * moveSpeed * Time.deltaTime;

        float distanceMoved =
            escapeStartPosition.x - vanTransform.position.x;

        if (distanceMoved >= escapeDistance)
        {
            escapeStarted = false;

            vanAnimator.SetBool("move", false);

            MissionManager.instance.CompleteEscape();

          
        }
    }

    private void Awake()
    {
        passengers = new Passenger[maxCapacity];
    }

    private void Start()
    {
        seatRenderers = new SpriteRenderer[seats.Length];

        for (int i = 0; i < seats.Length; i++)
        {
            seatRenderers[i] =
                seats[i].GetComponent<SpriteRenderer>();
        }
    }

    public bool AddPlayerToSeat(Sprite playerSprite)
    {
        if (playerSprite == null)
            return false;

        if (!HasSpace() || currentPeople >= seatRenderers.Length)
        {
            
            return false;
        }

        seatRenderers[currentPeople].sprite = playerSprite;
        currentPeople++;

        

        return true;
    }
    public void UnloadAllPassengers(Vector3 exitPosition)
    {
        for (int i = 0; i < currentPeople; i++)
        {
            if (passengers[i] != null)
            {
                GameObject passenger = passengers[i].obj;

                if (passenger != null)
                {
                    passenger.transform.position = exitPosition;
                    passenger.SetActive(true);

                    NPC npc = passenger.GetComponent<NPC>();

                    if (npc != null)
                    {
                        npc.RevokePermission();
                    }
                }
            }

            seatRenderers[i].sprite = null;
            passengers[i] = null;
        }

        currentPeople = 0;

        
    }
    public void ClearSeat(int index)
    {
        if (index >= 0 && index < seatRenderers.Length)
        {
            seatRenderers[index].sprite = null;
        }
    }

    public bool HasSpace()
    {
        return currentPeople < maxCapacity;
    }

    public bool TryAddPassenger(GameObject obj, string idName, Sprite sprite)
    {
        if (!HasSpace())
        {
            return false;
        }

        if (sprite == null)
        {
            
            return false;
        }

        if (currentPeople >= seatRenderers.Length)
        {
            Debug.Log("Not enough seats assigned in the Inspector.");
            return false;
        }

        passengers[currentPeople] = new Passenger
        {
            obj = obj,
            IDName = idName,
            sprite = sprite
        };

        seatRenderers[currentPeople].sprite = sprite;
        currentPeople++;

        // Update the survivor mission.
        if (MissionManager.instance != null)
        {
            MissionManager.instance.AddProgress("Find useful survivors");
        }

        Debug.Log("Passenger entered van: " + currentPeople + "/" + maxCapacity);
        return true;
    }

    public bool HasAllRequiredParts()
    {
        return hasGasoline && hasSparkPlug;
    }

    public void CollectGasoline()
    {
        hasGasoline = true;
        Debug.Log("Gasoline collected.");
    }

    public void CollectSparkPlug()
    {
        hasSparkPlug = true;
        Debug.Log("Spark plug collected.");
    }

    public void MoveIfReady(bool playerInside)
    {
        if (!playerInside)
            return;

        if (!HasAllRequiredParts())
            return;

        transform.position +=
            Vector3.left * moveSpeed * Time.deltaTime;
    }

    public void RemovePerson(int index)
    {
        if (index < 0 || index >= currentPeople)
        {
            Debug.Log("Invalid passenger index.");
            return;
        }

        for (int i = index; i < currentPeople - 1; i++)
        {
            passengers[i] = passengers[i + 1];
            seatRenderers[i].sprite = passengers[i].sprite;
        }

        currentPeople--;
        passengers[currentPeople] = null;
        seatRenderers[currentPeople].sprite = null;

        Debug.Log(
            "Person removed from van: " +
            currentPeople + "/" + maxCapacity);
    }
}