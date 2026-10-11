using TMPro;
using UnityEngine;

// This is just to show collected resources
public class TempShowResource : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comfortText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI toyText;
    [SerializeField] private TextMeshProUGUI treatText;
    [SerializeField] private TextMeshProUGUI waterText;

    public void OnEnable()
    {
        DataPersistenceManager save = DataPersistenceManager.instance;

        comfortText.SetText("Comfort: {0}", save.GetResourceCount("Comfort"));
        foodText.SetText("Food: {0}", save.GetResourceCount("Food"));
        toyText.SetText("Toy: {0}", save.GetResourceCount("Toy"));
        treatText.SetText("Treat: {0}", save.GetResourceCount("Treat"));
        waterText.SetText("Water: {0}", save.GetResourceCount("Water"));
    }
}
