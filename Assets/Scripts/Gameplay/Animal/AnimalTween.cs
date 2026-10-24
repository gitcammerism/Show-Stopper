using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AnimalTween : MonoBehaviour
{
    // This script handles the animal movement around the hub lvl
    // Instead of regular transforms, the animals use RectTransforms in order to be able to move around the canvas.
    // Movement/position values need to be generally larger as a result (ex: instead of 1, 100)

    private Vector3 startingScale;
    private Vector3 endScale;
    private Vector3 animalPos;
    private Vector3 cameraPos;
    private Vector3 currentPos;
    private Vector2 targetPosition;

    private RectTransform rectTransform;
    private RectTransform canvasRect;

    private float xDisplacement;
    private float yDisplacement;
    private float displacementRange = 200;

    private float moveDuration = .5f; // Duration of the movement in seconds 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        // OLD
        //cameraPos = Camera.main.WorldToScreenPoint(transform.position);
        //currentPos = rectTransform.anchoredPosition;

        startingScale = new Vector3(rectTransform.localScale.x / 3f, rectTransform.localScale.y / 3f, rectTransform.localScale.z);
        xDisplacement = Random.Range(-displacementRange, displacementRange);
        yDisplacement = Random.Range(-displacementRange, displacementRange);

        endScale = new Vector3(startingScale.x, startingScale.y / 1.5f, startingScale.z);
        targetPosition = new Vector3(currentPos.x + 100f, currentPos.y, currentPos.z); // Set your target position here
        rectTransform.localScale = endScale;
        // scale !
        rectTransform.DOScale(startingScale, moveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutQuint);
        // jump !
        // transform.DOJump(new Vector3(currentPos.x, currentPos.y + 1f, currentPos.z), 1f, 1, moveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutCubic);

        StartCoroutine(AnimalMovement());


    }


    IEnumerator AnimalMovement()
    {
        while (true)
        {
            // gets animal's local canvas position
            Vector3 canvasLocalPos = canvasRect.InverseTransformPoint(rectTransform.position);

            // if the animal isn't in frame, pick new position
            if (!canvasRect.rect.Contains(canvasLocalPos))
            {
                float randomXPos = Random.Range(canvasRect.rect.xMin + 50f, canvasRect.rect.xMax - 50f);
                float randomYPos = Random.Range(canvasRect.rect.yMin + 50f, canvasRect.rect.yMax - 50f);

                Vector3 worldTarget = canvasRect.TransformPoint(new Vector3(randomXPos, randomYPos, canvasLocalPos.z));
                Vector3 localTarget = rectTransform.parent.InverseTransformPoint(worldTarget);

                targetPosition = localTarget;
            }
            else
            {
                targetPosition = rectTransform.anchoredPosition + new Vector2(xDisplacement, yDisplacement);
            }

            // OLD LOGIC - Kept for safekeeping just in case

            //cameraPos = Camera.main.WorldToScreenPoint(transform.position);

            // If the animal goes off-screen, choose a new target
            //if (cameraPos.x - 1 < 0 || cameraPos.x + 1 > Screen.width || cameraPos.y - 1 < 0 || cameraPos.y + 1 > Screen.height)
            //{
            //    // Pick a random screen position - probably need to make less drastic
            //    Vector3 randomScreenPos = new Vector3(
            //        Random.Range(50, Screen.width - 50),
            //        Random.Range(50, Screen.height - 50),
            //        cameraPos.z
            //    );

            //    // Convert the screen position to a world position
            //    RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)rectTransform.parent, randomScreenPos, Camera.main, out Vector2 localPoint);
            //    targetPosition = localPoint;
            //}
            //else
            //{
            //    // Continue moving in the current direction
            //    currentPos = rectTransform.anchoredPosition;

            //    targetPosition = new Vector3(currentPos.x + xDisplacement, currentPos.y + yDisplacement, currentPos.z);
            //}

            yield return rectTransform.DOJumpAnchorPos(targetPosition, 200f, 1, moveDuration * 2).WaitForCompletion();

            // Pick a new direction for the next movement
            xDisplacement = Random.Range(-displacementRange, displacementRange);
            yDisplacement = Random.Range(-displacementRange, displacementRange);
        }
    }
}
