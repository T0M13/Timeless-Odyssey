using UnityEngine;

public class RewindGrenadeInventoryTool : InventoryTool
{
    [Header("Throw/Drop Settings")]
    [SerializeField] protected float throwForce = 30f;
    [SerializeField] protected float dropForce = 2f;
    [SerializeField] protected GameObject grenadePrefab;

    [Header("Rewind Settings")]
    [SerializeField] protected float rewindRadius = 8f;
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float rewindSpeed = .03f;
    [SerializeField] private float rewindDelay = .1f;

    public override void PrimaryAction()
    {
        ThrowGrenade(InstantiateGrenade());
    }

    public override void SecondaryAction()
    {
        DropGrenade(InstantiateGrenade());
    }

    public virtual GameObject InstantiateGrenade()
    {
        Vector3 dropPos = InventoryParent.DropTransform.position;
        GameObject droppedGrenade = Instantiate(grenadePrefab, dropPos, Quaternion.identity);
        SetGrenadeStats(droppedGrenade);
        return droppedGrenade;
    }

    public virtual void SetStats(float rewindRadius, float recordInterval, float rewindSpeed, float rewindDelay)
    {
        this.rewindRadius = rewindRadius;
        this.recordInterval = recordInterval;
        this.rewindSpeed = rewindSpeed;
        this.rewindDelay = rewindDelay;
    }

    protected virtual void SetGrenadeStats(GameObject grenade)
    {
        RewindGrenade grenadeComponent = grenade.GetComponent<RewindGrenade>();
        if (grenadeComponent != null)
        {
            grenadeComponent.SetStats(rewindRadius, recordInterval,  rewindSpeed, rewindDelay);
        }
    }

    protected virtual void ThrowGrenade(GameObject grenade)
    {
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(InventoryParent.ToolsParent.forward * throwForce, ForceMode.Impulse);
        }
        InventoryParent.RemoveToolFromInventory(gameObject);
    }

    protected virtual void DropGrenade(GameObject grenade)
    {
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(InventoryParent.ToolsParent.forward * dropForce, ForceMode.Impulse);
        }
        InventoryParent.RemoveToolFromInventory(gameObject);
    }
}
