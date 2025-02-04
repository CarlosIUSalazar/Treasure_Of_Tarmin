using UnityEngine;

public class ItemManager : MonoBehaviour
{

    Player player;
    InventoryManager inventoryManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickUpItem(RaycastHit hit) {
    GameObject item = hit.collider.gameObject;
    Debug.Log("item is " + item.gameObject.name);
    
    string itemName = hit.collider.gameObject.name;

    switch (itemName) {
        case "Flour(Clone)":
            player.ModifyFood(10);
            Debug.Log("Picked up flour");
            break;
        case "Quiver(Clone)":
            player.ModifyArrows(10);
            Debug.Log("Picked up 10 Arrows");
            break;
        
        //////////////////////
        /// TREASURES 
        //////////////////////
// COINS
        case "Coins-Grey(Clone)":
            player.ModifyScore(10);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Coins-Yellow(Clone)":
            player.ModifyScore(30);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Coins-White(Clone)":
            player.ModifyScore(70);
            Debug.Log("Picked up Yellow Coins");
            break;
//  NECKLACES
        case "Necklace-Grey(Clone)":
            player.ModifyScore(20);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Necklace-Yellow(Clone)":
            player.ModifyScore(70);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Necklace-White(Clone)":
            player.ModifyScore(200);
            Debug.Log("Picked up Yellow Coins");
            break;
// INGOTS
        case "Ingot-Grey(Clone)":
            player.ModifyScore(50);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Ingot-Yellow(Clone)":
            player.ModifyScore(350);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Ingot-White(Clone)":
            player.ModifyScore(450);
            Debug.Log("Picked up Yellow Coins");
            break;
// LAMPS
        case "Lamp-Grey(Clone)":
            player.ModifyScore(100);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Lamp-Yellow(Clone)":
            player.ModifyScore(150);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Lamp-White(Clone)":
            player.ModifyScore(220);
            Debug.Log("Picked up Yellow Coins");
            break;
// CHALICES
        case "Chalice-Grey(Clone)":
            player.ModifyScore(120);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Chalice-Yellow(Clone)":
            player.ModifyScore(250);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Chalice-White(Clone)":
            player.ModifyScore(400);
            Debug.Log("Picked up Yellow Coins");
            break;
// CROWNS
        case "Crown-Grey(Clone)":
            player.ModifyScore(300);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Crown-Yellow(Clone)":
            player.ModifyScore(500);
            Debug.Log("Picked up Yellow Coins");
            break;

        case "Crown-White(Clone)":
            player.ModifyScore(600);
            Debug.Log("Picked up Yellow Coins");
            break;

        //////////////////////
        /// WAR WEAPONS 
        //////////////////////
        
        case "Bow-Grey.vox(Clone)":
            //player.ModifyScore(600);
            Debug.Log("Picked up " + item.gameObject.name);
            inventoryManager.AssignToRightHand(inventoryManager.GreyBow2D, true);
            break;
// BOWS
    }
    //item.SetActive(false);
    Destroy(item);
    }
}
