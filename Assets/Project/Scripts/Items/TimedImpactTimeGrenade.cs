using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedImpactTimeGrenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;

    [Header("Time Dilation Settings")]
    [SerializeField] private float timeRadius = 10f;
    [SerializeField] private float slowFactor = 0.5f;
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private float timeDilationDelay = 1f;
    [SerializeField] private GameObject timeSpherePrefab;
    [SerializeField] private GameObject timeSphere;

    [Header("Time Throw/Drop Scale Settings")]
    [SerializeField] private float grenadeThrowTimeScale = 0.1f;
    [SerializeField] private float timeThrowEffectDuration = 5f;
    [SerializeField] private float timeThrowDilationDelay = 0.2f;

    [Header("Visual Gizmo Settings")]
    [SerializeField] private bool showGizmos = true;

    [SerializeField] private bool hasExploded = false;
    [SerializeField] private SphereCollider grenadeCollider;
    [SerializeField] private Renderer grenadeRenderer;

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
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

    public void StartDelayTimeEffect(GameObject grenade)
    {
        StartCoroutine(DelayedTimeEffect(grenade));
    }

    private IEnumerator DelayedTimeEffect(GameObject grenade)
    {
        yield return new WaitForSeconds(timeDilationDelay);

        ApplyTimeEffect(grenade);
    }

    private void ApplyTimeEffect(GameObject grenade)
    {
        ObjectTimeManager timeManager = grenade.GetComponent<ObjectTimeManager>();
        if (timeManager != null)
        {
            timeManager.SetTimeScale(grenadeThrowTimeScale);
            StartCoroutine(RestoreTimeAfterDuration(timeManager));
        }
    }

    private IEnumerator RestoreTimeAfterDuration(ObjectTimeManager timeManager)
    {
        yield return new WaitForSeconds(timeThrowEffectDuration);
        timeManager.ResetTimeScale();
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
