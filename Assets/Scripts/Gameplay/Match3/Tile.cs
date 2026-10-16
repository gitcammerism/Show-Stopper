using System.Collections;
using UnityEngine;

// holds the logic for board tiles basically

public class Tile : MonoBehaviour
{
    public TileType type;
    public Cell cell;   // cell this tile occupies

    [SerializeField] private float moveSpeed = 8f;

    // for the purpose of tiles not spawning in above the grid awkwardly
    void Awake()
    {
        // foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        // {
        //     sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        // }
    }

    // initializes tile; basically a constructor
    public void Init(TileType type, Cell cell)
    {
        this.type = type;
        this.cell = cell;
    }

    // moves tile to a world position and yields until it arrives
    public IEnumerator MoveTo(Vector3 target, float duration = 0.25f)
    {
        Vector3 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float eased = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(start, target, eased);
            yield return null;
        }
        transform.position = target;
    }

    // animates tile disappearing
    public IEnumerator Clear(float duration = 0.2f)
    {
        Vector3 startScale = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        // pop animation/particles/vfx go here
        Destroy(gameObject);
        yield break;
    }
}
