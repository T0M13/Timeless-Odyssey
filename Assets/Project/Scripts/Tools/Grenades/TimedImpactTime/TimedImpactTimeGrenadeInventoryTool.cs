using UnityEngine;

public class TimedImpactTimeGrenadeInventoryTool : ImpactTimeGrenadeInventoryTool
{
    [Header("Time Throw/Drop Scale Settings")]
    [SerializeField] private float grenadeThrowSlowFactor = 0.1f;
    [SerializeField] private float throwTimeEffectDuration = 5f;
    [SerializeField] private float throwTimeDilationDelay = 0.2f;

    public virtual void SetStats(float explosionRadius, float explosionForce, float explosionDelay,
         float timeEffectRadius, float slowFactor, float effectDuration, float timeDilationDelay,
         float grenadeThrowSlowFactor, float throwTimeEffectDuration, float throwTimeDilationDelay)
    {
        base.SetStats(explosionRadius, explosionForce, explosionDelay, timeEffectRadius, slowFactor, effectDuration, timeDilationDelay);

        this.grenadeThrowSlowFactor = grenadeThrowSlowFactor;
        this.throwTimeEffectDuration = throwTimeEffectDuration;
        this.throwTimeDilationDelay = throwTimeDilationDelay;
     
    }

    protected override void SetGrenadeStats(GameObject grenade)
    {
        TimedImpactTimeGrenade grenadeComponent = grenade.GetComponent<TimedImpactTimeGrenade>();
        if (grenadeComponent != null)
        {
            grenadeComponent.SetStats(explosionRadius, explosionForce, explosionDelay, timeEffectRadius, slowFactor, effectDuration, timeDilationDelay, grenadeThrowSlowFactor, throwTimeEffectDuration, throwTimeDilationDelay);
        }
    }

    protected override void ThrowGrenade(GameObject grenade)
    {
        base.ThrowGrenade(grenade);
        TimedImpactTimeGrenade grenadeComponent = grenade.GetComponent<TimedImpactTimeGrenade>();
        if (grenadeComponent != null)
        {
            grenadeComponent.StartSlowedThrowEffect(grenade);
        }

    }

    protected override void DropGrenade(GameObject grenade)
    {
        base.DropGrenade(grenade);
        TimedImpactTimeGrenade grenadeComponent = grenade.GetComponent<TimedImpactTimeGrenade>();
        if (grenadeComponent != null)
        {
            grenadeComponent.StartSlowedThrowEffect(grenade);
        }

    }

}
