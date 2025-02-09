using UnityEngine;

public class ItemManager : MonoBehaviour
{

    Player player;
    InventoryManager inventoryManager;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();
    }

    public void PickUpItem(RaycastHit hit) {
        GameObject item = hit.collider.gameObject;

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
                break;
            case "Quiver":
                player.ModifyArrows(10);
                Debug.Log("Picked up 10 Arrows");
                break;
            
            //////////////////////
            /// TREASURES 
            //////////////////////
    // COINS
            case "Coins-Grey":
                player.ModifyScore(10);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Coins-Yellow":
                player.ModifyScore(30);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Coins-White":
                player.ModifyScore(70);
                Debug.Log("Picked up Yellow Coins");
                break;
    //  NECKLACES
            case "Necklace-Grey":
                player.ModifyScore(20);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Necklace-Yellow":
                player.ModifyScore(70);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Necklace-White":
                player.ModifyScore(200);
                Debug.Log("Picked up Yellow Coins");
                break;
    // INGOTS
            case "Ingot-Grey":
                player.ModifyScore(50);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Ingot-Yellow":
                player.ModifyScore(350);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Ingot-White":
                player.ModifyScore(450);
                Debug.Log("Picked up Yellow Coins");
                break;
    // LAMPS
            case "Lamp-Grey":
                player.ModifyScore(100);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Lamp-Yellow":
                player.ModifyScore(150);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Lamp-White":
                player.ModifyScore(220);
                Debug.Log("Picked up Yellow Coins");
                break;
    // CHALICES
            case "Chalice-Grey":
                player.ModifyScore(120);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Chalice-Yellow":
                player.ModifyScore(250);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Chalice-White":
                player.ModifyScore(400);
                Debug.Log("Picked up Yellow Coins");
                break;
    // CROWNS
            case "Crown-Grey":
                player.ModifyScore(300);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Crown-Yellow":
                player.ModifyScore(500);
                Debug.Log("Picked up Yellow Coins");
                break;

            case "Crown-White":
                player.ModifyScore(600);
                Debug.Log("Picked up Yellow Coins");
                break;
            
            default: 
                inventoryManager.AssignToRightHand(itemName, true);
                break;
        }

        //item.SetActive(false);
        Destroy(item);
    }
}
