using UnityEngine;

public class RewindGrenadeTool : Tool
{
    [Header("Rewind Settings")]
    [SerializeField] protected float rewindRadius = 25f;
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float rewindSpeed = .03f;
    [SerializeField] protected float rewindDelay = .1f;

    public override void OnInteract(PlayerReferences playerReferences)
    {
        base.OnInteract(playerReferences);
    }

    public override void DestoryOnPickUp(PlayerReferences playerReferences, GameObject inventoryTool)
    {
        RewindGrenadeInventoryTool inventoryToolComponent = inventoryTool.GetComponent<RewindGrenadeInventoryTool>();
        inventoryToolComponent.SetStats(rewindRadius, recordInterval,  rewindSpeed, rewindDelay);
        base.DestoryOnPickUp(playerReferences, inventoryTool);  
    }

    public virtual void SetStatsForTool(float rewindRadius, float recordInterval,  float rewindSpeed, float rewindDelay)
    {
        this.rewindRadius = rewindRadius;
        this.recordInterval = recordInterval;
        this.rewindSpeed = rewindSpeed;
        this.rewindDelay = rewindDelay;
    }

}
