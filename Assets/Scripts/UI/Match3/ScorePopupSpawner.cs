using UnityEngine;

// spawns ScorePopup prefab

public class ScorePopupSpawner : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ScorePopup popupPrefab;

    void OnEnable() => scoreManager.RunScored += Spawn;
    void OnDisable() => scoreManager.RunScored -= Spawn;

    private void Spawn(Vector3 pos, int points, int cascadeStep)
    {
        // random offset added to position to avoid overlapping scores
        var popup = Instantiate(popupPrefab, pos + new Vector3(Random.Range(-0.2f, 0.2f), 0, 0), Quaternion.identity);
        popup.Show(points, cascadeStep);
    }
}
