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
    [SerializeField] private RawImage breastPlateImg;
    [SerializeField] private RawImage helmetImg;
    [SerializeField] private RawImage hauberkImg;
    [SerializeField] private RawImage gauntletImg;
    [SerializeField] private RawImage ringImg;

    private Texture emptyTexture; // Assign an empty/default texture in the Inspector
    public bool isHoldingRightHandItem = false;
    private ItemMapping currentHelmet;
    private ItemMapping currentBreastPlate;
    private ItemMapping currentHauberk;
    private ItemMapping currentGauntlet;
    private ItemMapping currentRing;
    private ItemMapping currentShield;

    Player player;

    public void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void Update()
    {
    }

    public void EmptyRightHand() {
        rightHandSlot.texture = transparentImg;
    }

    public bool CheckIfRightHandHasItem()
    {
        if (rightHandSlot.texture == transparentImg)
        {
            isHoldingRightHandItem = false;
        } else {
            isHoldingRightHandItem = true;
        }
        return isHoldingRightHandItem;
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


    public void UseButton() {
        if (!CheckIfRightHandHasItem()) {
            Debug.Log("Nothing to use");
        } else {
            ItemMapping rightHandItem = GetItemMapping(rightHandSlot.texture.name);
            ////////////////////
            // IF ITEM IS ARMOR
            ///////////////////
            // BREASTPLATE
            if (rightHandItem.isArmor && rightHandItem.isBreastPlate) {
                if (currentBreastPlate == null || rightHandItem.warDefense > currentBreastPlate.warDefense || rightHandItem.spiritualDefense >  currentBreastPlate.spiritualDefense) {
                    // If RightHand holding Armor is better than currently wearing one, replace:
                    breastPlateImg.texture = rightHandItem.item2DSprite;
                    currentBreastPlate = rightHandItem; // Update the equipped Breastplate
                    Debug.Log("Equiped " + rightHandItem.name);
                    CalculateCurrentArmorTotal();
                } else {
                    Debug.Log("Better Item Already Equiped, Discarding");
                }
                EmptyRightHand();
            }
            // HELMET
            if (rightHandItem.isArmor && rightHandItem.isHelmet) {
                if (currentHelmet == null || rightHandItem.warDefense > currentHelmet.warDefense || rightHandItem.spiritualDefense > currentHelmet.spiritualDefense) {
                    // If RightHand holding Armor is better than currently wearing one, replace:
                    helmetImg.texture = rightHandItem.item2DSprite;
                    currentHelmet = rightHandItem; // Update the equipped helmet
                    Debug.Log("Equiped " + rightHandItem.name);
                    CalculateCurrentArmorTotal();
                } else {
                    Debug.Log("Better Item Already Equiped, Discarding");
                }
                EmptyRightHand();
            }

            // HAUBERK
            if (rightHandItem.isArmor && rightHandItem.isHauberk) {
                if (currentHauberk == null || rightHandItem.warDefense > currentHauberk.warDefense || rightHandItem.spiritualDefense > currentHauberk.spiritualDefense) {
                    // If RightHand holding Armor is better than currently wearing one, replace:
                    hauberkImg.texture = rightHandItem.item2DSprite;
                    currentHauberk = rightHandItem; // Update the equipped Hauberk
                    Debug.Log("Equiped " + rightHandItem.name);
                    CalculateCurrentArmorTotal();
                } else {
                    Debug.Log("Better Item Already Equiped, Discarding");
                }
                EmptyRightHand();
            }
            
            // GAUNTLET
            if (rightHandItem.isArmor && rightHandItem.isGauntlet) {
                if (currentGauntlet == null || rightHandItem.warDefense > currentGauntlet.warDefense || rightHandItem.spiritualDefense > currentGauntlet.spiritualDefense) {
                    // If RightHand holding Armor is better than currently wearing one, replace:
                    gauntletImg.texture = rightHandItem.item2DSprite;
                    currentGauntlet = rightHandItem; // Update the equipped Gauntlet
                    Debug.Log("Equiped " + rightHandItem.name);
                    CalculateCurrentArmorTotal();
                } else {
                    Debug.Log("Better Item Already Equiped, Discarding");
                }
                EmptyRightHand();
            }

            // RING
            if (rightHandItem.isArmor && rightHandItem.isRing) {
                if (currentRing == null || rightHandItem.warDefense > currentRing.warDefense || rightHandItem.spiritualDefense > currentRing.spiritualDefense) {
                    // If RightHand holding Armor is better than currently wearing one, replace:
                    ringImg.texture = rightHandItem.item2DSprite;
                    currentRing = rightHandItem; // Update the equipped Ring
                    Debug.Log("Equiped " + rightHandItem.name);
                    CalculateCurrentArmorTotal();
                } else {
                    Debug.Log("Better Item Already Equiped, Discarding");
                }
                EmptyRightHand();
            }
        }
    }

    private void CalculateCurrentArmorTotal() {
        int currentPhysicalArmorTotal = 0;

        currentPhysicalArmorTotal = 
            (currentHelmet?.warDefense ?? 0) + 
            (currentBreastPlate?.warDefense ?? 0) + 
            (currentHauberk?.warDefense ?? 0) + 
            (currentGauntlet?.warDefense ?? 0) + 
            (currentRing?.warDefense ?? 0);

        Debug.Log("Total Physical Armor: " + currentPhysicalArmorTotal);

        int currentSpiritualArmorTotal = 0;

        if (currentHelmet != null) currentSpiritualArmorTotal += currentHelmet.spiritualDefense;
        if (currentBreastPlate != null) currentSpiritualArmorTotal += currentBreastPlate.spiritualDefense;
        if (currentHauberk != null) currentSpiritualArmorTotal += currentHauberk.spiritualDefense;
        if (currentGauntlet != null) currentSpiritualArmorTotal += currentGauntlet.spiritualDefense;
        if (currentRing != null) currentSpiritualArmorTotal += currentRing.spiritualDefense;

        Debug.Log("Total Physical Armor: " + currentSpiritualArmorTotal);

        player.physicalArmor = currentPhysicalArmorTotal;
        player.spiritualArmor = currentSpiritualArmorTotal;
        player.UpdateUIStats();
        }
}

// 🔍 Explanation:
// ?. (Null Conditional Operator):
// currentHelmet?.warDefense means "if currentHelmet is not null, access warDefense, otherwise return null".
// ?? (Null Coalescing Operator):
// (currentHelmet?.warDefense ?? 0) means "if warDefense is null (because currentHelmet is null), use 0 instead".
// Adds only non-null values:
// If any armor piece is missing (null), its warDefense will default to 0, avoiding errors.