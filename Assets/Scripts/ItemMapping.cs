using UnityEngine;

[CreateAssetMenu(fileName = "ItemMapping", menuName = "Inventory/ItemMapping")]
public class ItemMapping : ScriptableObject
{
    public string itemName; // A unique name for the item
    public GameObject item3DPrefab; // Reference to the 3D prefab
    public Texture2D item2DSprite; // Reference to the 2D sprite
}
