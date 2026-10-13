using UnityEngine;

// This class handles the linear interpolation ("animation") of tile-swapping.

[System.Serializable]
public class TileSwapper
{
    [SerializeField, Range(0.1f, 10f)]
    private float duration = 0.25f;

    [SerializeField, Range(0f, 1f)]
    private float maxDepthOffset = 0.5f;

    private OldTile _oldTileA, _oldTileB;
    private Vector3 _positionA, _positionB;
    private float _progress = -1f;
    private bool _pingPong;

    // Initiates tile swap by storing tile values for Update().
    public float Swap (OldTile a, OldTile b, bool pingPong)
    {
        _oldTileA = a;
        _oldTileB = b;
        _positionA = a.transform.localPosition;
        _positionB = b.transform.localPosition;

        this._pingPong = pingPong;
        _progress = 0f;

        return _pingPong ? 2f * duration : duration;
    }

    public void Update() 
    {
        // If progress is -1, swapper is inactive.
        if (_progress < 0f) return;

        // While progress is active, tiles are swapped.
        _progress += Time.deltaTime;
        if (_progress >= duration)
        {
            if (_pingPong)
            {
                // If ping-ponging, swap tiles and continue.
                _progress -= duration;
                _pingPong = false;
                (_oldTileA, _oldTileB) = (_oldTileB, _oldTileA);
            }
            else
            {
                // If not ping-ponging, exit.
                _progress = -1f;
                _oldTileA.transform.localPosition = _positionB;
                _oldTileB.transform.localPosition = _positionA;
                return;
            }
        }

        // Linearly interpolates tiles to simulate tile-swapping animation.
        float t = _progress / duration;
        float z = Mathf.Sin(Mathf.PI * t) * maxDepthOffset;

        Vector3 p = Vector3.Lerp(_positionA, _positionB, t);
        p.z = -z;
        _oldTileA.transform.localPosition = p;

        p = Vector3.Lerp(_positionA, _positionB, 1f - t);
        p.z = z;
        _oldTileB.transform.localPosition = p;
    }
}
