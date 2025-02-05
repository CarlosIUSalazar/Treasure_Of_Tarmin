using UnityEngine;
using UnityEngine.UI;
// Tan, Orange, Blue, Grey, Yellow, White
// Blue, Grey, White, Pink, Red, Purple
public class InventoryManager : MonoBehaviour
{
    [SerializeField] private RawImage leftHandSlot; // RawImage for left-hand slot
    [SerializeField] private RawImage rightHandSlot; // RawImage for right-hand slot
    [SerializeField] private RawImage[] backpackSlots; // Drag BackpackSlot1 to BackpackSlot6 here
    
    //3D models
    [SerializeField] public GameObject TanBow3D;
    [SerializeField] public GameObject OrangeBow3D;
    [SerializeField] public GameObject BlueBow3D;    
    [SerializeField] public GameObject GreyBow3D;
    [SerializeField] public GameObject YellowBow3D;
    [SerializeField] public GameObject WhiteBow3D;

    [SerializeField] public GameObject TanKnife3D;
    [SerializeField] public GameObject OrangeKnife3D;
    [SerializeField] public GameObject BlueKnife3D;    
    [SerializeField] public GameObject GreyKnife3D;
    [SerializeField] public GameObject YelowKnife3D;
    [SerializeField] public GameObject WhiteKnife3D;
    
    //2D textures
    [SerializeField] public Texture2D TanBow2D;
    [SerializeField] public Texture2D OrangeBow2D;
    [SerializeField] public Texture2D GreyBow2D;
    [SerializeField] public Texture2D BlueBow2D;
    [SerializeField] public Texture2D YellowBow2D;
    [SerializeField] public Texture2D WhiteBow2D;

    [SerializeField] public Texture2D TanKnife2D;
    [SerializeField] public Texture2D OrangeKnife2D;
    [SerializeField] public Texture2D GreyKnife2D;
    [SerializeField] public Texture2D BlueKnife2D;
    [SerializeField] public Texture2D YellowKnife2D;
    [SerializeField] public Texture2D WhiteKnife2D;

    


    private Texture emptyTexture; // Assign an empty/default texture in the Inspector
    Player player;

    public void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void Update() {
        //Debug.Log("Right hand item: " + rightHandSlot.texture.name);
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
    public void AssignToRightHand(Texture itemTexture)
    {
        if (rightHandSlot.texture != emptyTexture)
        {

        }
        if (itemTexture != null)
        {
            Texture currentTexture = itemTexture;
            Debug.Log("currentTexture in rightHand: " + currentTexture.name);
            if (true)
            {
                rightHandSlot.texture = itemTexture;
                rightHandSlot.color = Color.white;
                Spawn3DItem(currentTexture.name);
            }
            else
            {
                //rightHandSlot.texture = itemTexture;
                //rightHandSlot.color = Color.white;
            }

            // rightHandSlot.texture = itemTexture;
            // rightHandSlot.color = Color.white;
        }
    }

    public void Spawn3DItem(string item)
    {
        Vector3 playerPosition = GameObject.Find("Player").transform.position;

        GameObject itemToSpawn = Item2Dto3D(name);

        GameObject item3D = Instantiate(itemToSpawn, new Vector3(playerPosition.x, 0.1f,playerPosition.z), Quaternion.identity);
        Debug.Log("Instantiated 3D item: " + item3D.name);
    }

    private GameObject Item2Dto3D(string name)
    {
        switch (name)
        {
            case "Bow-Tan":
                return TanBow3D;
            case "Bow-Orange":
                return OrangeBow3D;
            case "Bow-Grey":
                return GreyBow3D;
            case "Bow-Blue":
                return BlueBow3D;
            case "Bow-Yellow":
                return YellowBow3D;
            case "Bow-White":
                return GreyBow3D;

            case "Knife-Tan":
                return TanKnife3D; 
            case "Knife-Orange":
                return OrangeKnife3D;
            case "Knife-Grey":
                return GreyKnife3D;
            case "Knife-Blue":
                return BlueKnife3D; 
            case "Knife-Yellow":
                return YelowKnife3D;
            case "Knife-White":
                return WhiteKnife3D;
            default:
                return null;
        }
    }


    public GameObject TakenItemDecider(string name) {
        switch (name)
        {
            case "Bow-Tan.vox(Clone)":
                return TanBow3D;
            case "Bow-Orange.vox(Clone)":
                return OrangeBow3D;
            case "Bow-Grey.vox(Clone)":
                return GreyBow3D;
            case "Bow-Blue.vox(Clone)":
                return BlueBow3D;
            case "Bow-Yellow.vox(Clone)":
                return YellowBow3D;
            case "Bow-White.vox(Clone)":
                return GreyBow3D;

            case "Knife-Tan.vox(Clone)":
                return TanKnife3D; 
            case "Knife-Orange.vox(Clone)":
                return OrangeKnife3D;
            case "Knife-Grey.vox(Clone)":
                return GreyKnife3D;
            case "Knife-Blue.vox(Clone)":
                return BlueKnife3D; 
            case "Knife-Yellow.vox(Clone)":
                return YelowKnife3D;
            case "Knife-White.vox(Clone)":
                return WhiteKnife3D;
            default:
                return null;
        }

    }

}
