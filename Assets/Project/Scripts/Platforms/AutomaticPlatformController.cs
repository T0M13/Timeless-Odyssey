using UnityEngine;
using System.Collections;

public class AutomaticPlatformController : MovablePlatformController
{
    [Header("Automatic Movement Cycle Settings")]
    public float delayBetweenMoves = 2f; 

    private bool isMovingUp = true;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(ContinuousMovement());
    }

    private IEnumerator ContinuousMovement()
    {
        while (true)
        {
            if (isMovingUp)
            {
                Open();  // Start opening
                yield return new WaitUntil(() => !isOpening);  // Wait until fully opened
            }
            else
            {
                Close(); // Start closing
                yield return new WaitUntil(() => !isClosing);  // Wait until fully closed
            }

            isMovingUp = !isMovingUp; // Toggle direction

            // Apply the delay between moves
            yield return new WaitForSeconds(delayBetweenMoves * timeManager.LocalTimeScale);
        }
    }
}
