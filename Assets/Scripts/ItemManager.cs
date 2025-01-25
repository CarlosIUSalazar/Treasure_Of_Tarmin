using UnityEngine;

public class ItemManager : MonoBehaviour
{

    Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
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
        case "Coins-Yellow(Clone)":
            player.ModifyScore(100);
            Debug.Log("Picked up Yellow Coins");
            break;
    }
    
    item.SetActive(false);
    }
}
