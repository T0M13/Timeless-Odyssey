using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField] private PlayerRewind playerRewind; 
    [SerializeField] private GameObject rewindParticles;

    private void Start()
    {
        if (playerRewind == null)
        {
            playerRewind = GetComponent<PlayerRewind>();  
        }

        if (rewindParticles != null)
        {
            rewindParticles.SetActive(false); 
        }

        playerRewind.OnRewindStateChanged += HandleRewindStateChanged;
    }

    private void OnDestroy()
    {
        playerRewind.OnRewindStateChanged -= HandleRewindStateChanged;
    }

    private void HandleRewindStateChanged(bool isRewinding)
    {
        if (rewindParticles != null)
        {
            rewindParticles.SetActive(isRewinding); 
        }
    }
}
