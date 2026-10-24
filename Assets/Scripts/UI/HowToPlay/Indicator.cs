using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class Indicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;

    private UnityAction _onClickAction;
    
    
    public void Initialize(UnityAction onClickAction)
    {
        _onClickAction = onClickAction;
        button.onClick.AddListener(_onClickAction);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(_onClickAction);
    }

    private void Reset()
    {
        image = GetComponent<Image>();
    }

    public void Activate()
    {
        ChangeAlpha(1);
    }

    public void Deactivate()
    {
        ChangeAlpha(0);
    }

    private void ChangeAlpha(float targetAlpha)
    {
        Color newColor = image.color;
        newColor.a = targetAlpha;
        image.color = newColor;
    }
}
