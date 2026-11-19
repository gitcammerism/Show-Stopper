using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private const int NoPointer = int.MinValue;
    private RectTransform _rectTransform;
    private RectTransform _parentRect;
    private Canvas _canvas;
    private Vector2 _grabOffset;
    private int _activePointerID = NoPointer;
    private bool _isDragging;

    private Coroutine _moveCoroutine;
    private Vector2 _originalPos;
    [SerializeField] private float moveDuration = 0.5f;

    private Animal currentAnimal;

    private void Awake()
    {
        _rectTransform = (RectTransform)transform;
        _parentRect = (RectTransform)_rectTransform.parent;
        _canvas = transform.root.GetComponent<Canvas>().rootCanvas;
        _originalPos = _rectTransform.anchoredPosition;
    }

    private void OnDisable()
    {
        currentAnimal = null;
    }

    private Camera EventCam => _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

    // converts screen pixels into parent rect transform local space depending on render mode
    private bool TryGetLocalPoint(PointerEventData e, out Vector2 local) =>
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, e.position, EventCam, out local);

    public void OnBeginDrag(PointerEventData e)
    {
        if (_activePointerID != NoPointer) return; // means something is already being dragged
        if (!TryGetLocalPoint(e, out var local)) return;
        if (_moveCoroutine != null) return; // ensures nothing can be dragged while lerp is running

        _activePointerID = e.pointerId;
        _grabOffset = (Vector2)_rectTransform.localPosition - local;
    }

    public void OnDrag(PointerEventData e)
    {
        if (e.pointerId != _activePointerID) return; // ignore extra finger input
        if (!TryGetLocalPoint(e, out var local)) return;

        Vector2 p = local + _grabOffset;
        _rectTransform.localPosition = new Vector3(p.x, p.y, _rectTransform.localPosition.z);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (e.pointerId == _activePointerID) _activePointerID = NoPointer;

        if (currentAnimal == null)
        {
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(e, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.TryGetComponent<Animal>(out var animal))
                {
                    Debug.Log("Dropped item on: " + animal.name);
                    currentAnimal = animal;
                    break;
                }
            }
        }

        if (_moveCoroutine == null) _moveCoroutine = StartCoroutine(ReturnToSlot(moveDuration));
    }

    private IEnumerator ReturnToSlot(float duration)
    {
        Vector2 endPos = _rectTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = Mathf.SmoothStep(0f, 1f, t);
            _rectTransform.anchoredPosition = Vector2.Lerp(endPos, _originalPos, t);
            yield return null;
        }

        _rectTransform.anchoredPosition = _originalPos;
        _moveCoroutine = null;
    }
}
