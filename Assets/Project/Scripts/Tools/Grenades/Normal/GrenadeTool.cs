using UnityEngine;

public class GrenadeTool : Tool
{
    [Header("Explosion Settings")]
    [SerializeField] protected float explosionRadius = 8f;
    [SerializeField] protected float explosionForce = 2700f;
    [SerializeField] protected float explosionDelay = 3f;

    public override void OnInteract(PlayerReferences playerReferences)
    {
        base.OnInteract(playerReferences);
    }

    public override void DestoryOnPickUp(PlayerReferences playerReferences, GameObject inventoryTool)
    {
        GrenadeInventoryTool inventoryToolComponent = inventoryTool.GetComponent<GrenadeInventoryTool>();
        inventoryToolComponent.SetStats(explosionRadius, explosionForce, explosionDelay);
        base.DestoryOnPickUp(playerReferences, inventoryTool);  
    }

    public virtual void SetStatsForTool(float explosionRadius, float explosionForce, float explosionDelay)
    {
        this.explosionRadius = explosionRadius;
        this.explosionForce = explosionForce;
        this.explosionDelay = explosionDelay;
    }

}
