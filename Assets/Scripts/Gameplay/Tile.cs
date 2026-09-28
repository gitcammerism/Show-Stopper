using UnityEngine;

// This is a script that handles the spawning and despawning of the assigned tile.

public class Tile : MonoBehaviour
{
    // 0 = inactive/not started, 1 = active/complete
    [SerializeField, Range(0f, 1f)]
    private float disappearDuration = 0.25f;
    private float _disappearProgress;
    private FallingState _falling;

    private TileInstancePool<Tile> _pool;

    // Gets an instance of itself, assigns self a pool, and places tile at given position
    public Tile Spawn (Vector3 position)
    {
        Tile instance = _pool.GetInstance(this);
        instance._pool = _pool;
        instance.transform.localPosition = position;
        instance.transform.localScale = Vector3.one;

        // Setup for the disappearing tile animations.
        instance._disappearProgress = -1f;
        instance._falling.progress = -1f;
        instance.enabled = false;

        return instance;
    }

    // Tile handles disappearing and falling "animations" in update.
    private void Update()
    {
        if (_disappearProgress >= 0f)
        {
            _disappearProgress += Time.deltaTime;
            if (_disappearProgress >= disappearDuration)
            {
                Despawn();
                return;
            }

            transform.localScale = Vector3.one * (1f - _disappearProgress / disappearDuration);
        }

        if (_falling.progress >= 0f)
        {
            Vector3 position = transform.localPosition;
            _falling.progress += Time.deltaTime;

            if (_falling.progress >= _falling.duration)
            {
                _falling.progress = -1f;
                position.y = _falling.toY;
                enabled = _disappearProgress >= 0f;
            }
            else
            {
                position.y = Mathf.Lerp(_falling.fromY, _falling.toY, _falling.progress / _falling.duration);
            }

            transform.localPosition = position;
        }
    }

    // Recycles the given tile.
    public void Despawn () => _pool.Recycle(this);

    // Resets disappearing progress to 0 and enables tile.
    public float Disappear ()
    {
        _disappearProgress = 0f;
        enabled = true;
        return disappearDuration;
    }

    // Sets up the falling state struct of the tile for the falling animation.
    public float Fall (float toY, float speed)
    {
        _falling.fromY = transform.localPosition.y;
        _falling.toY = toY;
        _falling.duration = (_falling.fromY - toY) / speed;
        _falling.progress = 0f;
        enabled = true;
        return _falling.duration;
    }
}
