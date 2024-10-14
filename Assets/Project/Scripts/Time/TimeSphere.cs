using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSphere : MonoBehaviour
{
    [Header("Time Sphere Settings")]
    [SerializeField] protected float timeScaleInsideSphere = 0.5f;
    [SerializeField] protected float sphereSize = 5f;
    [SerializeField] protected float effectDuration = 5f;
    [SerializeField] protected bool isInDilation = false;
    [SerializeField] protected bool setDilationOnStart = false;
    [SerializeField] protected bool showGizmos = true;
    [SerializeField] protected List<ObjectTimeManager> objectsInsideSphere = new List<ObjectTimeManager>();
    [SerializeField] protected float sphereSizeDistanceOffsetThreshold = 1f;

    [Header("Growth Settings")]
    [SerializeField] protected float growthDuration = 1f;
    [SerializeField] protected float currentGrowthTime = 0f;
    [SerializeField] protected bool isGrowing = false;
    [SerializeField]
    protected AnimationCurve growthCurve = new AnimationCurve(
  new Keyframe(0f, 0f),
  new Keyframe(0.7f, 0.9f),
  new Keyframe(1f, 1f));

    [Header("Shader Settings")]
    [SerializeField] protected Renderer bubbleRenderer;
    [SerializeField] protected Material bubbleMaterialInstance;

    protected virtual void Start()
    {
        if (!setDilationOnStart) return;
        InitiateTimeSphere();
    }

    protected virtual void SetMaterial()
    {
        bubbleMaterialInstance = bubbleRenderer.material;
    }

    protected virtual void SetTimeScale(float scale)
    {
        timeScaleInsideSphere = scale;
    }

    protected virtual void SetSize(float size)
    {
        sphereSize = size;
        UpdateBubbleScaleSize(0f);
    }

    protected virtual void SetDuration(float duration)
    {
        effectDuration = duration;
    }

    public virtual void SetSphereStats(float size, float slowFactor, float effectDuration)
    {
        SetSize(size);
        SetTimeScale(slowFactor);
        SetDuration(effectDuration);
    }

    public virtual void InitiateTimeSphere()
    {
        SetMaterial();
        bubbleMaterialInstance = bubbleRenderer.material;
        UpdateBubbleScaleSize(0f);
        isGrowing = true;
        isInDilation = true;

        if (effectDuration > 0)
            StartCoroutine(RestoreTimeAfterDuration());
    }

    protected virtual IEnumerator RestoreTimeAfterDuration()
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


    protected virtual void Update()
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

    protected virtual void SetTimeDilation()
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

    protected virtual void CheckObjectsStillInRange()
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

    protected virtual void UpdateBubbleScaleSize(float size)
    {
        Vector3 newSize = new Vector3(size, size, size);
        transform.localScale = newSize;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, sphereSize);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, sphereSize * sphereSizeDistanceOffsetThreshold);

        }
    }

    protected virtual void OnValidate()
    {
        UpdateBubbleScaleSize(sphereSize);
    }

}