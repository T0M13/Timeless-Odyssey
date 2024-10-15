using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindProxy : MonoBehaviour
{
    [SerializeField] private GameObject originalObject;
    [SerializeField] private bool isRewinding = false;
    [SerializeField] private float rewindDuration = 30f;
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float rewindSpeed = .03f; 
    [SerializeField] private int maxSnapshots = 30000;

    [System.NonSerialized]
    [SerializeField] private List<TransformSnapshot> snapshots = new List<TransformSnapshot>();
    [SerializeField] private bool isDestroyed = false;

    // To track if all objects are done rewinding
    private static List<RewindProxy> allRewindProxies = new List<RewindProxy>();

    private Vector3 lastRecordedPosition;
    private Quaternion lastRecordedRotation;
    private float snapshotDistanceThreshold = 0.01f;
    private float recordTimer = 0f; // Tracks time for recording
    private float rewindTimer = 0f; // Tracks time for rewinding

    private Rigidbody rb;
    private Collider objCollider;

    public GameObject OriginalObject { get => originalObject; set => originalObject = value; }
    public bool IsRewinding { get => isRewinding; set => isRewinding = value; }

    private void Start()
    {
        lastRecordedPosition = OriginalObject.transform.position;
        lastRecordedRotation = OriginalObject.transform.rotation;

        rb = OriginalObject.GetComponent<Rigidbody>();
        objCollider = OriginalObject.GetComponent<Collider>();

        allRewindProxies.Add(this);  // Track this instance for synchronization
    }

    private void Update()
    {
        if (IsRewinding && !isDestroyed)
        {
            RewindObjects();
        }
        if (!IsRewinding)
        {
            RecordObjects();
        }
    }

    private void RecordObjects()
    {
        // Record interval logic
        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            recordTimer = 0f;

            if (ShouldRecordSnapshot())
            {
                if (snapshots.Count >= maxSnapshots)
                {
                    snapshots.RemoveAt(0); // Maintain snapshot limit
                }

                snapshots.Add(new TransformSnapshot(OriginalObject.transform.position, OriginalObject.transform.rotation));
                lastRecordedPosition = OriginalObject.transform.position;
                lastRecordedRotation = OriginalObject.transform.rotation;
            }
        }
    }

    private void RewindObjects()
    {
        // Disable physics and colliders during rewind
        if (rb != null) rb.isKinematic = true;
        if (objCollider != null) objCollider.enabled = false;

        rewindTimer += Time.deltaTime;

        if (rewindTimer >= rewindSpeed && snapshots.Count > 0)
        {
            rewindTimer = 0f;

            TransformSnapshot snapshot = snapshots[snapshots.Count - 1];
            OriginalObject.transform.position = snapshot.position;
            OriginalObject.transform.rotation = snapshot.rotation;
            snapshots.RemoveAt(snapshots.Count - 1);
        }

        // When rewind finishes, check if all objects have finished before re-enabling physics
        if (snapshots.Count == 0)
        {
            IsRewinding = false;
            CheckAllRewindsComplete();
        }
    }

    private bool ShouldRecordSnapshot()
    {
        return Vector3.Distance(OriginalObject.transform.position, lastRecordedPosition) > snapshotDistanceThreshold
               || Quaternion.Angle(OriginalObject.transform.rotation, lastRecordedRotation) > 0.01f;
    }

    public void ObjectDestroyed()
    {
        isDestroyed = true;
        OriginalObject.SetActive(false);
    }

    public void RestoreObject()
    {
        if (snapshots.Count > 0)
        {
            TransformSnapshot lastSnapshot = snapshots[snapshots.Count - 1];
            OriginalObject.SetActive(true);
            OriginalObject.transform.position = lastSnapshot.position;
            OriginalObject.transform.rotation = lastSnapshot.rotation;
            isDestroyed = false;
        }
    }

    private void CheckAllRewindsComplete()
    {
        // Check if all rewind proxies have finished rewinding
        foreach (var proxy in allRewindProxies)
        {
            if (proxy.IsRewinding) return; // If any object is still rewinding, don't stop yet
        }

        // If we reach this point, all objects have finished rewinding
        EnablePhysicsForAll();
    }

    private void EnablePhysicsForAll()
    {
        // Re-enable physics and colliders for all objects
        foreach (var proxy in allRewindProxies)
        {
            if (proxy.rb != null) proxy.rb.isKinematic = false;
            if (proxy.objCollider != null) proxy.objCollider.enabled = true;
        }

        StartCoroutine(DestroyAfterRewind());
    }

    private IEnumerator DestroyAfterRewind()
    {
        yield return new WaitForSeconds(rewindDuration);
        if (!OriginalObject.activeInHierarchy)
        {
            Destroy(gameObject);
        }
    }

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
