using UnityEngine;
using UnityEngine.EventSystems;

public class DroppableSlot: MonoBehaviour, IDropHandler
{
    [SerializeField] public int slotIndex;
    [SerializeField] public bool canReplaceItem = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount > 0 && !canReplaceItem)
        {
            return; 
        }

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (transform.childCount > 0)
        {
            Transform existingItem = transform.GetChild(0);
            existingItem.SetParent(draggableItem.parentAfterDrag);
        }

        draggableItem.parentAfterDrag = transform;

        
    }
}