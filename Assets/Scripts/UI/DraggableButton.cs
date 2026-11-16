using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private bool _isDragging;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = transform.root.GetComponent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData) => _isDragging = true;
    public void OnEndDrag(PointerEventData data) => _isDragging = false;

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public bool IsDragging() => _isDragging;
}
