using UnityEngine;

public class TimeGrenadeTool : GrenadeTool
{
    [Header("Time Dilation Settings")]
    [SerializeField] protected float timeEffectRadius = 10f;
    [SerializeField] protected float slowFactor = 0.1f;
    [SerializeField] protected float effectDuration = 5f;
    [SerializeField] protected float timeDilationDelay = 0.1f;

    public override void DestoryOnPickUp(PlayerReferences playerReferences, GameObject inventoryTool)
    {
        TimeGrenadeInventoryTool inventoryToolComponent = inventoryTool.GetComponent<TimeGrenadeInventoryTool>();
        inventoryToolComponent.SetStats(explosionRadius, explosionForce, explosionDelay);
        base.DestoryOnPickUp(playerReferences, inventoryTool);  
    }

    public virtual void SetStatsForTool(float explosionRadius, float explosionForce, float explosionDelay,
        float timeEffectRadius, float slowFactor, float effectDuration, float timeDilationDelay)
    {
        base.SetStatsForTool(explosionRadius, explosionForce,explosionDelay);

        this.timeEffectRadius = timeEffectRadius;
        this.slowFactor = slowFactor;
        this.effectDuration = effectDuration;
        this.timeDilationDelay = timeDilationDelay;
    }

}
