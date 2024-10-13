using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSphere : MonoBehaviour
{
    [Header("Time Sphere Settings")]
    [SerializeField] private float timeScaleInsideSphere = 0.5f;
    [SerializeField] private float sphereSize = 5f;
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private bool isInDilation = false;
    [SerializeField] private bool setDilationOnStart = false;
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private List<ObjectTimeManager> objectsInsideSphere = new List<ObjectTimeManager>();
    [SerializeField] private float sphereSizeDistanceOffsetThreshold = 1f;

    [Header("Growth Settings")]
    [SerializeField] private float growthDuration = 1f;
    [SerializeField] private float currentGrowthTime = 0f;
    [SerializeField] private bool isGrowing = false;
    [SerializeField]
    private AnimationCurve growthCurve = new AnimationCurve(
  new Keyframe(0f, 0f),
  new Keyframe(0.7f, 0.9f),
  new Keyframe(1f, 1f));

    [Header("Shader Settings")]
    [SerializeField] private Renderer bubbleRenderer;
    [SerializeField] private Material bubbleMaterialInstance;

    private void Start()
    {
        if (!setDilationOnStart) return;
        InitiateTimeSphere();
    }

    private void SetMaterial()
    {
        bubbleMaterialInstance = bubbleRenderer.material;
    }

    private void SetTimeScale(float scale)
    {
        timeScaleInsideSphere = scale;
    }

    private void SetSize(float size)
    {
        sphereSize = size;
        UpdateBubbleScaleSize(0f);
    }

    private void SetDuration(float duration)
    {
        effectDuration = duration;
    }

    public void SetSphereStats(float size, float slowFactor, float effectDuration)
    {
        SetSize(size);
        SetTimeScale(slowFactor);
        SetDuration(effectDuration);
    }

    public void InitiateTimeSphere()
    {
        SetMaterial();
        bubbleMaterialInstance = bubbleRenderer.material;
        UpdateBubbleScaleSize(0f);
        isGrowing = true;
        isInDilation = true;

        if (effectDuration > 0)
            StartCoroutine(RestoreTimeAfterDuration());
    }

    private IEnumerator RestoreTimeAfterDuration()
    {
        yield return new WaitForSeconds(effectDuration);

        if (objectsInsideSphere.Count > 0)
        {
            List<ObjectTimeManager> objectsToRemove = new List<ObjectTimeManager>();

            foreach (var timeManager in objectsInsideSphere)
            {
                if (timeManager == null) continue;

                timeManager.ResetTimeScale();
                objectsToRemove.Add(timeManager);
            }

            foreach (var timeManager in objectsToRemove)
            {
                objectsInsideSphere.Remove(timeManager);
            }
        }

        Destroy(gameObject);
    }


    private void Update()
    {
        if (isGrowing)
        {
            currentGrowthTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentGrowthTime / growthDuration);
            float curveValue = growthCurve.Evaluate(progress);
            float size = Mathf.Lerp(0f, sphereSize, curveValue);
            UpdateBubbleScaleSize(size);

            // Stop growing when done
            if (progress >= 1f)
            {
                isGrowing = false;
            }
        }

        if (!isInDilation) return;
        SetTimeDilation();
        CheckObjectsStillInRange();
    }

    private void SetTimeDilation()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, sphereSize);

        foreach (Collider col in objectsInRange)
        {
            ObjectTimeManager timeManager = col.GetComponent<ObjectTimeManager>();
            if (timeManager != null && !objectsInsideSphere.Contains(timeManager))
            {
                timeManager.SetTimeScale(timeScaleInsideSphere);
                objectsInsideSphere.Add(timeManager);
            }
        }
    }

    private void CheckObjectsStillInRange()
    {
        if (objectsInsideSphere.Count > 0)
        {
            List<ObjectTimeManager> objectsToRemove = new List<ObjectTimeManager>();

            foreach (var timeManager in objectsInsideSphere)
            {
                if (timeManager == null) continue;

                if (Vector3.Distance(timeManager.transform.position, transform.position) >= sphereSize * sphereSizeDistanceOffsetThreshold)
                {
                    timeManager.ResetTimeScale();
                    objectsToRemove.Add(timeManager);  
                }
            }

            foreach (var timeManager in objectsToRemove)
            {
                objectsInsideSphere.Remove(timeManager);
            }

        }
    }

    private void UpdateBubbleScaleSize(float size)
    {
        Vector3 newSize = new Vector3(size, size, size);
        transform.localScale = newSize;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, sphereSize);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, sphereSize * sphereSizeDistanceOffsetThreshold);

        }
    }

    private void OnValidate()
    {
        UpdateBubbleScaleSize(sphereSize);
    }

}