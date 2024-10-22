using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRewind : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerReferences;

    [Header("Rewind Settings")]
    [SerializeField] private float snapshotInterval = 0.1f;
    [SerializeField] private int maxSnapshots = 200;
    [SerializeField] private float snapshotDistanceThreshold = 0.01f;
    [SerializeField] private float minRewindSpeed = 1f;
    [SerializeField] private float maxRewindSpeed = 5f;
    [SerializeField] private float rewindSpeedMultiplier = 1.5f;
    [SerializeField] private int smoothOutSnapshots = 5;

    [SerializeField][ShowOnly] private bool isRewinding = false;
    [SerializeField][ShowOnly] private List<TransformSnapshot> snapshots = new List<TransformSnapshot>();
    [SerializeField][ShowOnly] private float snapshotTimer = 0f;
    [SerializeField][ShowOnly] private float currentRewindSpeed;

    public event Action<bool> OnRewindStateChanged;  // Event to notify whether we are rewinding or not

    private void Start()
    {
        if (playerReferences == null)
            playerReferences = GetComponent<PlayerReferences>();
    }

    private void Update()
    {
        if (!isRewinding)
        {
            RecordSnapshots();
        }
    }

    private void RecordSnapshots()
    {
        if (ShouldRecordSnapshot())
        {
            snapshotTimer += Time.deltaTime;

            if (snapshotTimer >= snapshotInterval)
            {
                snapshotTimer = 0f;

                if (snapshots.Count >= maxSnapshots)
                {
                    snapshots.RemoveAt(0);
                }

                // Record position only (not rotation)
                snapshots.Add(new TransformSnapshot(transform.position));
            }
        }
        else
        {
            snapshotTimer = 0f;
        }
    }

    private bool ShouldRecordSnapshot()
    {
        return snapshots.Count == 0 || Vector3.Distance(transform.position, snapshots[snapshots.Count - 1].position) > snapshotDistanceThreshold;
    }

    public void OnRewind(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && snapshots.Count > 0)
        {
            StartRewind();
        }
        else if (context.phase == InputActionPhase.Canceled && isRewinding)
        {
            StopRewind();
        }
    }

    private void StartRewind()
    {
        isRewinding = true;
        currentRewindSpeed = minRewindSpeed;
        playerReferences.PlayerBody.isKinematic = true;

        OnRewindStateChanged?.Invoke(isRewinding);  // Notify that rewinding started

        StartCoroutine(SmoothRewindPlayer());
    }

    private void StopRewind()
    {
        isRewinding = false;
        playerReferences.PlayerBody.isKinematic = false;

        OnRewindStateChanged?.Invoke(isRewinding);  // Notify that rewinding stopped
    }

    private IEnumerator SmoothRewindPlayer()
    {
        playerReferences.PlayerBody.isKinematic = true;

        while (snapshots.Count > 0 && isRewinding)
        {
            TransformSnapshot lastSnapshot = snapshots[snapshots.Count - 1];
            snapshots.RemoveAt(snapshots.Count - 1);

            float t = 0;
            Vector3 startPos = transform.position;

            // Only rewinding the position
            Vector3 endPos = lastSnapshot.position;

            if (snapshots.Count > smoothOutSnapshots)
            {
                currentRewindSpeed = Mathf.Lerp(currentRewindSpeed, maxRewindSpeed * rewindSpeedMultiplier, Time.deltaTime);
            }
            else
            {
                currentRewindSpeed = Mathf.Lerp(currentRewindSpeed, 0f, Time.deltaTime * (smoothOutSnapshots - snapshots.Count) / smoothOutSnapshots);
            }

            while (t < 1f && isRewinding)
            {
                t += Time.deltaTime * currentRewindSpeed;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
        }

        StopRewind();
    }

    [System.Serializable]
    public class TransformSnapshot
    {
        public Vector3 position;

        public TransformSnapshot(Vector3 pos)
        {
            position = pos;
        }
    }
}
