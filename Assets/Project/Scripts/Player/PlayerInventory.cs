using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;

    [Header("Tools")]
    [SerializeField][ShowOnly] private List<GameObject> inventory = new List<GameObject>();
    [SerializeField][ShowOnly] private int activeToolIndex = 0;
    [SerializeField][ShowOnly] private GameObject currentTool;

    [Header("Tools Parent")]
    [SerializeField] private Transform toolsParent;

    [Header("Drop Settings")]
    [SerializeField] private Transform dropTransform;

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float throwThreshold = 0.5f;
    [SerializeField] private float maxHoldTime = 2f;

    [SerializeField][ShowOnly] private float holdTimer = 0f;
    [SerializeField][ShowOnly] private bool isThrowing = false;

    public Transform ToolsParent { get => toolsParent; set => toolsParent = value; }
    public Transform DropTransform { get => dropTransform; set => dropTransform = value; }

    private void Start()
    {
        if (playerReferences == null)
            playerReferences = GetComponent<PlayerReferences>();

        UpdateActiveTool();
    }

    private void Update()
    {
        if (isThrowing)
        {
            holdTimer = Mathf.Min(holdTimer + Time.deltaTime, maxHoldTime);
        }
    }

    public void OnUseToolPrimary(InputAction.CallbackContext value)
    {
        if (inventory.Count <= 0 || currentTool == null) return;
        if (value.phase != InputActionPhase.Performed) return;

        var toolComponent = currentTool.GetComponent<InventoryTool>();
        if (toolComponent == null) return;

        toolComponent.PrimaryAction();
    }

    public void OnUseToolSecondary(InputAction.CallbackContext value)
    {
        if (inventory.Count <= 0 || currentTool == null) return;
        if (value.phase != InputActionPhase.Performed) return;

        var toolComponent = currentTool.GetComponent<InventoryTool>();
        if (toolComponent == null) return;

        toolComponent.SecondaryAction();
    }

    public void OnScroll(InputAction.CallbackContext value)
    {
        if (inventory.Count <= 0) return;
        if (value.phase != InputActionPhase.Performed) return;

        float scrollValue = value.ReadValue<Vector2>().y;

        if (scrollValue > 0f)
        {
            activeToolIndex = (activeToolIndex + 1) % inventory.Count;
        }
        else if (scrollValue < 0f)
        {
            activeToolIndex = (activeToolIndex - 1 + inventory.Count) % inventory.Count;
        }

        UpdateActiveTool();
    }

    private void UpdateActiveTool()
    {
        if (inventory.Count <= 0) return;

        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i].SetActive(i == activeToolIndex);
        }

        currentTool = inventory[activeToolIndex];
    }

    public void DropCurrentTool(InputAction.CallbackContext context)
    {
        if (inventory.Count <= 0) return;

        if (context.phase == InputActionPhase.Started)
        {
            isThrowing = true;
            holdTimer = 0f;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            InventoryTool inventoryToolComponent = currentTool?.GetComponent<InventoryTool>();

            if (currentTool != null && inventoryToolComponent != null)
            {
                GameObject toolPrefab = inventoryToolComponent.ToolPrefab;
                Vector3 dropPos = DropTransform.position;
                GameObject droppedTool = Instantiate(toolPrefab, dropPos, Quaternion.identity);

                if (holdTimer >= throwThreshold)
                {
                    ThrowTool(droppedTool, holdTimer);
                }
                else
                {
                    DropTool(droppedTool);
                }

                isThrowing = false;
                holdTimer = 0f;

            }
        }
    }

    public void DropTool(GameObject tool)
    {
        RemoveToolFromInventory(currentTool);
    }

    public void ThrowTool(GameObject tool, float holdDuration)
    {
        Rigidbody rb = tool.GetComponent<Rigidbody>();

        if (rb != null)
        {
            float forceMultiplier = Mathf.Clamp(holdDuration, throwThreshold, maxHoldTime);
            rb.AddForce(ToolsParent.forward * throwForce * forceMultiplier, ForceMode.Impulse);
        }
        RemoveToolFromInventory(currentTool);

    }

    public void AddToolToInventory(GameObject tool)
    {
        tool.transform.SetParent(ToolsParent, false);
        inventory.Add(tool);

        //var toolComponent = tool.GetComponent<Tool>();
        //if(toolComponent != null)
        //{
        //    toolComponent.OnPickUp();
        //}and on Drop (Collider Effects, etc.)

        UpdateActiveTool();
    }

    public void RemoveToolFromInventory(GameObject tool)
    {
        if (tool == null) return;

        if (inventory.Contains(tool))
        {
            inventory.Remove(tool);
            Destroy(tool);
        }

        currentTool = null;
        if (inventory.Count > 0) activeToolIndex = Mathf.Clamp(activeToolIndex, 0, inventory.Count - 1);
        UpdateActiveTool();
    }
}
