using UnityEngine;

public class VanManager : MonoBehaviour
{
    [Header("Shelter")]
    [SerializeField] private int maxCapacity = 6;

    private int currentPeople = 0;

    public int CurrentPeople => currentPeople;
    public int MaxCapacity => maxCapacity;
    [SerializeField] private GameObject[] seats;
    SpriteRenderer[] seatRenderers;
    class Passenger
    {
        public GameObject obj;
        public string IDName;
        public Sprite sprite;
    }
    Passenger[] passengers = new Passenger[6];

    private void Start()
    {
        seatRenderers = new SpriteRenderer[seats.Length];
        for (int i = 0; i < seats.Length; i++)
        {
            seatRenderers[i] = seats[i].GetComponent<SpriteRenderer>();
        }
    }

    public bool HasSpace()
    {
        return currentPeople < maxCapacity;
    }

    public bool AddPerson(Sprite s)
    {
        if (!HasSpace())
        {
            Debug.Log("Van is full!");
            return false;
        }

        currentPeople++;

        if (currentPeople - 1 < seatRenderers.Length)
        {
            seatRenderers[currentPeople - 1].sprite = s;
        }

        Debug.Log(
            "Person entered Van: " +
            currentPeople + "/" + maxCapacity
        );

        return true;
    }
    public void RemovePerson(Sprite s)
    {


    }
    public void RemovePerson(int index)
    {
        if (index < 0 || index >= currentPeople)
        {
            Debug.Log("Invalid index for removing person from Van.");
            return;
        }
        currentPeople--;
        seatRenderers[index].sprite = null;

        Debug.Log(
            "Person removed from Van: " +
            currentPeople + "/" + maxCapacity
        );
    }
    public void RecordPassengerDetails(GameObject obj, string IDName, Sprite s)
    {
        if (s == null)
        {

            Debug.Log("Cannot record passenger with null sprite.");
            return;
        }

        else if (currentPeople >= maxCapacity)
        {
            Debug.Log("Cannot record passenger. Van is full.");
            return;
        }
        else
        {
            passengers[currentPeople] = new Passenger
            {
                obj = obj,
                IDName = IDName,
                sprite = s
            };
            Debug.Log("Passenger recorded: " + IDName);
        }


    }
}