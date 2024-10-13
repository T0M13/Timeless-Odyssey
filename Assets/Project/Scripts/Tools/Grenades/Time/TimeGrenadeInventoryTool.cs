using UnityEngine;

public class TimeGrenadeInventoryTool : GrenadeInventoryTool
{

    [Header("Time Dilation Settings")]
    [SerializeField] protected float timeEffectRadius = 10f;
    [SerializeField] protected float slowFactor = 0.1f;
    [SerializeField] protected float effectDuration = 5f;
    [SerializeField] protected float timeDilationDelay = 0.1f;

    public virtual void SetStats(float explosionRadius, float explosionForce, float explosionDelay,
        float timeEffectRadius, float slowFactor, float effectDuration, float timeDilationDelay)
    {
        base.SetStats(explosionRadius, explosionForce, explosionDelay);

        this.timeEffectRadius = timeEffectRadius;
        this.slowFactor = slowFactor;
        this.effectDuration = effectDuration;
        this.timeDilationDelay = timeDilationDelay;
    }

    protected override void SetGrenadeStats(GameObject grenade)
    {
        TimeGrenade grenadeComponent = grenade.GetComponent<TimeGrenade>();
        if (grenadeComponent != null)
        {
            grenadeComponent.SetStats(explosionRadius, explosionForce, explosionDelay, timeEffectRadius, slowFactor, effectDuration, timeDilationDelay);
        }
    }
}
