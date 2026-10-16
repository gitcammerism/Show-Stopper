using System;
using UnityEngine;

// Handles the logic of the level timer and invokes TimeUp when time runs out.

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private LevelData level;

    public float TimeRemaining { get; private set; }
    public float Duration => level.timeLimit;
    public bool IsRunning { get; private set; }

    public event Action TimeUp;

    void Start() => ResetTimer();

    public void SetRemaining(float seconds) => TimeRemaining = Mathf.Max(0f, seconds);

    public void ResetTimer()
    {
        TimeRemaining = level.timeLimit;
        IsRunning = false;
    }

    public void StartTimer()
    {
        if (TimeRemaining > 0f) IsRunning = true;
    }

    public void StopTimer() => IsRunning = false;

    void Update()
    {
        if (!IsRunning) return;

        TimeRemaining -= Time.deltaTime;
        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            IsRunning = false;
            TimeUp?.Invoke();
        }
    }
}
