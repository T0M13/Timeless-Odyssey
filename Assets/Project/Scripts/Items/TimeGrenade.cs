using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGrenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private float explosionDelay = 3f;

    [Header("Time Dilation Settings")]
    [SerializeField] private float timeRadius = 10f;
    [SerializeField] private float slowFactor = 0.5f;
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private float timeDilationDelay = 1f;
    [SerializeField] private GameObject timeSpherePrefab;
    [SerializeField] private GameObject timeSphere;

    [Header("Visual Gizmo Settings")]
    [SerializeField] private bool showGizmos = true;

    [SerializeField] private bool hasExploded = false;
    [SerializeField] private SphereCollider grenadeCollider;
    [SerializeField] private Renderer grenadeRenderer;

    private void Start()
    {
        grenadeCollider = GetComponent<SphereCollider>();
        grenadeRenderer = GetComponent<Renderer>();
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Disable grenade visual and colliders after explosion
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

        // Start time dilation after the delay
        StartCoroutine(DelayedTimeDilation());
    }

    private IEnumerator DelayedTimeDilation()
    {
        yield return new WaitForSeconds(timeDilationDelay);
        SetTimeDilation();
    }

    private void SetTimeDilation()
    {
        timeSphere = Instantiate(timeSpherePrefab, transform.position, Quaternion.identity);
        var timeSphereComponent = timeSphere.GetComponent<TimedSphere>();
        timeSphereComponent.SetMaterial();
        timeSphereComponent.SetSize(timeRadius);
        timeSphereComponent.SetTimeScale(slowFactor);
        timeSphereComponent.SetDuration(effectDuration);
        timeSphereComponent.SetTimeDilation();
        Destroy(gameObject);
    }


    private void DisableGrenade()
    {
        // Disable grenade renderer and colliders
        if (grenadeRenderer != null) grenadeRenderer.enabled = false;
        if (grenadeCollider != null) grenadeCollider.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            // Visualize explosion radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);

            // Visualize time dilation radius
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, timeRadius);
        }
    }
}
