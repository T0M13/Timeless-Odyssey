using System.Collections;
using UnityEngine;

public class TimedGrenadeInventoryTool : InventoryTool
{
    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float dropForce = 2f;
    [SerializeField] private GameObject grenadePrefab;

    public override void PrimaryAction()
    {
        Vector3 dropPos = InventoryParent.DropTransform.position;
        GameObject droppedGrenade = Instantiate(grenadePrefab, dropPos, Quaternion.identity);
        ThrowTimedGrenade(droppedGrenade);
    }

    public override void SecondaryAction()
    {
        Vector3 dropPos = InventoryParent.DropTransform.position;
        GameObject droppedGrenade = Instantiate(grenadePrefab, dropPos, Quaternion.identity);
        DropTimedGrenade(droppedGrenade);
    }

    public void ThrowTimedGrenade(GameObject grenade)
    {
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(InventoryParent.ToolsParent.forward * throwForce, ForceMode.Impulse);
        }

        DelayTimeGrenade(grenade);
        InventoryParent.RemoveToolFromInventory(gameObject);
    }

    public void DropTimedGrenade(GameObject grenade)
    {
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(InventoryParent.ToolsParent.forward * dropForce, ForceMode.Impulse);
        }
        DelayTimeGrenade(grenade);
        InventoryParent.RemoveToolFromInventory(gameObject);
    }

    private void DelayTimeGrenade(GameObject grenade)
    {
        var timedGrenadeComponent = grenade.GetComponent<TimedImpactTimeGrenade>();
        if (timedGrenadeComponent != null)
        {
            timedGrenadeComponent.StartDelayTimeEffect(grenade);
        }
    }


}
