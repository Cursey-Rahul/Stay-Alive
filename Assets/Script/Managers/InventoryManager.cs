using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryItem
{
    public Image itemImage;
    public TextMeshProUGUI quantity;
}

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory UI Elements")]
    [SerializeField] protected Image photo;
    [SerializeField] protected TextMeshProUGUI IDName;
    [SerializeField] protected TextMeshProUGUI profession;
    [SerializeField] protected TextMeshProUGUI description;
    [SerializeField] protected InventoryItem[] items;

    public void UpdateInventoryUI(InventoryData inventoryData)
    {
        IDName.text = inventoryData.CharacterName;
        photo.sprite = inventoryData.CharacterPhoto;
        profession.text = inventoryData.Designation;
        description.text = inventoryData.Backstory;

        for (int i = 0; i < items.Length; i++)
        {
            if (i < inventoryData.Items.Length)
            {
                items[i].itemImage.sprite = inventoryData.Items[i].itemSprite;
                items[i].quantity.text = inventoryData.Items[i].quantity.ToString();

                items[i].itemImage.gameObject.SetActive(true);
                items[i].quantity.gameObject.SetActive(true);
            }
            else
            {
                items[i].itemImage.gameObject.SetActive(false);
                items[i].quantity.gameObject.SetActive(false);
            }
        }
    }
}