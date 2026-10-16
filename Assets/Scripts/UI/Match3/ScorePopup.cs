using System.Collections;
using TMPro;
using UnityEngine;

// popup prefab script for scores that appear after a match

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float riseDistance = 0.8f;

    public void Show(int points, int cascadeStep)
    {
        label.text = cascadeStep > 1 ? $"+{points}\n<size=70%>x{cascadeStep} combo</size>" : $"+{points}";
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector3 start = transform.position;
        Vector3 ends = start + Vector3.up * riseDistance;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / lifetime;

            // quick pop-up, then settle
            transform.position = Vector3.Lerp(start, ends, t);
            transform.localScale = Vector3.one * Mathf.Lerp(0.6f, 1f, Mathf.Clamp01(t * 6f));

            // fade out over second half
            label.alpha = 1f - Mathf.Clamp01((t - 0.5f) * 2f);

            yield return null;
        }

        Destroy(gameObject);
    }
}
