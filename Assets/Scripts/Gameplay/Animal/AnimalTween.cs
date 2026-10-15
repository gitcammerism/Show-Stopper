using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AnimalTween : MonoBehaviour
{
    private Vector3 startingScale;
    private Vector3 endScale;
    private Vector3 cameraPos;
    private Vector3 currentPos;
    private Vector3 targetPosition = new Vector3(0, 0, 0); // Set your target position here 

    private float displacement;

    private float moveDuration = .5f; // Duration of the movement in seconds 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraPos = Camera.main.WorldToScreenPoint(transform.position);
        startingScale = transform.localScale;
        currentPos = transform.position;
        displacement = 1f;

        endScale = new Vector3(startingScale.x, startingScale.y / 1.5f, startingScale.z);
        targetPosition = new Vector3(currentPos.x + 1f, currentPos.y, currentPos.z); // Set your target position here
        transform.localScale = endScale;
        // scale !
        transform.DOScale(startingScale, moveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutQuint);
        // jump !
        // transform.DOJump(new Vector3(currentPos.x, currentPos.y + 1f, currentPos.z), 1f, 1, moveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutCubic);

        StartCoroutine(AnimalMovement());
        // transform.DOJump(targetPosition, 1f, 1, moveDuration * 2);

        Sequence mySequence = DOTween.Sequence();
        // mySequence.Append(transform.DOMove(new Vector3(currentPos.x, currentPos.y + 1f, currentPos.z), moveDuration).SetEase(Ease.OutCubic));
        // mySequence.Insert(0, transform.DOMove(new Vector3(currentPos.x + 1f, currentPos.y, currentPos.z), mySequence.Duration()).SetEase(Ease.OutCubic));

    }


    IEnumerator AnimalMovement()
    {
        while (!(cameraPos.x < 0 || cameraPos.x > Screen.width || cameraPos.y < 0 || cameraPos.y > Screen.height))
        {
            cameraPos = Camera.main.WorldToScreenPoint(transform.position);
            currentPos = transform.position;
            targetPosition = new Vector3(currentPos.x + displacement, currentPos.y + displacement/2, currentPos.z);
            yield return transform.DOJump(targetPosition, 1f, 1, moveDuration * 2).WaitForCompletion();
            // displacement *= -1; // Reverse the direction of movement
            // StartCoroutine(AnimalMovement()); // Restart the coroutine for continuous movement  
            yield return null;
            // Move the animal back to the original position
            // yield return transform.DOMove(currentPos, moveDuration).WaitForCompletion();
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (cameraPos.x < 0 || cameraPos.x > Screen.width || cameraPos.y < 0 || cameraPos.y > Screen.height)
        {
            DOTween.Pause(transform);   
        }
        else
        {

        }
    }
}
