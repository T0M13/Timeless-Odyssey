using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactTimeGrenade : ImpactGrenade
{
    [Header("Time Dilation Settings")]
    [SerializeField] protected float timeEffectRadius = 10f;
    [SerializeField] protected float slowFactor = 0.5f;
    [SerializeField] protected float effectDuration = 5f;
    [SerializeField] protected float timeDilationDelay = 1f;
    [SerializeField] protected GameObject timeSpherePrefab;
    [SerializeField] protected GameObject timeSphere;

    protected override void DestoryGrenade()
    {
        StartCoroutine(DelayedTimeDilation());
    }

    protected virtual IEnumerator DelayedTimeDilation()
    {
        yield return new WaitForSeconds(timeDilationDelay);
        SetTimeDilation();
    }

    public virtual void SetTimeDilation()
    {
        timeSphere = Instantiate(timeSpherePrefab, transform.position, Quaternion.identity);
        var timeSphereComponent = timeSphere.GetComponent<TimedSphere>();
        timeSphereComponent.SetMaterial();
        timeSphereComponent.SetSize(timeEffectRadius);
        timeSphereComponent.SetTimeScale(slowFactor);
        timeSphereComponent.SetDuration(effectDuration);
        timeSphereComponent.SetTimeDilation();
        Destroy(gameObject);
    }

    public virtual void SetStats(float explosionRadius, float explosionForce, float explosionDelay, float timeEffectRadius, float slowFactor, float effectDuration, float timeDilationDelay)
    {
        base.SetStats(explosionRadius, explosionForce, explosionDelay);
        this.timeEffectRadius = timeEffectRadius;
        this.slowFactor = slowFactor;
        this.effectDuration = effectDuration;
        this.timeDilationDelay = timeDilationDelay;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, timeEffectRadius);
    }
}
