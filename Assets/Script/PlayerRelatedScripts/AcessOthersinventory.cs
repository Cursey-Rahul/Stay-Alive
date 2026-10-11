using UnityEngine;

public class AcessOthersinventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private InventoryManager inventoryManager;

    public InventoryData otherInventoryData;
    public NPC otherNPC;

    private void OnTriggerEnter2D(Collider2D other)
    {
        InventoryData inventoryData =
            other.GetComponent<InventoryData>();
        NPC npc = other.GetComponent<NPC>();
        if (inventoryData != null)
        {
            otherInventoryData = inventoryData;
            otherNPC = npc;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InventoryData inventoryData =
            other.GetComponent<InventoryData>();
        NPC npc = other.GetComponent<NPC>();
       
        if (inventoryData == otherInventoryData)
        {
            otherInventoryData = null;
            otherNPC = null;
            
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OpenOtherInventory();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
           Permit();
        }
    }
    private void Permit()
    {
        if (otherNPC == null)
        {
            
            return;
        }
        otherNPC.permitted = true;
    }

    private void OpenOtherInventory()
    {
        if (otherInventoryData == null)
        {
            
            return;
        }

        inventoryManager.UpdateInventoryUI(
            otherInventoryData
        );

        UI.instance.ToggleInventory();
    }
}