using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContentManager : MonoBehaviour, IEndDragHandler
{
    [Header("Parts Setup")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentBoxHorizontal;
    
    [Header("Content")]
    [SerializeField] private List<GameObject> pages = new List<GameObject>();

    [Header("Navigation Indicators")]
    [SerializeField] private Indicator indicator;
    [SerializeField] private Transform navContainer;
    private List<Indicator> _indicators = new List<Indicator>();

    [Header("Scroll Animation")]
    [SerializeField, Range(0.25f, 1f)] private float duration = 0.5f;
    [SerializeField] private AnimationCurve easeCurve;

    private int _currentIndex = 0;
    private Coroutine _scrollCoroutine;

    private void Reset()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var page in pages)
        {
            var newIndicator = Instantiate(indicator, navContainer);
            newIndicator.Initialize(() => ScrollToSpecificIndex(pages.IndexOf(page)));
            _indicators.Add(newIndicator);
        }

        _indicators[0].Activate();
        
    }

    public void ClearCurrentIndex()
    {
        _indicators[_currentIndex].Deactivate();
    }

    private void ScrollToSpecificIndex(int index)
    {
        ClearCurrentIndex();
        ScrollTo(index);
    }

    public void ScrollToNext()
    {
        ClearCurrentIndex();
        _currentIndex = (_currentIndex + 1) % pages.Count;
        ScrollTo(_currentIndex);
    }

    public void ScrollToPrevious()
    {
        ClearCurrentIndex();
        _currentIndex = (_currentIndex - 1 + pages.Count) % pages.Count;
        ScrollTo(_currentIndex);
    }

    private void ScrollTo(int index)
    {
        _currentIndex = index;
        float targetHorizontalPosition = (float)_currentIndex / (pages.Count - 1);

        if(_scrollCoroutine != null)
        {
            StopCoroutine(_scrollCoroutine);
        }

        _scrollCoroutine = StartCoroutine(LerpToPos(targetHorizontalPosition));

        _indicators[_currentIndex].Activate();
    }

    private IEnumerator LerpToPos(float targetHorizontalPosition)
    {
        float elapsedTime = 0;
        float initialPos = scrollRect.horizontalNormalizedPosition;

        if (duration > 0)
        {
            while (elapsedTime <= duration)
            {
                float easeValue = easeCurve.Evaluate(elapsedTime / duration);
                float newPosition = Mathf.Lerp(initialPos, targetHorizontalPosition, easeValue);

                scrollRect.horizontalNormalizedPosition = newPosition;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        scrollRect.horizontalNormalizedPosition = targetHorizontalPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.delta.x != 0)
        {
            if (eventData.delta.x > 0)
            {
                ScrollToPrevious();
            }
            else if (eventData.delta.x < 0)
            {
                ScrollToNext();
            }
            else
            {
                ScrollToSpecificIndex(_currentIndex);
            }
        }
    }

}
