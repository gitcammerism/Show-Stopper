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

    private float xDisplacement;
    private float yDisplacement;    

    private float moveDuration = .5f; // Duration of the movement in seconds 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraPos = Camera.main.WorldToScreenPoint(transform.position);
        startingScale = transform.localScale;
        currentPos = transform.position;
        xDisplacement = 1f;
        yDisplacement = 1f;

        endScale = new Vector3(startingScale.x, startingScale.y / 1.5f, startingScale.z);
        targetPosition = new Vector3(currentPos.x + 1f, currentPos.y, currentPos.z); // Set your target position here
        transform.localScale = endScale;
        // scale !
        transform.DOScale(startingScale, moveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutQuint);
        // jump !
        // transform.DOJump(new Vector3(currentPos.x, currentPos.y + 1f, currentPos.z), 1f, 1, moveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutCubic);

        StartCoroutine(AnimalMovement());


    }


    IEnumerator AnimalMovement()
    {
        while (true)
        {
            cameraPos = Camera.main.WorldToScreenPoint(transform.position);

            // If the animal goes off-screen, choose a new target
            if (cameraPos.x-1 < 0 || cameraPos.x+1 > Screen.width || cameraPos.y-1 < 0 || cameraPos.y+1 > Screen.height)
            {
                // Pick a random screen position - probably need to make less drastic
                Vector3 randomScreenPos = new Vector3(
                    Random.Range(50, Screen.width - 50),
                    Random.Range(50, Screen.height - 50),
                    cameraPos.z
                );

                // Convert the screen position to a world position
                targetPosition = Camera.main.ScreenToWorldPoint(randomScreenPos);
            }
            else
            {
                // Continue moving in the current direction
                currentPos = transform.position;

                targetPosition = new Vector3(currentPos.x + xDisplacement, currentPos.y + yDisplacement, currentPos.z);
            }

            yield return transform.DOJump(targetPosition, 1f, 1, moveDuration * 2).WaitForCompletion();

            // Pick a new direction for the next movement
            xDisplacement = Random.Range(-1f, 1f);
            yDisplacement = Random.Range(-1f, 1f);
        }
    }
}
