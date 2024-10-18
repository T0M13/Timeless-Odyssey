using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private PlayerRewind playerRewind;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private CapsuleCollider playerCollider;
    [Header("Grounded")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 1f;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraTransform;

    [Header("Platforming")]
    [SerializeField] private Transform currentPlatform = null;

    [Header("Collider & Crouch Settings")]
    [SerializeField] private Vector3 defaultColliderCenter = new Vector3(0, 0, 0);
    [SerializeField] private float defaultColliderHeight = 3f;
    [SerializeField] private Vector3 crouchColliderCenter = new Vector3(0f, -0.75f, 0f);
    [SerializeField] private float crouchColliderHeight = 1.5f;
    [SerializeField] private float colliderTransitionSpeed = 5f;
    [SerializeField] private Vector3 defaultCameraPosition;
    [SerializeField] private Vector3 crouchCameraPosition;
    [SerializeField] private float cameraTransitionSpeed = 5f;
    private Coroutine colliderCoroutine;
    private Coroutine cameraCoroutine;


    public PlayerMovement PlayerMovement { get => playerMovement; set => playerMovement = value; }
    public PlayerLook PlayerLook { get => playerLook; set => playerLook = value; }
    public Rigidbody PlayerBody { get => playerBody; set => playerBody = value; }
    public CapsuleCollider PlayerCollider { get => playerCollider; set => playerCollider = value; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public Camera PlayerCamera { get => playerCamera; set => playerCamera = value; }
    public PlayerInventory PlayerInventory { get => playerInventory; set => playerInventory = value; }
    public PlayerInteraction PlayerInteraction { get => playerInteraction; set => playerInteraction = value; }
    public Transform CurrentPlatform { get => currentPlatform; set => currentPlatform = value; }
    public PlayerRewind PlayerRewind { get => playerRewind; set => playerRewind = value; }

    private void OnValidate()
    {
        GetReferences();
    }

    private void Awake()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        if (PlayerMovement == null)
        {
            try
            {
                PlayerMovement = GetComponent<PlayerMovement>();
            }
            catch
            {
                Debug.Log("PlayerMovement Missing from PlayerReferences");
            }
        }

        if (PlayerLook == null)
        {
            try
            {
                PlayerLook = GetComponent<PlayerLook>();
            }
            catch
            {
                Debug.Log("PlayerLook Missing from PlayerReferences");
            }
        }

        if (PlayerInventory == null)
        {
            try
            {
                PlayerInventory = GetComponent<PlayerInventory>();
            }
            catch
            {
                Debug.Log("PlayerInventory Missing from PlayerReferences");
            }
        }

        if (PlayerInteraction == null)
        {
            try
            {
                PlayerInteraction = GetComponent<PlayerInteraction>();
            }
            catch
            {
                Debug.Log("PlayerInteraction Missing from PlayerReferences");
            }
        }


        if (PlayerRewind == null)
        {
            try
            {
                PlayerRewind = GetComponent<PlayerRewind>();
            }
            catch
            {
                Debug.Log("PlayerRewind Missing from PlayerReferences");
            }
        }

        if (PlayerBody == null)
        {
            try
            {
                PlayerBody = GetComponent<Rigidbody>();
            }
            catch
            {
                Debug.Log("Rigidbody Missing from PlayerReferences");
            }
        }
        if (playerCollider == null)
        {
            try
            {
                playerCollider = GetComponent<CapsuleCollider>();
            }
            catch
            {
                Debug.Log("CapsuleCollider Missing from PlayerReferences");
            }
        }
    }

    private void Update()
    {
        isGrounded = CheckIsGrounded(playerBody);
    }

    private bool CheckIsGrounded(Rigidbody rb)
    {
        return Physics.SphereCast(rb.position, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);
    }

    public void SetDefaultCollider()
    {
        if (colliderCoroutine != null) StopCoroutine(colliderCoroutine);
        colliderCoroutine = StartCoroutine(LerpCollider(playerCollider, playerCollider.center, defaultColliderCenter, playerCollider.height, defaultColliderHeight));
    }

    public void SetCrouchCollider()
    {
        if (colliderCoroutine != null) StopCoroutine(colliderCoroutine);
        colliderCoroutine = StartCoroutine(LerpCollider(playerCollider, playerCollider.center, crouchColliderCenter, playerCollider.height, crouchColliderHeight));
    }

    public void SetDefaultCameraPosition()
    {
        if (cameraCoroutine != null) StopCoroutine(cameraCoroutine);
        cameraCoroutine = StartCoroutine(LerpCameraPosition(cameraTransform, cameraTransform.localPosition, defaultCameraPosition));
    }

    public void SetCrouchCameraPosition()
    {
        if (cameraCoroutine != null) StopCoroutine(cameraCoroutine);
        cameraCoroutine = StartCoroutine(LerpCameraPosition(cameraTransform, cameraTransform.localPosition, crouchCameraPosition));
    }

    public void SetCurrentPlatform(Transform platform)
    {
        CurrentPlatform = platform;
        transform.SetParent(CurrentPlatform);
    }
    public void UnsetCurrentPlatform()
    {
        transform.SetParent(null);
        CurrentPlatform = null;
    }


    private IEnumerator LerpCollider(CapsuleCollider collider, Vector3 startCenter, Vector3 targetCenter, float startHeight, float targetHeight)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * colliderTransitionSpeed;
            collider.center = Vector3.Lerp(startCenter, targetCenter, t);
            collider.height = Mathf.Lerp(startHeight, targetHeight, t);
            yield return null;
        }
    }

    private IEnumerator LerpCameraPosition(Transform cameraTransform, Vector3 startPosition, Vector3 targetPosition)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * cameraTransitionSpeed;
            cameraTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerBody != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(playerBody.position + Vector3.down * groundCheckDistance, groundCheckRadius);
        }
    }


}
