using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    [Header("References")]
    [SerializeField] protected SphereCollider grenadeCollider;
    [SerializeField] protected Renderer grenadeRenderer;

    [Header("Explosion Settings")]
    [SerializeField] protected float explosionRadius = 8f;
    [SerializeField] protected float explosionForce = 2700f;
    [SerializeField] protected float explosionDelay = 3f;

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

    protected virtual void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    protected virtual IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);

        Explode();
    }

    protected virtual void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        DisableGrenade();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        DestoryGrenade();
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

    public virtual void SetStats(float explosionRadius, float explosionForce, float explosionDelay)
    {
        this.explosionRadius = explosionRadius;
        this.explosionForce = explosionForce;
        this.explosionDelay = explosionDelay;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
