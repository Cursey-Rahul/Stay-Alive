
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite itemSprite;
    public int quantity;
}

public class InventoryData : MonoBehaviour
{
    [Header("Character Details")]
    [SerializeField] private string characterName;
    [SerializeField] private Sprite characterPhoto;
    [SerializeField] private string designation;
    [SerializeField, TextArea] private string backstory;

    [Header("Inventory")]
    [SerializeField] private List<Item> items = new List<Item>();

    public string CharacterName => characterName;
    public Sprite CharacterPhoto => characterPhoto;
    public string Designation => designation;
    public string Backstory => backstory;
    public Item[] Items => items.ToArray();

    public void AddItem(string itemName, Sprite itemSprite, int amount = 1)
    {
        Item existingItem = items.Find(
            item => item.itemName == itemName
        );

        if (existingItem != null)
        {
            existingItem.quantity += amount;
        }
        else
        {
            items.Add(new Item
            {
                itemName = itemName,
                itemSprite = itemSprite,
                quantity = amount
            });
        }
    }
}
