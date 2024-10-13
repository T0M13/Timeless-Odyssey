using UnityEngine;

public class Tool : InteractableObject
{
    [SerializeField] protected GameObject inventoryToolPrefab;
    [SerializeField] protected bool destoryOnPickUp = true;

    public override void OnInteract(PlayerReferences playerReferences)
    {
        if (playerReferences == null) return;

        GameObject inventoryTool = Instantiate(inventoryToolPrefab);
        InventoryTool inventoryToolComponent = inventoryTool.GetComponent<InventoryTool>();
        inventoryToolComponent.InventoryParent = playerReferences.PlayerInventory;
        playerReferences.PlayerInventory.AddToolToInventory(inventoryTool);
        DestoryOnPickUp(playerReferences, inventoryTool);
    }

    public virtual void DestoryOnPickUp(PlayerReferences playerReferences, GameObject inventoryTool)
    {
        if (destoryOnPickUp)
            Destroy(gameObject);
    }

}
