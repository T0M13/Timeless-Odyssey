using UnityEngine;

public class Tool : InteractableObject
{
    [SerializeField] private GameObject inventoryToolPrefab;
    [SerializeField] private bool destoryOnPickUp = true;

    public override void OnInteract(PlayerReferences playerReferences)
    {
        GameObject inventoryTool = Instantiate(inventoryToolPrefab);

        if (playerReferences != null)
        {
            inventoryTool.GetComponent<InventoryTool>().InventoryParent = playerReferences.PlayerInventory;
            playerReferences.PlayerInventory.AddToolToInventory(inventoryTool);
            if (destoryOnPickUp)
                Destroy(gameObject);
        }

    }

}
