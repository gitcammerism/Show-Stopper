using TMPro;
using UnityEngine;

// Component/class that handles the floating score for matches.

public class FloatingScore : MonoBehaviour
{
    [SerializeField] private TextMeshPro displayText;
    private TileInstancePool<FloatingScore> _pool;

    // Variables that handle the floating score "animation".
    [SerializeField, Range(0.1f, 1f)]
    private float displayDuration = 0.5f;

    [SerializeField, Range(0f, 4f)]
    private float riseSpeed = 2f;
    private float _age;

    // Displays the floating score.
    public void Show (Vector3 position, int value)
    {
        FloatingScore instance = _pool.GetInstance(this);
        instance._pool = _pool;
        instance.displayText.SetText("{0}", value);
        instance.transform.localPosition = position;
        instance._age = 0f;
    }

    // Executes the floating score "animation" on tick.
    private void Update ()
    {
        _age += Time.deltaTime;
        if (_age > displayDuration) _pool.Recycle(this);
        else
        {
            Vector3 p = transform.localPosition;
            p.y += riseSpeed * Time.deltaTime;
            transform.localPosition = p;
        }
    }
}
