using UnityEngine;

public class TimedImpactTimeGrenadeTool : ImpactTimeGrenadeTool
{
    [Header("Time Throw/Drop Scale Settings")]
    [SerializeField] private float grenadeThrowSlowFactor = 0.1f;
    [SerializeField] private float throwTimeEffectDuration = 5f;
    [SerializeField] private float throwTimeDilationDelay = 0.2f;

    public override void DestoryOnPickUp(PlayerReferences playerReferences, GameObject inventoryTool)
    {
        TimedImpactTimeGrenadeInventoryTool inventoryToolComponent = inventoryTool.GetComponent<TimedImpactTimeGrenadeInventoryTool>();
        inventoryToolComponent.SetStats(explosionRadius, explosionForce, explosionDelay, timeEffectRadius, slowFactor, effectDuration, timeDilationDelay, grenadeThrowSlowFactor, throwTimeEffectDuration, throwTimeDilationDelay);
        base.DestoryOnPickUp(playerReferences, inventoryTool);
    }

    public virtual void SetStatsForTool(float explosionRadius, float explosionForce, float explosionDelay,
        float timeEffectRadius, float slowFactor, float effectDuration, float timeDilationDelay,
         float grenadeThrowSlowFactor, float throwTimeEffectDuration, float throwTimeDilationDelay)
    {
        base.SetStatsForTool(explosionRadius, explosionForce, explosionDelay, timeEffectRadius, slowFactor, effectDuration, timeDilationDelay);

        this.grenadeThrowSlowFactor = grenadeThrowSlowFactor;
        this.throwTimeEffectDuration = throwTimeEffectDuration;
        this.throwTimeDilationDelay = throwTimeDilationDelay;
    }

}
