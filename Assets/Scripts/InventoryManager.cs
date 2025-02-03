using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private RawImage leftHandSlot; // RawImage for left-hand slot
    [SerializeField] private RawImage rightHandSlot; // RawImage for right-hand slot
    [SerializeField] private RawImage[] backpackSlots; // Drag BackpackSlot1 to BackpackSlot6 here

    private Texture emptyTexture; // Assign an empty/default texture in the Inspector

    public void Update() {
        Debug.Log("leftHandSlot: " + leftHandSlot.GetComponent<RawImage>().texture.name);
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
        if (itemTexture != null)
        {
            rightHandSlot.texture = itemTexture;
            rightHandSlot.color = Color.white;
        }
    }
}
