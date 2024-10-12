using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isInteractable = true;

    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }

    public virtual void OnInteract(PlayerReferences playerReferences)
    {
        //Do Something
    }
}
