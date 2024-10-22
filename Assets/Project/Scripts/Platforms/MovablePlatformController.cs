using UnityEngine;

[RequireComponent(typeof(ObjectTimeManager))]
public class MovablePlatformController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected ObjectTimeManager timeManager;

    [Header("Local Position")]
    [SerializeField] protected Vector3 closedLocalPosition;
    [SerializeField] protected Vector3 openLocalPosition;

    [Header("Local Rotation")]
    [SerializeField] protected Quaternion closedLocalRotation = Quaternion.identity;
    [SerializeField] protected Quaternion openLocalRotation = Quaternion.identity;

    [Header("Local Scale")]
    [SerializeField] protected Vector3 closedLocalScale = Vector3.one;
    [SerializeField] protected Vector3 openLocalScale = Vector3.one;

    [Header("Speed")]
    [SerializeField] protected float movementSpeed = 2f;

    [Header("Other Settings")]
    [SerializeField] protected bool isOpening = false;
    [SerializeField] protected bool isClosing = false;
    [SerializeField] protected bool startOpen = false;

    [Header("Platform Settings")]
    [SerializeField] protected bool isPlatform = false;

    [Header("Debug Settings")]
    [SerializeField][ShowOnly] protected Vector3 targetPosition;
    [SerializeField][ShowOnly] protected Quaternion targetRotation;
    [SerializeField][ShowOnly] protected Vector3 targetScale;
    [SerializeField][ShowOnly] protected bool isMoving = false;
    [SerializeField][ShowOnly] protected bool updatePosition = false;
    [SerializeField] protected bool showGizmos = false;

    [SerializeField][ShowOnly] protected Vector3 lastPosition;
    [SerializeField][ShowOnly] protected Vector3 platformVelocity;

    public Vector3 PlatformVelocity { get => platformVelocity; set => platformVelocity = value; }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        timeManager = GetComponent<ObjectTimeManager>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (startOpen)
        {
            SetLocalState(openLocalPosition, openLocalRotation, openLocalScale);
        }
        else
        {
            SetLocalState(closedLocalPosition, closedLocalRotation, closedLocalScale);
        }

        lastPosition = transform.position;

    }

    protected virtual void FixedUpdate()
    {
        if (timeManager.LocalTimeScale == 0f) return;

        float scaledMovementSpeed = movementSpeed * timeManager.LocalTimeScale;

        if (isOpening)
        {
            MoveToState(openLocalPosition, openLocalRotation, openLocalScale, scaledMovementSpeed);
        }

        if (isClosing)
        {
            MoveToState(closedLocalPosition, closedLocalRotation, closedLocalScale, scaledMovementSpeed);
        }

        PlatformVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }

    protected virtual void MoveToState(Vector3 targetLocalPosition, Quaternion targetLocalRotation, Vector3 targetLocalScale, float speed)
    {
        if (!isMoving)
        {
            isMoving = true;
            targetPosition = targetLocalPosition;
            targetRotation = targetLocalRotation;
            targetScale = targetLocalScale;
        }

        // Convert local positions to world positions for Rigidbody
        Vector3 targetWorldPosition = transform.parent != null ? transform.parent.TransformPoint(targetLocalPosition) : targetLocalPosition;

        Vector3 newPosition = Vector3.Lerp(transform.position, targetWorldPosition, Time.fixedDeltaTime * speed);
        Quaternion newRotation = Quaternion.Lerp(transform.localRotation, targetLocalRotation, Time.fixedDeltaTime * speed);
        Vector3 newScale = Vector3.Lerp(transform.localScale, targetLocalScale, Time.fixedDeltaTime * speed);

        rb.MovePosition(newPosition);
        transform.localRotation = newRotation;
        transform.localScale = newScale;

        if (Vector3.Distance(transform.position, targetWorldPosition) < 0.01f &&
            Quaternion.Angle(transform.localRotation, targetLocalRotation) < 0.01f &&
            Vector3.Distance(transform.localScale, targetLocalScale) < 0.01f)
        {
            SetLocalState(targetLocalPosition, targetLocalRotation, targetLocalScale);
            StopMoving();
        }
    }

    protected void SetLocalState(Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        Vector3 worldPosition = transform.parent != null ? transform.parent.TransformPoint(localPosition) : localPosition;
        rb.MovePosition(worldPosition);
        transform.localRotation = localRotation;
        transform.localScale = localScale;
    }


    public virtual void Open()
    {
        isOpening = true;
        isClosing = false;
        isMoving = false;
    }

    public virtual void Close()
    {
        isClosing = true;
        isOpening = false;
        isMoving = false;
    }

    protected virtual void StopMoving()
    {
        isOpening = false;
        isClosing = false;
        isMoving = false;
    }

    private void OnDrawGizmos()
    {
        if(!showGizmos) return;

        // Convert local positions to world positions for proper visualization
        Vector3 closedWorldPosition = transform.parent != null ? transform.parent.TransformPoint(closedLocalPosition) : closedLocalPosition;
        Vector3 openWorldPosition = transform.parent != null ? transform.parent.TransformPoint(openLocalPosition) : openLocalPosition;

        // Draw closed state
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(closedWorldPosition, closedLocalRotation, closedLocalScale);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        // Draw open state
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(openWorldPosition, openLocalRotation, openLocalScale);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(!isPlatform) return;
        if(other.GetComponent<PlayerReferences>() != null)
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isPlatform) return;
        if (other.GetComponent<PlayerReferences>() != null)
        {
            other.GetComponent<PlayerReferences>().SetCurrentPlatform(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!isPlatform) return;
        if (other.GetComponent<PlayerReferences>() != null)
        {
            other.GetComponent<PlayerReferences>().UnsetCurrentPlatform();
        }

    }


}
