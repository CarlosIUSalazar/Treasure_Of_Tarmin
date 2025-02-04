using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private RawImage leftHandSlot; // RawImage for left-hand slot
    [SerializeField] private RawImage rightHandSlot; // RawImage for right-hand slot
    [SerializeField] private RawImage[] backpackSlots; // Drag BackpackSlot1 to BackpackSlot6 here
    [SerializeField] public GameObject GreyBow3D;
    [SerializeField] public Texture2D GreyBow2D;


    private Texture emptyTexture; // Assign an empty/default texture in the Inspector
    Player player;

    public void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
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
    public void AssignToRightHand(Texture itemTexture, bool isNew)
    {
        if (itemTexture != null)
        {
            Texture currentTexture = itemTexture;
            Debug.Log("currentTexture in rightHand: " + currentTexture.name);
            if (isNew)
            {
                rightHandSlot.texture = itemTexture;
                rightHandSlot.color = Color.white;
                Spawn3DItem(currentTexture.name);
            }
            else
            {
                rightHandSlot.texture = itemTexture;
                rightHandSlot.color = Color.white;
            }

            // rightHandSlot.texture = itemTexture;
            // rightHandSlot.color = Color.white;
        }
    }

    public void Spawn3DItem(string item)
    {
        Vector3 playerPosition = GameObject.Find("Player").transform.position;

        GameObject = Item2Dto3D(string name);

        GameObject item3D = Instantiate(item3D, new Vector3(playerPosition.x, 0.1f,playerPosition.z), Quaternion.identity);
        Debug.Log("Instantiated 3D item: " + item3D.name);
    }

    private GameObject item2Dto3D(string name)
    {
        switch (name)
        {
            case "GreyBow2D":
                return GreyBow3D;
            default:
                return null;
        }
    }
}
