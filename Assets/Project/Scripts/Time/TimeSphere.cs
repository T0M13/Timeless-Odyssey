using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSphere : MonoBehaviour
{
    [Header("Time Sphere Settings")]
    [SerializeField] private float timeScaleInsideSphere = 0.5f;
    [SerializeField] private float sphereSize = 5f;
    [SerializeField] private float startDuration = .1f;
    [SerializeField] private bool isInDilation = false;
    [SerializeField] private bool showGizmos = true;
    private HashSet<ObjectTimeManager> objectsInsideSphere = new HashSet<ObjectTimeManager>();

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


    public float SphereSize { get => sphereSize; set => sphereSize = value; }
    public float TimeScaleInsideSphere { get => timeScaleInsideSphere; set => timeScaleInsideSphere = value; }

    public void SetMaterial()
    {
        bubbleMaterialInstance = bubbleRenderer.material;
    }

    public void SetTimeScale(float scale)
    {
        timeScaleInsideSphere = scale;
    }

    public void SetSize(float size)
    {
        sphereSize = size;
        UpdateBubbleScaleSize(0f);
    }

    private void Start()
    {
        StartCoroutine(WaitBeforeDilation());
    }

    private IEnumerator WaitBeforeDilation()
    {
        bubbleMaterialInstance = bubbleRenderer.material;
        UpdateBubbleScaleSize(0f);
        yield return new WaitForSeconds(startDuration);
        isGrowing = true;
        isInDilation = true;
    }

    private void Update()
    {
        if (isGrowing)
        {
            currentGrowthTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentGrowthTime / growthDuration);
            float curveValue = growthCurve.Evaluate(progress);  // Use the custom curve
            float size = Mathf.Lerp(0f, sphereSize, curveValue);  // Apply the curve to the size
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
        List<ObjectTimeManager> objectsToRemove = new List<ObjectTimeManager>();

        foreach (var timeManager in objectsInsideSphere)
        {
            if (timeManager == null) continue;

            if (Vector3.Distance(timeManager.transform.position, transform.position) > sphereSize)
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

    private void UpdateBubbleScaleSize(float size)
    {
        Vector3 newSize = new Vector3(size , size , size);
        transform.localScale = newSize;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, sphereSize);
        }
    }

    private void OnValidate()
    {
        UpdateBubbleScaleSize(sphereSize);
    }

}