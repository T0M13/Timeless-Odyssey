using UnityEngine;

public class ImpactGrenadeTool : GrenadeTool
{
    public override void DestoryOnPickUp(PlayerReferences playerReferences, GameObject inventoryTool)
    {
        ImpactGrenadeInventoryTool inventoryToolComponent = inventoryTool.GetComponent<ImpactGrenadeInventoryTool>();
        inventoryToolComponent.SetStats(explosionRadius, explosionForce, explosionDelay);
        base.DestoryOnPickUp(playerReferences, inventoryTool);  
    }

}
