using UnityEngine;

[CreateAssetMenu(
    fileName = "NewItem",
    menuName = "EmberTail/Inventory/Item"
)]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;

    public string ItemId =>
        itemId;

    public string DisplayName =>
        displayName;
}