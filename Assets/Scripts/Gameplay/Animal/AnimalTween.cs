using DG.Tweening;
using UnityEngine;

public class AnimalTween : MonoBehaviour
{
    private Vector3 startingScale;
    private Vector3 endScale;
    private Vector3 cameraPos;
    private Vector3 currentPos;
    private Vector3 targetPosition = new Vector3(0, 0, 0); // Set your target position here 

    private float moveDuration = .5f; // Duration of the movement in seconds 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraPos = Camera.main.WorldToScreenPoint(transform.position);

        startingScale = transform.localScale;
        currentPos = transform.position;

        endScale = new Vector3(startingScale.x, startingScale.y / 1.5f, startingScale.z);
        targetPosition = new Vector3(currentPos.x + 2f, currentPos.y, currentPos.z); // Set your target position here
        transform.localScale = endScale;
        // scale !
        transform.DOScale(startingScale, moveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutCubic);
        // jump !
        transform.DOMove(new Vector3(currentPos.x, currentPos.y + 1f, currentPos.z), moveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutCubic);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraPos.x < 0 || cameraPos.x > Screen.width || cameraPos.y < 0 || cameraPos.y > Screen.height)
        {
            DOTween.Pause(transform);   
        }
    }
}
