using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] public Action<PointerEventData> onDragStartCallback;
    [SerializeField] public Action<PointerEventData> onDragEndCallback;

    [HideInInspector] public Transform parentAfterDrag;
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        onDragStartCallback?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        transform.SetParent(parentAfterDrag);
        canvasGroup.blocksRaycasts = true;
        onDragEndCallback?.Invoke(eventData);
    }
}
