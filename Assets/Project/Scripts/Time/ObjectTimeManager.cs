using System.Collections.Generic;
using UnityEngine;

public class ObjectTimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float localTimeScale = 1f;
    [SerializeField] private float defaultLocalTimeScale = 1f;
    [SerializeField] private bool isTimeControlledCurrently = false;
    [SerializeField] private bool CanBeTimeControlled = true;

    [Header("Rigidbody Settings")]
    [SerializeField] private Rigidbody rb;

    [Header("Original Rigidbody Data")]
    [SerializeField] private Vector3 originalVelocity;
    [SerializeField] private Vector3 originalAngularVelocity;
    [SerializeField] private float originalDrag;
    [SerializeField] private float originalAngularDrag;
    [SerializeField] private bool originalUseGravity;
    [SerializeField] private Vector3 originalGravity;

    public float LocalTimeScale { get => localTimeScale; set => localTimeScale = value; }

    void Awake()
    {
        GetRefs();
    }

    void FixedUpdate()
    {
        if (LocalTimeScale != defaultLocalTimeScale && rb != null && isTimeControlledCurrently && CanBeTimeControlled)
        {
            ApplyTimeScale();
        }
    }

    public void SetTimeScale(float newTimeScale)
    {
        if (!CanBeTimeControlled) return;
        if (!isTimeControlledCurrently)
            StoreOriginalData();

        isTimeControlledCurrently = true;
        LocalTimeScale = newTimeScale;
        UpdateRigidbody();
    }

    public void ResetTimeScale()
    {
        if (!CanBeTimeControlled) return;

        isTimeControlledCurrently = false;
        RestoreOriginalData();
        LocalTimeScale = defaultLocalTimeScale;
    }

    private void GetRefs()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void StoreOriginalData()
    {
        if (rb == null) return;

        // Only store velocity if the Rigidbody is not kinematic
        if (!rb.isKinematic)
        {
            originalVelocity = rb.velocity;
            originalAngularVelocity = rb.angularVelocity;
        }

        originalDrag = rb.drag;
        originalAngularDrag = rb.angularDrag;
        originalUseGravity = rb.useGravity;
        originalGravity = Physics.gravity;
        rb.useGravity = false;
    }

    private void RestoreOriginalData()
    {
        if (rb == null) return;

        // Only restore velocity if the Rigidbody is not kinematic
        if (!rb.isKinematic)
        {
            rb.velocity = originalVelocity;
            rb.angularVelocity = originalAngularVelocity;
        }

        rb.drag = originalDrag;
        rb.angularDrag = originalAngularDrag;
        rb.useGravity = originalUseGravity;
    }

    private void ApplyTimeScale()
    {
        // Only apply velocity scaling if the Rigidbody is not kinematic
        if (!rb.isKinematic)
        {
            rb.velocity = originalVelocity * LocalTimeScale;
            rb.angularVelocity = originalAngularVelocity * LocalTimeScale;
        }

        Vector3 customGravity = originalGravity * LocalTimeScale;
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    private void UpdateRigidbody()
    {
        rb.drag *= LocalTimeScale;
        rb.angularDrag *= LocalTimeScale;
    }
}
