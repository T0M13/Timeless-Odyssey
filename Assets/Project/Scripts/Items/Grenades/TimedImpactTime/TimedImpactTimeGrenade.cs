using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedImpactTimeGrenade : ImpactTimeGrenade
{
    [Header("Time Throw/Drop Scale Settings")]
    [SerializeField] private float grenadeThrowSlowFactor = 0.1f;
    [SerializeField] private float throwTimeEffectDuration = 5f;
    [SerializeField] private float throwTimeDilationDelay = 0.2f;
 
    public virtual void StartSlowedThrowEffect(GameObject grenade)
    {
        StartCoroutine(DelayedTimeEffect(grenade));
    }

    protected virtual IEnumerator DelayedTimeEffect(GameObject grenade)
    {
        yield return new WaitForSeconds(timeDilationDelay);

        ApplyTimeEffect(grenade);
    }

    protected virtual void ApplyTimeEffect(GameObject grenade)
    {
        ObjectTimeManager timeManager = grenade.GetComponent<ObjectTimeManager>();
        if (timeManager != null)
        {
            timeManager.SetTimeScale(grenadeThrowSlowFactor);
            StartCoroutine(RestoreTimeAfterDuration(timeManager));
        }
    }

    public virtual void SetStats(float explosionRadius, float explosionForce, float explosionDelay, 
        float timeEffectRadius, float slowFactor, float effectDuration, float timeDilationDelay,
        float grenadeThrowSlowFactor, float throwTimeEffectDuration, float throwTimeDilationDelay)
    {
        base.SetStats(explosionRadius, explosionForce, explosionDelay, timeEffectRadius, slowFactor, effectDuration, timeDilationDelay);
        this.grenadeThrowSlowFactor = grenadeThrowSlowFactor;
        this.throwTimeEffectDuration = throwTimeEffectDuration;
        this.throwTimeDilationDelay = throwTimeDilationDelay;
    }

    protected virtual IEnumerator RestoreTimeAfterDuration(ObjectTimeManager timeManager)
    {
        yield return new WaitForSeconds(throwTimeEffectDuration);
        timeManager.ResetTimeScale();
    }

}
