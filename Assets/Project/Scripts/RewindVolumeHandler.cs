using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RewindVolumeHandler : MonoBehaviour
{
    [SerializeField] private PlayerRewind playerRewind;
    [SerializeField] private Volume volume;

    [Header("Volume Weight Settings")]
    [SerializeField] private float rewindLerpSpeed = 1f;  // Speed when rewinding
    [SerializeField] private float nonRewindLerpSpeed = 2f;  // Speed when returning to non-rewind
    [SerializeField] private float rewindWeight = 1f;  // Weight when rewinding
    [SerializeField] private float nonRewindWeight = 0f;  // Weight when not rewinding

    private bool isRewinding = false;

    private void Start()
    {
        if (playerRewind == null)
        {
            playerRewind = GetComponentInParent<PlayerRewind>();
        }

        if (volume == null)
        {
            Debug.LogError("Volume component is not assigned in the inspector.");
        }

        playerRewind.OnRewindStateChanged += HandleRewindStateChanged;

        volume.weight = nonRewindWeight;  // Start with zero weight
    }

    private void OnDestroy()
    {
        playerRewind.OnRewindStateChanged -= HandleRewindStateChanged;
    }

    private void HandleRewindStateChanged(bool isRewindingNow)
    {
        isRewinding = isRewindingNow;
        StopAllCoroutines();  // Stop previous smooth transition before starting a new one

        // Choose the lerp speed depending on the rewinding state
        float selectedLerpSpeed = isRewinding ? rewindLerpSpeed : nonRewindLerpSpeed;
        StartCoroutine(SmoothUpdateVolumeWeight(isRewinding ? rewindWeight : nonRewindWeight, selectedLerpSpeed));
    }

    private IEnumerator SmoothUpdateVolumeWeight(float targetWeight, float lerpSpeed)
    {
        float initialWeight = volume.weight;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lerpSpeed;
            volume.weight = Mathf.Lerp(initialWeight, targetWeight, t);
            yield return null;
        }

        volume.weight = targetWeight;
    }
}
