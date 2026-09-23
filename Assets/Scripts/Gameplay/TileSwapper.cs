using UnityEditor;
using UnityEngine;

/*
 * Last Modified: 09/23/2026 by Chandler Guzman
 * 
 * This class handles the animation of tile-swapping.
 */

[System.Serializable]
public class TileSwapper
{
    [SerializeField, Range(0.1f, 10f)]
    private float duration = 0.25f;

    [SerializeField, Range(0f, 1f)]
    private float maxDepthOffset = 0.5f;

    private Tile _tileA, _tileB;
    private Vector3 _positionA, _positionB;
    private float _progress = -1f;
    private bool _pingPong;

    // Initiates tile swap by storing tile values for Update().
    public float Swap (Tile a, Tile b, bool pingPong)
    {
        _tileA = a;
        _tileB = b;
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
                (_tileA, _tileB) = (_tileB, _tileA);
            }
            else
            {
                // If not ping-ponging, exit.
                _progress = -1f;
                _tileA.transform.localPosition = _positionB;
                _tileB.transform.localPosition = _positionA;
                return;
            }
        }

        // Linearly interpolates tiles to simulate tile-swapping animation.
        float t = _progress / duration;
        float z = Mathf.Sin(Mathf.PI * t) * maxDepthOffset;

        Vector3 p = Vector3.Lerp(_positionA, _positionB, t);
        p.z = -z;
        _tileA.transform.localPosition = p;

        p = Vector3.Lerp(_positionA, _positionB, 1f - t);
        p.z = z;
        _tileB.transform.localPosition = p;
    }
}
