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
    private bool isDragging = false;

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

        // If it’s empty, bail out and mark that we’re not dragging.
        if (draggedImage.texture == null
            || draggedImage.texture == inventoryManager.transparentImg)
        {
            isDragging = false;
            return;
        }

        // Otherwise we really are dragging now:
        isDragging      = true;
        originalSlot    = transform;
        originalTexture = draggedImage.texture;
        draggedImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;    // <-- guard against dragging nothing

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
        if (!isDragging) return;    // <-- guard against dragging nothing
        isDragging = false;

        // Re-enable raycast
        draggedImage.raycastTarget = true;

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

            if (droppedOn.name == "LeftHandSlot") { //Update Shield Armor Values
                inventoryManager.AssignToLeftHand(originalTexture.name);
            }

            // Reset positions of both slots to their hardcoded values
            ResetSlotPosition(originalSlot.name);
            ResetSlotPosition(targetSlot.name);
        } else {
            // If not dropped on a valid slot, reset to the original slot
            draggedImage.transform.SetParent(originalSlot);
            ResetSlotPosition(originalSlot.name);
        }
        // This covers the case where an non shield item on left hand is dragged and dropped on a Shield and the shield ends in left hand, the method above only don't wont
        inventoryManager.CheckIfLeftHandHasShield();
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
