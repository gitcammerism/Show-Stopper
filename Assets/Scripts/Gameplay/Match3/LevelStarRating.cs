using UnityEngine;

public class LevelStarRating : MonoBehaviour
{
    [Header("Score Requirements")]
    [SerializeField] private int twoStarScore;
    [SerializeField] private int threeStarScore;

    // Calculate stars based on score
    public int CalculateStars(int score)
    {
        if (score >= threeStarScore)
        {
            return 3;
        }

        if (score >= twoStarScore)
        {
            return 2;
        }

        // Completing level always awards at least one star
        return 1;
    }
}