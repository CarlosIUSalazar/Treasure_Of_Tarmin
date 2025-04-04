using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class SlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    InventoryManager inventoryManager;
    private RawImage draggedImage; // The image being dragged
    private Transform originalSlot; // The original slot the drag started from
    private Texture originalTexture; // The texture in the original slot
    private Canvas canvas; // Reference to the canvas

    // Hardcoded positions for the slots
    private Dictionary<string, Vector3> slotPositions = new Dictionary<string, Vector3>
    {
        { "BackpackSlot1", new Vector3(-80, -198, 0) },
        { "BackpackSlot2", new Vector3(-35, -198, 0) },
        { "BackpackSlot3", new Vector3(10, -198, 0) },
        { "BackpackSlot4", new Vector3(56, -198, 0) },
        { "BackpackSlot5", new Vector3(102, -198, 0) },
        { "BackpackSlot6", new Vector3(147, -198, 0) },
        { "LeftHandSlot", new Vector3(-140, -198, 0) },
        { "RightHandSlot", new Vector3(135, -267, 0) }
    };

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedImage = GetComponent<RawImage>();

        // Record the specific slot being clicked and dragged
        originalSlot = transform; // Use `transform` to refer to the current object (the slot itself)
        Debug.Log("originalSlot: " + originalSlot.name);

        // Record the texture in the original slot
        originalTexture = draggedImage.texture;
        if (originalTexture != null)
        {
            Debug.Log("originalTexture: " + originalTexture.name);
        }
        else
        {
            Debug.Log("originalTexture: None");
        }

        // Disable raycast to avoid blocking during drag
        if (draggedImage != null)
        {
            draggedImage.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedImage != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)originalSlot.parent, // The parent RectTransform (typically the canvas or a container)
                eventData.position,                // Pointer position
                eventData.pressEventCamera,        // The camera associated with the event
                out Vector2 localPoint             // Output the local point
            );

            // Update the position of the dragged image
            draggedImage.rectTransform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Re-enable raycast
        if (draggedImage != null)
        {
            draggedImage.raycastTarget = true;
        }

        // Check if dropped on a valid slot
        GameObject droppedOn = eventData.pointerCurrentRaycast.gameObject;
        if (droppedOn != null && droppedOn.GetComponent<RawImage>() != null)
        {
            RawImage targetImage = droppedOn.GetComponent<RawImage>();
            Debug.Log("targetImage: " + targetImage.name);
            Transform targetSlot = targetImage.transform.parent;
            Debug.Log("targetSlot: " + targetSlot.name);
            Debug.Log("DroppedOn" + droppedOn);
            Debug.Log("DroppedOn name " + droppedOn.name);


            // Swap textures without moving slots
            Texture targetTexture = targetImage.texture;
            targetImage.texture = originalTexture;
            Debug.Log("OnEndDrag originalTexture" + originalTexture.name);
            draggedImage.texture = targetTexture;
            Debug.Log("OnEndDrag targetTexture" + targetTexture);

            if (droppedOn.name == "RightHandSlot") { //Update Weapons Values
                inventoryManager.AssignToRightHand(originalTexture.name, false);
            }

            // Reset positions of both slots to their hardcoded values
            ResetSlotPosition(originalSlot.name);
            ResetSlotPosition(targetSlot.name);
        }
        else
        {
            // If not dropped on a valid slot, reset to the original slot
            draggedImage.transform.SetParent(originalSlot);
            ResetSlotPosition(originalSlot.name);
        }
    }

    private void ResetSlotPosition(string slotName)
    {
        if (slotPositions.ContainsKey(slotName))
        {
            Transform slotTransform = GameObject.Find(slotName).transform;
            slotTransform.localPosition = slotPositions[slotName]; // Reset to hardcoded position
        }
    }
}
