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

    private void Start()
    {
        StartCoroutine(WaitBeforeDilation());
    }

    private IEnumerator WaitBeforeDilation()
    {
        yield return new WaitForSeconds(startDuration);
        isInDilation = true;
    }

    private void Update()
    {
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

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, sphereSize);
        }
    }
}