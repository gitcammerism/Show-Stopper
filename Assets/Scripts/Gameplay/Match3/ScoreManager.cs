using System;
using System.Collections.Generic;
using UnityEngine;

// handles score for the game yay

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int pointsPerTile = 10;
    [SerializeField] private int[] lengthBonus = { 0, 20, 60, 120 }; // bonus for 3, 4, 5, and 6+-matches respectively
    [SerializeField] private float cascadeBonus = 0.5f; // +50% per extra cascade step
    public int pointsPerSecond = 50; // points gained per second left on the timer

    public int Score { get; private set; }

    public event Action<int> ScoreChanged;
    public event Action<Vector3, int, int> RunScored; // position, points, cascade step

    // basic add point method
    public void AddPoints(int points)
    {
        Score += points;
        ScoreChanged?.Invoke(Score);
    }

    // calculates score based off of match size
    public void ScoreRuns(List<List<Cell>> runs, int cascadeStep)
    {
        float multiplier = 1f + cascadeBonus * (cascadeStep - 1);

        foreach (var run in runs)
        {
            int len = run.Count;
            int bonus = lengthBonus.Length > 0
                ? lengthBonus[Mathf.Clamp(len - 3, 0, lengthBonus.Length - 1)]
                : 0;

            int points = Mathf.RoundToInt((len * pointsPerTile + bonus) * multiplier);
            Score += points;

            // popup position is at the middle of the run
            Vector3 center = Vector3.zero;
            foreach (var c in run) center += c.worldPos;
            center /= len;

            RunScored?.Invoke(center, points, cascadeStep);
        }

        ScoreChanged?.Invoke(Score);
    }

    // adds bonus to score for however much time was left on the timer
    public int AddTimeBonus(float secondsLeft)
    {
        int seconds = Mathf.CeilToInt(Mathf.Max(0f, secondsLeft));
        int bonus = seconds * pointsPerSecond;

        Score += bonus;
        ScoreChanged?.Invoke(Score);
        return bonus;
    }

    public void ResetScore()
    {
        Score = 0;
        ScoreChanged?.Invoke(Score);
    }
}
