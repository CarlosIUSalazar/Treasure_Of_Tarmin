using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using Unity.VisualScripting;

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
    ViewSwitcher viewSwitcher;
    PlayerGridMovement playerGridMovement;
    GameManager gameManager;
    ItemManager itemManager;


    public void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
        viewSwitcher = GameObject.Find("ViewSwitcher").GetComponent<ViewSwitcher>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
    }

    public void Update()
    {
        CheckIfRightHandHasItem();
    }


    public ItemMapping FigureOutCurrentRightHandItemMapping() {
        //RETURNS CURRENT RIGHT HAND ITEM MAPPING
        if (rightHandSlot.texture.name != "Transparent") {
            String currentWeaponName = rightHandSlot.texture.name;
            ItemMapping currentRightHandItemMapping = GetItemMapping(currentWeaponName);
            return currentRightHandItemMapping;
        } else {
            return null;
        }
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


    public ItemMapping GetItemMapping(string itemName) {
        // Check if the name ends with ".vox(Clone)" and remove it if so.
        if (itemName.EndsWith(".vox(Clone)"))
        {
            itemName = itemName.Replace(".vox(Clone)", "").Trim();
        } else if (itemName.EndsWith(".vox")) {
            itemName = itemName.Replace(".vox", "").Trim();
        }
        return itemMappings.Find(mapping => mapping.itemName == itemName);
    }


    public List<ItemMapping> GetBackpackItemMappings() {
        List<ItemMapping> backpackItems = new List<ItemMapping>();
        foreach (RawImage slot in backpackSlots) {
            // Check if the slot is not empty (assuming transparentImg means empty)
            if (slot.texture != null && slot.texture != transparentImg) {
                // Use the texture name to find the corresponding mapping
                ItemMapping mapping = GetItemMapping(slot.texture.name);
                if (mapping != null) {
                    backpackItems.Add(mapping);
                }
            }
        }
        return backpackItems;
    }


    public List<ItemMapping> GetKeysInBackpack() {
        // Reuse your existing method that gets backpack items.
        List<ItemMapping> backpackItems = GetBackpackItemMappings();
        // Filter for items that are keys.
        return backpackItems.FindAll(item => item.isKey);
    }


    public bool HasKeyForContainer(ItemMapping containerMapping) {
        List<ItemMapping> keys = GetAllKeys(); // Now includes backpack, left, and right keys
        if (containerMapping.isBlue) {
            // Blue containers require a blue key.
            return keys.Any(key => key.isBlue);
        } else if (containerMapping.isOrange) {
            // Orange containers accept an orange key or a blue key.
            return keys.Any(key => key.isBlue || key.isOrange);
        } else if (containerMapping.isTan) {
            // Tan containers accept any key.
            return keys.Any(key => key.isBlue || key.isOrange || key.isTan);
        }
        return false;
    }


    public List<ItemMapping> GetAllKeys() {
        List<ItemMapping> keys = new List<ItemMapping>();
        // Get keys from backpack slots.
        keys.AddRange(GetKeysInBackpack());
        // Check left hand slot.
        if (leftHandSlot.texture != null && leftHandSlot.texture != transparentImg) {
            ItemMapping leftItem = GetItemMapping(leftHandSlot.texture.name);
            if (leftItem != null && leftItem.isKey) {
                keys.Add(leftItem);
            }
        }
        // Check right hand slot.
        if (rightHandSlot.texture != null && rightHandSlot.texture != transparentImg) {
            ItemMapping rightItem = GetItemMapping(rightHandSlot.texture.name);
            if (rightItem != null && rightItem.isKey) {
                keys.Add(rightItem);
            }
        }
        return keys;
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
        } else if (itemName == "Treasure-Tarmin") {
            // WIN GAME SEQUENCE!!
            viewSwitcher.GameWinningSequence();
        } else {
            Debug.Log("Item mapping not found for " + itemName);
        }
    }


    public void DropAnItem() {
        Debug.Log("Pressed Drop Button");
        if (!isHoldingRightHandItem) {
            Debug.Log("I don't have anything to drop");
            return;
        } 
        Collider itemOnTheFloor = playerGridMovement.CheckForInteractablesAndReturnHitCollider();
        
        if (!itemOnTheFloor || itemOnTheFloor.gameObject.tag == "MazeSet" || itemOnTheFloor.gameObject.tag == "Door" || itemOnTheFloor.gameObject.tag == "OuterWall" || itemOnTheFloor.gameObject.tag == "CorridorDoorEast" || itemOnTheFloor.gameObject.tag == "CorridorDoorWest" || itemOnTheFloor.gameObject.tag == "Enemy") {
            itemOnTheFloor = null;
        }
        
        if (isHoldingRightHandItem && itemOnTheFloor == null) { //If I have something in my right hand and there is nothing on the floor just drop it
            Debug.Log("I have something in my right hand and nothing on the floor, I'll drop it");
            String currentItem = rightHandSlot.texture.name; //Remember current item in hand
            Spawn3DItem(currentItem);
            rightHandSlot.texture = transparentImg;
            isHoldingRightHandItem = false;
        } else if (isHoldingRightHandItem && itemOnTheFloor) { // If I have something in my right hand and there is something on the floor Swap it
            Debug.Log("I already have something in my right, I'll swap drop it");
            String currentItem = rightHandSlot.texture.name; //Remember current item in hand
            RaycastHit hit;
            hit = playerGridMovement.CheckForInteractablesAndReturnRaycastHit();
            Debug.Log("hit is " + hit.collider.name);

            if (hit.collider.name.Contains("Ladder")) {
                Debug.Log("Can't drop item on Ladder");
                gameManager.SetPlayerMessage("Can't drop item on Ladder");
                return;    
            } else if (hit.collider.name.Contains("Treasure")) {
                Debug.Log("Can't drop item on Ladder or Treasure");
                gameManager.SetPlayerMessage("Can't drop item on Treasure");
                return;
            }

            Debug.Log("item in hand is " + currentItem);
            rightHandSlot.texture = transparentImg; //For a moment erase the item from right hand
            itemManager.PickUpItem(hit);
            Spawn3DItem(currentItem);
            isHoldingRightHandItem = true;
        }
    }


    public void Spawn3DItem(string itemName)
    {
        Debug.Log("Inside Spawn3DItem, String is " + itemName);
        Vector3 playerPosition = GameObject.Find("Player").transform.position;

        ItemMapping itemMapping = GetItemMapping(itemName);
        Debug.Log("Spawn3D itemMapping is " + itemMapping);

        if (itemMapping != null && itemMapping.item3DPrefab != null) 
        {
            Debug.Log("Spawn3D inside if");
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
        // else 
        // {
        //     //This happens when you pick up an item with nothin in right hand currently
        //     Debug.LogWarning("3D Prefab not found for: " + itemName);
        // } 
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

            ////////////////////
            // IF ITEM IS SPECIAL BOOKS
            ///////////////////
            ///BLUE BOOK - TELEPORTATION ///
            if (rightHandItem.name == "Book-Special-Blue"){
                Debug.Log("Used Blue Special Book");
                playerGridMovement.MoveForwardWithBlueBook();            
            }

            ///PINK BOOK - INVISIBLE WALLS ///
            if (rightHandItem.name == "Book-Special-Pink" && !gameManager.isMazeTransparent){
                playerGridMovement.MakeMazeSetsTransparent();
                gameManager.isMazeTransparent = true;
                StartCoroutine(TimerToEndTransparentMaze());
            }

            /// PURPLE BOOK - MIDAS ///
            if (rightHandItem.name == "Book-Special-Purple"){
                //Debug.Log("Purple special");
                Collider item = playerGridMovement.CheckForInteractablesAndReturnHitCollider();
                if (item != null) {
                    string itemName = item.name.Replace(".vox(Clone)", "").Trim();
                    //Debug.Log("item is " + item.gameObject.name);
                    //Debug.Log("Trimmed name is " + itemName);
                    if (item == null) {
                        Debug.Log("No Collider");
                        return;
                    } else {
                        ItemMapping itemMapping = GetItemMapping(itemName);
                        if (!itemMapping.isArmor && !itemMapping.isWarWeapon && !itemMapping.isShield && !itemMapping.isTreasure && !itemMapping.isQuiver && !itemMapping.isFlour) { //Only Apply to War weapons, Shields and Armor
                            Debug.Log("Can't apply Midas to " + itemName);
                        } else {
                            // if itemName = "Knife-Orange" change to "Knife-White"
                            // Replace the last color with "White"
                            int lastDashIndex = itemName.LastIndexOf('-');
                            if (lastDashIndex != -1) 
                            {
                                string newItemName = itemName.Substring(0, lastDashIndex) + "-White";
                                Debug.Log($"Converted {itemName} to {newItemName}");
                                Destroy(item.gameObject);
                                Spawn3DItem(newItemName);
                            }
                        }
                    }
                }
            }

            ///////////
            ///SMALL POTIONS
            ///////////
            /// 
            ///BLUE SMALL POTION
            if (rightHandItem.name == "Potion-Small-Blue"){
                player.RestoreMaxPhysicalStrengthWithSmallBluePotion();
                EmptyRightHand();
            }
            ///PINK SMALL POTION
            if (rightHandItem.name == "Potion-Small-Pink"){
                //Help find better things in containers; magic ends with a little "whoosh" noise
                Debug.Log("Potion Better Loot enabled");
                gameManager.isSmallPinkPotionActive = true;
                StartCoroutine(DisableSmallPinkPotionEffect());
                // SWOOSH Sunds effct 
                EmptyRightHand();
            }

            ///PURPLE SMALL POTION
            if (rightHandItem.name == "Potion-Small-Purple"){
                //Make you invisible to all demons except the Minotaur (while making them invisible to you); magic ends with a little "whoosh" noise
                if (gameManager.isFighting == true) {
                    gameManager.SetPlayerMessage("Can't use in combat");
                    return;
                }
                
                if (gameManager.isSmallPurplePotionActive == true) {
                    gameManager.SetPlayerMessage("Potion already active");
                    return;
                }
                gameManager.HideAllEnemies(false);
                EmptyRightHand();
            }

            IEnumerator DisableSmallPinkPotionEffect() {
                yield return new WaitForSeconds(300f); //5Minutes
                gameManager.isSmallPinkPotionActive = false;
                // SWOOSH Sunds effct 
                Debug.Log("Potion Better Loot disabled");
            }

            ///////////
            ///LARGE POTIONS
            /////////
            if (rightHandItem.name == "Potion-Large-Blue"){
                player.IncreasePhysicalScoreBy10WithLargeBluePotion();
                EmptyRightHand();
            }

            if (rightHandItem.name == "Potion-Large-Pink"){
                player.IncreaseSpiritualScoreBy10WithLargePinkPotion();
                EmptyRightHand();
            }

            if (rightHandItem.name == "Potion-Large-Purple"){
            }
        }
    }


    private IEnumerator TimerToEndTransparentMaze() {
        yield return new WaitForSeconds(90f);
        playerGridMovement.RestoreMazeOpacity();
        gameManager.isMazeTransparent = false;
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