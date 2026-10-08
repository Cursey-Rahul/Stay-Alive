using UnityEngine;

public class VanManager : MonoBehaviour
{
    [Header("Shelter")]
    [SerializeField] private int maxCapacity = 5;

    private int currentPeople = 0;

    public int CurrentPeople => currentPeople;
    public int MaxCapacity => maxCapacity;
    [SerializeField] private GameObject[] seats;
    SpriteRenderer[] seatRenderers;

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
}