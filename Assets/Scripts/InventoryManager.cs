using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

// Tan, Orange, Blue, Grey, Yellow, White
// Blue, Grey, White, Pink, Red, Purple
public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<ItemMapping> itemMappings; // Drag all ItemMapping assets here
    [SerializeField] private RawImage leftHandSlot; // RawImage for left-hand slot
    [SerializeField] public RawImage rightHandSlot; // RawImage for right-hand slot
    [SerializeField] private RawImage[] backpackSlots; // Drag BackpackSlot1 to BackpackSlot6 here
    [SerializeField] private Texture2D transparentImg;
    private Texture emptyTexture; // Assign an empty/default texture in the Inspector
    Player player;
    public bool isHoldingRightHandItem = false;

    public void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void Update()
    {
        CheckIfRightHandHasItem();
    }

    public void EmptyRightHand() {
        rightHandSlot.texture = transparentImg;
    }

    private void CheckIfRightHandHasItem()
    {
        if (rightHandSlot.texture == transparentImg)
        {
            isHoldingRightHandItem = false;
        } else {
            isHoldingRightHandItem = true;
        }
    }

    public ItemMapping GetItemMapping(string itemName){
        return itemMappings.Find(mapping => mapping.itemName == itemName);
    }

    // Assign an item to a slot
    public void AssignItemToSlot(int slotIndex, Texture itemTexture)
    {
        if (slotIndex < 0 || slotIndex >= backpackSlots.Length) return;

        backpackSlots[slotIndex].texture = itemTexture;
        backpackSlots[slotIndex].color = Color.white; // Make sure it’s visible
    }

    // Swap items between two slots
    public void SwapItems(int slotIndexA, int slotIndexB)
    {
        if (slotIndexA < 0 || slotIndexB < 0 || slotIndexA >= backpackSlots.Length || slotIndexB >= backpackSlots.Length)
            return;

        Texture temp = backpackSlots[slotIndexA].texture;
        backpackSlots[slotIndexA].texture = backpackSlots[slotIndexB].texture;
        backpackSlots[slotIndexB].texture = temp;
    }

    // Assign an item directly to the left hand
    public void AssignToLeftHand(Texture shieldTexture)
    {
        if (shieldTexture != null)
        {
            leftHandSlot.texture = shieldTexture;
            leftHandSlot.color = Color.white;
        }
    }

    // Assign an item directly to the right hand
    public void AssignToRightHand(string itemName, bool isNewItem)
    {
        ItemMapping itemMapping = GetItemMapping(itemName);
        if (rightHandSlot.texture != null) // If right hand is already holding an item
        {
            Debug.Log("Right hand is already holding an item which is: " + rightHandSlot.texture.name);
            if (isNewItem == true) {  // Is not assinged from Drag and Drop but from the floor
                Spawn3DItem(rightHandSlot.texture.name); // Spawn the currently holding item into a 3D item on the ground
            }
        }

        if (itemMapping != null) // If the item mapping is found
        {
            rightHandSlot.texture = itemMapping.item2DSprite; // Assign the item to the right hand in 2D
            Debug.Log("rightHandSlot.texture is " + rightHandSlot.texture.name);
            rightHandSlot.color = Color.white;
            player.ModifyWeaponAttackPower(itemMapping);
            Debug.Log("Assinged " + itemMapping.itemName + " to right hand");
        } else 
        {
            Debug.Log("Item mapping not found for " + itemName);
        }
    }

    public void DropAnItem() {
        if (rightHandSlot.texture != null) {
            String currentItem = rightHandSlot.texture.name;
            Spawn3DItem(currentItem);
            rightHandSlot.texture = transparentImg;
        }
    }

    public void Spawn3DItem(string itemName)
    {
        Vector3 playerPosition = GameObject.Find("Player").transform.position;

        ItemMapping itemMapping = GetItemMapping(itemName);
        
        if (itemMapping != null && itemMapping.item3DPrefab != null) 
        {
            // Instantiate the item
            GameObject spawnedItem = Instantiate(itemMapping.item3DPrefab, 
                new Vector3(playerPosition.x, 0.1f, playerPosition.z), 
                Quaternion.identity);

            // Check and remove the Projectile component if it exists
            Projectile projectileComponent = spawnedItem.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                Destroy(projectileComponent);
            }
            
            Debug.Log("Spawned 3D Item: " + itemMapping.item3DPrefab.name);
        }
        else 
        {
            Debug.LogWarning("3D Prefab not found for: " + itemName);
        } 
    }



}
