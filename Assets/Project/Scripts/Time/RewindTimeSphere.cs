using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindTimeSphere : MonoBehaviour
{
    [Header("Time Sphere Settings")]
    [SerializeField] protected float sphereSize = 25f;
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float rewindSpeed = .03f;
    [SerializeField] protected bool showGizmos = true;
    [SerializeField] protected List<RewindProxy> objectsInsideSphere = new List<RewindProxy>();
    [SerializeField] protected bool isDestroyed = false;
    [SerializeField] protected bool destoryAfter = true;

    [Header("Growth Settings")]
    [SerializeField] protected float growthDuration = 1f;
    [SerializeField] protected float currentGrowthTime = 0f;
    [SerializeField] protected bool isGrowing = false;
    [SerializeField]
    protected AnimationCurve growthCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.7f, 0.9f),
        new Keyframe(1f, 1f)
    );

    [Header("Shader Settings")]
    [SerializeField] protected Renderer bubbleRenderer;
    [SerializeField] protected Material bubbleMaterialInstance;

    public float SphereSize { get => sphereSize; set => sphereSize = value; }
    public Material BubbleMaterialInstance { get => bubbleMaterialInstance; set => bubbleMaterialInstance = value; }

    public event Action<float> OnSphereSizeChanged;

    private void Awake()
    {
        SetMaterial();
    }

    private void Start()
    {
        InitiateTimeSphere();
    }

    protected virtual void SetMaterial()
    {
        bubbleMaterialInstance = bubbleRenderer.material;
    }

    protected virtual void SetSize(float size)
    {
        sphereSize = size;
        UpdateBubbleScaleSize(0f);
    }

    protected virtual void SetRewindScale(float recordInterval)
    {
        this.recordInterval = recordInterval;
    }

    protected virtual void SetRewindSpeed(float rewindSpeed)
    {
        this.rewindSpeed = rewindSpeed;
    }

    protected virtual void SetDuration(float duration)
    {
        //effectDuration = duration;
    }

    public virtual void SetSphereStats(float size, float recordInterval, float rewindSpeed)
    {
        SetSize(size);
        SetRewindScale(recordInterval);
        SetRewindSpeed(rewindSpeed);
    }

    public virtual void InitiateTimeSphere()
    {
        SetMaterial();
        UpdateBubbleScaleSize(0f);
        isGrowing = true;
        RewindObjects();
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
            OnSphereSizeChanged?.Invoke(size);

            if (progress >= 1f)
            {
                isGrowing = false;
            }
        }

        CheckRewindingObjects();
    }

    protected virtual void RewindObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereSize);

        foreach (Collider nearbyObject in colliders)
        {
            RewindProxy proxy = nearbyObject.GetComponent<RewindProxy>();
            if (proxy != null)
            {
                proxy.IsRewinding = true;
                objectsInsideSphere.Add(proxy);
            }
        }
    }

    protected virtual void CheckRewindingObjects()
    {
        if (objectsInsideSphere.Count > 0)
        {
            List<RewindProxy> objectsToRemove = new List<RewindProxy>();

            foreach (var proxy in objectsInsideSphere)
            {
                if (proxy == null || !proxy.IsRewinding)
                {
                    objectsToRemove.Add(proxy);
                }
            }

            foreach (var proxy in objectsToRemove)
            {
                objectsInsideSphere.Remove(proxy);
            }
        }

        if (objectsInsideSphere.Count == 0)
        {
            DestroyTimeSphere();
        }
    }

    private void DestroyTimeSphere()
    {
        if (!destoryAfter) return;
        if (isDestroyed) return;
        isDestroyed = true;
        Destroy(gameObject);
    }

    //private IEnumerator StopRewindAfterDuration()
    //{
    //    yield return new WaitForSeconds(effectDuration);

    //    foreach (var proxy in objectsInsideSphere)
    //    {
    //        if (proxy != null)
    //        {
    //            proxy.IsRewinding = false; 
    //        }
    //    }

    //    DestroyTimeSphere();
    //}

    protected virtual void UpdateBubbleScaleSize(float size)
    {
        Vector3 newSize = new Vector3(size, size, size);
        transform.localScale = newSize;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, sphereSize);
        }
    }

    protected virtual void OnValidate()
    {
        UpdateBubbleScaleSize(sphereSize);
    }
}
