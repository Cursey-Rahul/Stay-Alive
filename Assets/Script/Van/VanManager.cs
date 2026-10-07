using UnityEngine;

public class VanManager : MonoBehaviour
{
    [Header("Shelter")]
    [SerializeField] private int maxCapacity = 5;

    private int currentPeople = 0;

    public int CurrentPeople => currentPeople;
    public int MaxCapacity => maxCapacity;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddPerson();
        }
    }

    public bool HasSpace()
    {
        return currentPeople < maxCapacity;
    }

    public bool AddPerson()
    {
        if (!HasSpace())
        {
            Debug.Log("Van is full!");
            return false;
        }

        currentPeople++;

        Debug.Log(
            "Person entered Van: " +
            currentPeople + "/" + maxCapacity
        );

        return true;
    }
}