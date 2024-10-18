using UnityEngine;
using System.Collections;

public class AutomaticPlatformController : MovablePlatformController
{
    [Header("Automatic Movement Cycle Settings")]
    public float delayBetweenMoves = 2f;

    [SerializeField] protected bool isMovingUp = true;

    protected override void Start()
    {
        base.Start(); 

        InitializePlatformState();

        StartCoroutine(ContinuousMovement());
    }

    private void InitializePlatformState()
    {
        if (startOpen)
        {
            OpenPlatformInitially(); 
        }
        else
        {
            ClosePlatformInitially();
        }
    }

    private void OpenPlatformInitially()
    {
        SetLocalState(openLocalPosition, openLocalRotation, openLocalScale); 
        isMovingUp = false; 
    }

    private void ClosePlatformInitially()
    {
        SetLocalState(closedLocalPosition, closedLocalRotation, closedLocalScale); 
        isMovingUp = true; 
    }

    private IEnumerator ContinuousMovement()
    {
        yield return new WaitForSeconds(.1f);

        while (true)
        {
            if (isMovingUp)
            {
                Open(); 
                yield return new WaitUntil(() => !isOpening); 
            }
            else
            {
                Close(); 
                yield return new WaitUntil(() => !isClosing); 
            }

            isMovingUp = !isMovingUp; 
            yield return new WaitForSeconds(delayBetweenMoves * timeManager.LocalTimeScale);
        }
    }
}
