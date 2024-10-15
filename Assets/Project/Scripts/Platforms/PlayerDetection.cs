using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class PlayerDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 5f;

    [Header("Detection Events")]
    public UnityEvent<PlayerReferences> onPlayerEnter;
    public UnityEvent<PlayerReferences> onPlayerExit;

    [SerializeField] private SphereCollider detectionCollider;

    private void Start()
    {
        if (detectionCollider == null)
            detectionCollider = GetComponent<SphereCollider>();
    }

    public void SetDetectionRadius(float newRadius)
    {
        detectionRadius = newRadius;
        UpdateRadius();
    }

    private void UpdateRadius()
    {
        if (detectionCollider != null)
        {
            detectionCollider.radius = detectionRadius;
        }
    }

    private void OnValidate()
    {
        UpdateRadius();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerReferences>() != null)
        {
            onPlayerEnter?.Invoke(other.GetComponent<PlayerReferences>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerReferences>())
        {
            onPlayerExit?.Invoke(other.GetComponent<PlayerReferences>());
        }
    }
}
