using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactGrenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 8f;
    [SerializeField] private float explosionForce = 2700f;

    [Header("Visual Gizmo Settings")]
    [SerializeField] private bool showGizmos = true;

    private bool hasExploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Optional: Add explosion effects here (like sound, particle effects, etc.)

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
