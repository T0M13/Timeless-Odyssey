using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRewind : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerReferences;

    [Header("Rewind Settings")]
    [SerializeField] private float snapshotInterval = 0.1f;
    [SerializeField] private int maxSnapshots = 200;  // Maximum number of snapshots
    [SerializeField] private float snapshotDistanceThreshold = 0.01f;  // Movement threshold for recording
    [SerializeField] private float smoothRewindSpeed = 0.5f;  // How fast we interpolate between positions

    [SerializeField][ShowOnly] private bool isRewinding = false;
    [SerializeField][ShowOnly] private List<TransformSnapshot> snapshots = new List<TransformSnapshot>();
    [SerializeField][ShowOnly] private float snapshotTimer = 0f;

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
        // Check if the player is moving
        if (ShouldRecordSnapshot())
        {
            snapshotTimer += Time.deltaTime;

            // Only record snapshots at intervals when the player is moving
            if (snapshotTimer >= snapshotInterval)
            {
                snapshotTimer = 0f;

                if (snapshots.Count >= maxSnapshots)
                {
                    snapshots.RemoveAt(0);  // Remove oldest snapshot to maintain the limit
                }

                snapshots.Add(new TransformSnapshot(transform.position, transform.rotation));
            }
        }
        else
        {
            // Reset the timer if the player is not moving
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
            isRewinding = true;
            StartCoroutine(SmoothRewindPlayer());
        }
        else if (context.phase == InputActionPhase.Canceled && isRewinding)
        {
            StopRewind();
        }
    }

    private void StartRewind()
    {
        isRewinding = true;
        playerReferences.PlayerBody.isKinematic = true;  // Disable physics during rewind
    }

    private void StopRewind()
    {
        isRewinding = false;
        playerReferences.PlayerBody.isKinematic = false;  // Re-enable physics after rewind
    }

    private IEnumerator SmoothRewindPlayer()
    {
        playerReferences.PlayerBody.isKinematic = true;

        // Rewind for a maximum of 20 seconds or until snapshots run out
        while (snapshots.Count > 0 && isRewinding)
        {
            TransformSnapshot lastSnapshot = snapshots[snapshots.Count - 1];
            snapshots.RemoveAt(snapshots.Count - 1);

            // Smoother rewind by interpolating between the current and the previous snapshot
            float t = 0;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            Vector3 endPos = lastSnapshot.position;
            Quaternion endRot = lastSnapshot.rotation;

            // Smooth transition between snapshots
            while (t < 1f && isRewinding)
            {
                t += Time.deltaTime * smoothRewindSpeed;  // Adjust the interpolation speed
                transform.position = Vector3.Lerp(startPos, endPos, t);
                transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
        }

        StopRewind();  // Stop once the rewind is complete
    }

    [System.Serializable]
    public class TransformSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformSnapshot(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }
}
