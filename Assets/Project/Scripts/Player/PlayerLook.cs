using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public enum UpdateType
    {
        Update,
        FixedUpdate,
        LateUpdate
    }

    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;

    [Header("Look Settings")]
    [SerializeField] private float minViewDistance = 25f;
    [SerializeField] private float aimSensitivity = 100f;
    [SerializeField] private bool reverseMouse = false;
    [SerializeField][ShowOnly] private float xRotation = 0f;

    [Header("Input")]
    [SerializeField] private Vector2 lookPos;

    [Header("Update Method")]
    [SerializeField] private UpdateType updateMethod = UpdateType.LateUpdate; 

    public Vector2 LookPos { get => lookPos; set => lookPos = value; }

    private void Awake()
    {
        GetReferences();
    }

    private void OnValidate()
    {
        GetReferences();
    }

    private void Start()
    {
        LockMouse();
    }

    private void Update()
    {
        if (updateMethod == UpdateType.Update)
        {
            HandleLook();
        }
    }

    private void FixedUpdate()
    {
        if (updateMethod == UpdateType.FixedUpdate)
        {
            HandleLook();
        }
    }

    private void LateUpdate()
    {
        if (updateMethod == UpdateType.LateUpdate)
        {
            HandleLook();
        }
    }

    private void GetReferences()
    {
        if (playerReferences == null)
        {
            playerReferences = GetComponent<PlayerReferences>();
            if (playerReferences == null)
            {
                Debug.LogWarning("PlayerReferences component is missing from PlayerMovement");
            }
        }
    }

    private void HandleLook()
    {
        float mouseX = lookPos.x * aimSensitivity;
        float mouseY = (reverseMouse ? -lookPos.y : lookPos.y) * aimSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, minViewDistance);

        playerReferences.PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerReferences.PlayerBody.transform.Rotate(Vector3.up * mouseX);
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnLook(InputAction.CallbackContext value)
    {
        LookPos = value.ReadValue<Vector2>();
    }
}
