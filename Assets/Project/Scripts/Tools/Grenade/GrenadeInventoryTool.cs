using UnityEngine;

public class GrenadeInventoryTool : InventoryTool
{

    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float dropForce = 2f;
    [SerializeField] private GameObject grenadePrefab;

    public override void PrimaryAction()
    {
        Vector3 dropPos = InventoryParent.DropTransform.position;
        GameObject droppedGrenade = Instantiate(grenadePrefab, dropPos, Quaternion.identity);
        ThrowGrenade(droppedGrenade);
    }

    public override void SecondaryAction()
    {
        Vector3 dropPos = InventoryParent.DropTransform.position;
        GameObject droppedGrenade = Instantiate(grenadePrefab, dropPos, Quaternion.identity);
        DropGrenade(droppedGrenade);
    }

    public void ThrowGrenade(GameObject grenade)
    {
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(InventoryParent.ToolsParent.forward * throwForce, ForceMode.Impulse);
        }
        InventoryParent.RemoveToolFromInventory(gameObject);
    }

    public void DropGrenade(GameObject grenade)
    {
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(InventoryParent.ToolsParent.forward * dropForce, ForceMode.Impulse);
        }
        InventoryParent.RemoveToolFromInventory(gameObject);
    }
}
