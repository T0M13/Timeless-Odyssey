using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindGrenade : MonoBehaviour
{

    [Header("References")]
    [SerializeField] protected SphereCollider grenadeCollider;
    [SerializeField] protected Renderer grenadeRenderer;
    [SerializeField] protected GameObject rewindTimeSpherePrefab;

    [Header("Rewind Settings")]
    [SerializeField] protected float rewindRadius = 25f;
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float rewindSpeed = .03f;
    [SerializeField] protected float rewindDelay = .1f;

    [Header("Visual Gizmo Settings")]
    [SerializeField] protected bool showGizmos = true;

    protected bool hasExploded = false;

    protected virtual void Awake()
    {
        if (grenadeCollider == null)
            grenadeCollider = GetComponent<SphereCollider>();

        if (grenadeRenderer == null)
            grenadeRenderer = GetComponent<Renderer>();
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    protected virtual IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(rewindDelay);
        Explode();
    }

    protected virtual void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        DisableGrenade();

        SpawnRewindSphere();

        DestoryGrenade();
    }

    public virtual void SpawnRewindSphere()
    {
        GameObject rewindSphere = Instantiate(rewindTimeSpherePrefab, transform.position, Quaternion.identity);
        RewindTimeSphere rewindTimeSphereComponent = rewindSphere.GetComponent<RewindTimeSphere>();
        rewindTimeSphereComponent.SetSphereStats(rewindRadius, recordInterval,  rewindSpeed);
        rewindTimeSphereComponent.InitiateTimeSphere();
    }

    protected virtual void DisableGrenade()
    {
        if (grenadeRenderer != null) grenadeRenderer.enabled = false;
        if (grenadeCollider != null) grenadeCollider.enabled = false;
    }

    protected virtual void DestoryGrenade()
    {
        Destroy(gameObject);
    }

    public virtual void SetStats(float rewindRadius, float recordInterval, float rewindSpeed, float rewindDelay)
    {
        this.rewindRadius = rewindRadius;
        this.recordInterval = recordInterval;
        this.rewindSpeed = rewindSpeed;
        this.rewindDelay = rewindDelay;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rewindRadius);
    }
}
