using UnityEngine;

public class ItemManager : MonoBehaviour
{

    Player player;
    InventoryManager inventoryManager;
    GameManager gameManager;
    ViewSwitcher viewSwitcher;
    PlayerGridMovement playerGridMovement;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        viewSwitcher = GameObject.Find("ViewSwitcher").GetComponent<ViewSwitcher>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
    }

    public void PickUpItem(RaycastHit hit) {
        if (playerGridMovement.isMoving || playerGridMovement.isRotating) return; // Prevent movement if already moving or rotating

        GameObject item = hit.collider.gameObject;

        gameManager.PlayClickSoundEffect();

        string itemName = item.name.Replace(".vox(Clone)", "").Trim();
        Debug.Log("item is " + item.gameObject.name);
        Debug.Log("Trimmed name is " + itemName);
        //string itemName = hit.collider.gameObject.name;

        //Here checking if right hand item is already assigned
        //If yes, then destroy the item and assign the new item to right hand and spawn the already existing item in hand on the floor in front of player in 3D
        //If no, then only assign the item to right hand
        switch (itemName) {
            case "Flour":
                player.ModifyFood(10);
                Debug.Log("Picked up flour");
                gameManager.SetPlayerMessage("Picked up Food");
                break;
            case "Quiver":
                player.ModifyArrows(10);
                Debug.Log("Picked up 10 Arrows");
                gameManager.SetPlayerMessage("Picked up 10 Arrows");
                break;
            
            //////////////////////
            /// TREASURES 
            //////////////////////
    // COINS
            case "Coins-Grey":
                player.ModifyScore(10);
                Debug.Log("Picked up Grey Coins");
                gameManager.SetPlayerMessage("Picked up Grey Coins");
                break;

            case "Coins-Yellow":
                player.ModifyScore(30);
                Debug.Log("Picked up Yellow Coins");
                gameManager.SetPlayerMessage("Picked up Yellow Coins");
                break;

            case "Coins-White":
                player.ModifyScore(70);
                Debug.Log("Picked up White Coins");
                gameManager.SetPlayerMessage("Picked up White Coins");
                break;
    //  NECKLACES
            case "Necklace-Grey":
                player.ModifyScore(20);
                Debug.Log("Picked up Grey Necklace");
                gameManager.SetPlayerMessage("Picked up Grey Necklace");
                break;

            case "Necklace-Yellow":
                player.ModifyScore(70);
                Debug.Log("Picked up Yellow Necklace");
                gameManager.SetPlayerMessage("Picked up Yellow Neckalce");
                break;

            case "Necklace-White":
                player.ModifyScore(200);
                Debug.Log("Picked up White Necklace");
                gameManager.SetPlayerMessage("Picked up White Necklace");
                break;
    // INGOTS
            case "Ingot-Grey":
                player.ModifyScore(50);
                Debug.Log("Picked up Grey Ingot");
                gameManager.SetPlayerMessage("Picked up Grey Ingot");
                break;

            case "Ingot-Yellow":
                player.ModifyScore(350);
                Debug.Log("Picked up Yellow Ingot");
                gameManager.SetPlayerMessage("Picked up Yellow Ingot");
                break;

            case "Ingot-White":
                player.ModifyScore(450);
                Debug.Log("Picked up White Ingot");
                gameManager.SetPlayerMessage("Picked up White Ingot");
                break;
    // LAMPS
            case "Lamp-Grey":
                player.ModifyScore(100);
                Debug.Log("Picked up Grey Lamp");
                gameManager.SetPlayerMessage("Picked up Grey Lamp");
                break;

            case "Lamp-Yellow":
                player.ModifyScore(150);
                Debug.Log("Picked up Yellow Lamp");
                gameManager.SetPlayerMessage("Picked up Yellow Lamp");
                break;

            case "Lamp-White":
                player.ModifyScore(220);
                Debug.Log("Picked up White Lamp");
                gameManager.SetPlayerMessage("Picked up White Lamp");
                break;
    // CHALICES
            case "Chalice-Grey":
                player.ModifyScore(120);
                Debug.Log("Picked up Grey Chalice");
                gameManager.SetPlayerMessage("Picked up Grey Chalice");
                break;

            case "Chalice-Yellow":
                player.ModifyScore(250);
                Debug.Log("Picked up Yellow Chalice");
                gameManager.SetPlayerMessage("Picked up Yellow Chalice");
                break;

            case "Chalice-White":
                player.ModifyScore(400);
                Debug.Log("Picked up White Chalice");
                gameManager.SetPlayerMessage("Picked up White Chalice");
                break;
    // CROWNS
            case "Crown-Grey":
                player.ModifyScore(300);
                Debug.Log("Picked up Grey Crown");
                gameManager.SetPlayerMessage("Picked up Grey Crown");
                break;

            case "Crown-Yellow":
                player.ModifyScore(500);
                Debug.Log("Picked up Yellow Crown");
                gameManager.SetPlayerMessage("Picked up Yellow Crown");
                break;

            case "Crown-White":
                player.ModifyScore(600);
                Debug.Log("Picked up White Crown");
                gameManager.SetPlayerMessage("Picked up White Crown");
                break;

            case "Treasure-Tarmin":
                // WIN GAME SEQUENCE!!
                Debug.Log("Picked up TREASURE OF TARMIN!");
                viewSwitcher.GameWinningSequence();
                break;

            default: 
                inventoryManager.HandleItemPickup(itemName);
                //inventoryManager.AssignToRightHand(itemName, true); // true tells the inventory manager is a new item from the floor not passed from drag and drop to right hand
                break;
        }

        //item.SetActive(false);
        // Prevents certain types of items or things from being destroyed when dropping an item and there's no other item to swap for
        //if (item.tag != "MazeSet" && item.tag != "Door" && item.tag != "OuterWall" && item.tag != "CorridorDoorEast" && item.tag != "CorridorDoorWest" && item.tag != "Enemy") {
            Debug.Log("Destroying " + item.name);
            Debug.Log("Destroy tag is " + item.tag);
            Destroy(item);
        //}
    }
}
