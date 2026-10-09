using UnityEngine;

[System.Serializable]
public class Item
{
    public Sprite itemSprite;
    public int quantity;
}

public class InventoryData : MonoBehaviour
{
    [Header("Character Details")]
    [SerializeField] private string characterName;
    [SerializeField] private Sprite characterPhoto;
    [SerializeField] private string designation;
    [SerializeField][TextArea] private string backstory;

    [Header("Inventory")]
    [SerializeField] private Item[] items;

    public string CharacterName => characterName;
    public Sprite CharacterPhoto => characterPhoto;
    public string Designation => designation;
    public string Backstory => backstory;
    public Item[] Items => items;
}