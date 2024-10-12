using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSphere : MonoBehaviour
{
    [Header("Time Sphere Settings")]
    [SerializeField] private float timeScaleInsideSphere = 0.5f;
    [SerializeField] private float sphereSize = 5f;
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private bool showGizmos = true;

    private HashSet<ObjectTimeManager> objectsInsideSphere = new HashSet<ObjectTimeManager>();

    public float SphereSize { get => sphereSize; set => sphereSize = value; }
    public float TimeScaleInsideSphere { get => timeScaleInsideSphere; set => timeScaleInsideSphere = value; }

    public void SetTimeScale(float scale)
    {
        timeScaleInsideSphere = scale;
    }

    public void SetSize(float size)
    {
        sphereSize = size;
    }

    public void SetDuration(float duration)
    {
        effectDuration = duration;
    }

    public void SetTimeDilation()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, sphereSize);

        foreach (Collider col in objectsInRange)
        {
            ObjectTimeManager timeManager = col.GetComponent<ObjectTimeManager>();
            if (timeManager != null && !objectsInsideSphere.Contains(timeManager))
            {
                timeManager.SetTimeScale(timeScaleInsideSphere);
                objectsInsideSphere.Add(timeManager);
                StartCoroutine(RestoreTimeAfterDuration(timeManager));
            }
        }
    }

    private IEnumerator RestoreTimeAfterDuration(ObjectTimeManager timeManager)
    {
        yield return new WaitForSeconds(effectDuration);
        timeManager.ResetTimeScale();
        objectsInsideSphere.Remove(timeManager);
        Destroy(gameObject);
    }

    private void Update()
    {
        CheckObjectsStillInRange();
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

        if (objectsInsideSphere.Count == 0)
            Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, sphereSize);
        }
    }
}