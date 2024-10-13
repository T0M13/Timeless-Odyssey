using UnityEngine;

public class InventoryTool : MonoBehaviour
{
    [SerializeField] protected PlayerInventory inventoryParent;
    [SerializeField] protected GameObject toolPrefab;

    public GameObject ToolPrefab { get => toolPrefab; set => toolPrefab = value; }
    public PlayerInventory InventoryParent { get => inventoryParent; set => inventoryParent = value; }

    public virtual void PrimaryAction()
    {
        Debug.Log($"{name} primary action executed.");
    }

    public virtual void SecondaryAction()
    {
        Debug.Log($"{name} secondary action executed.");
    }
}
