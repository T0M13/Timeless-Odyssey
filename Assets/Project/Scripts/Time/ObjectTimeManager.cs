using System.Collections.Generic;
using UnityEngine;

public class ObjectTimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float localTimeScale = 1f;
    [SerializeField] private float defaultLocalTimeScale = 1f;
    [SerializeField] private bool isTimeControlledCurrently = false;

    [Header("Rigidbody Settings")]
    [SerializeField] private Rigidbody rb;
    
    [Header("Original Rigidbody Data")]
    [SerializeField] private Vector3 originalVelocity;
    [SerializeField] private Vector3 originalAngularVelocity;
    [SerializeField] private float originalDrag;
    [SerializeField] private float originalAngularDrag;
    [SerializeField] private bool originalUseGravity;
    [SerializeField] private Vector3 originalGravity;

    void Awake()
    {
        GetRefs();
    }

    void FixedUpdate()
    {
        if (localTimeScale != defaultLocalTimeScale && rb != null && isTimeControlledCurrently)
        {
            ApplyTimeScale();
        }
    }

    public void SetTimeScale(float newTimeScale)
    {
        if (!isTimeControlledCurrently)
            StoreOriginalData();
            
        isTimeControlledCurrently = true;
        localTimeScale = newTimeScale;
        UpdateRigidbody();
    }

    public void ResetTimeScale()
    {
        isTimeControlledCurrently = false;
        RestoreOriginalData();
        localTimeScale = defaultLocalTimeScale;
    }

    private void GetRefs()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void StoreOriginalData()
    {
        originalVelocity = rb.velocity;
        originalAngularVelocity = rb.angularVelocity;
        originalDrag = rb.drag;
        originalAngularDrag = rb.angularDrag;
        originalUseGravity = rb.useGravity;
        originalGravity = Physics.gravity;
        rb.useGravity = false;
    }

    private void RestoreOriginalData()
    {
        rb.velocity = originalVelocity;
        rb.angularVelocity = originalAngularVelocity;
        rb.drag = originalDrag;
        rb.angularDrag = originalAngularDrag;
        rb.useGravity = originalUseGravity;
    }

    private void ApplyTimeScale()
    {
        rb.velocity = originalVelocity * localTimeScale;
        rb.angularVelocity = originalAngularVelocity * localTimeScale;
        Vector3 customGravity = originalGravity * localTimeScale;
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    private void UpdateRigidbody()
    {
        rb.drag *= localTimeScale;
        rb.angularDrag *= localTimeScale;
    }
}
