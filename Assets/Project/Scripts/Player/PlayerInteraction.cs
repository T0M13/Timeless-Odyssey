using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;

    private RaycastHit hitInfo;

    private void Start()
    {
        if (playerReferences == null)
            playerReferences = GetComponent<PlayerReferences>();
    }

    private void Update()
    {
        PerformRaycast();
    }

    private void PerformRaycast()
    {
        Vector3 rayOrigin = playerReferences.PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).origin;

        if (Physics.Raycast(rayOrigin, playerReferences.PlayerCamera.transform.forward, out hitInfo, interactionDistance, interactionLayer))
        {
            Debug.DrawRay(rayOrigin, playerReferences.PlayerCamera.transform.forward * interactionDistance, Color.green);
        }
        else
        {
            Debug.DrawRay(rayOrigin, playerReferences.PlayerCamera.transform.forward * interactionDistance, Color.red);
        }
    }

    public void OnInteract(InputAction.CallbackContext value)
    {
        if (value.phase != InputActionPhase.Performed) return;

        if (hitInfo.collider != null)
        {
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.OnInteract(playerReferences);
            }
        }
    }
}
