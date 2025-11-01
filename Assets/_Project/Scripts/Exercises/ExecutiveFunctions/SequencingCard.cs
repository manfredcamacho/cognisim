using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class SequenceCard : MonoBehaviour
{
    public TextMeshProUGUI stepText;
    public int correctIndex;
    public Action<SequenceCard, PointerEventData> OnCardDragged;

    private DraggableItem draggableItem;

    private void Awake()
    {
        draggableItem = GetComponent<DraggableItem>();
        draggableItem.onDragEndCallback += HandleDragEnd;
    }
    private void HandleDragEnd(PointerEventData eventData)
    {
        OnCardDragged?.Invoke(this, eventData);
    }
    private void OnDestroy()
    {
        if (draggableItem != null)
        {
            draggableItem.onDragEndCallback -= HandleDragEnd;
        }
    }
}