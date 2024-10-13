using UnityEngine;

public class GrenadeInventoryTool : InventoryTool
{
    [Header("Throw/Drop Settings")]
    [SerializeField] protected float throwForce = 30f;
    [SerializeField] protected float dropForce = 2f;
    [SerializeField] protected GameObject grenadePrefab;

    [Header("Explosion Settings")]
    [SerializeField] protected float explosionRadius = 8f;
    [SerializeField] protected float explosionForce = 2700f;
    [SerializeField] protected float explosionDelay = 3f;

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

    public virtual void SetStats(float explosionRadius, float explosionForce, float explosionDelay)
    {
        this.explosionRadius = explosionRadius;
        this.explosionForce = explosionForce;
        this.explosionDelay = explosionDelay;
    }

    protected virtual void SetGrenadeStats(GameObject grenade)
    {
        Grenade grenadeComponent = grenade.GetComponent<Grenade>();
        if (grenadeComponent != null)
        {
            grenadeComponent.SetStats(explosionRadius, explosionForce, explosionDelay);
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
