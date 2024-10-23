using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemSphereHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimeSphere timeSphere;

    [Header("Particle Systems")]
    [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    private void Awake()
    {
        if (timeSphere == null)
            timeSphere = GetComponent<TimeSphere>();

        if (timeSphere != null)
        {
            timeSphere.OnSphereSizeChanged += UpdateSphereRadius;
        }
    }

    private void OnDestroy()
    {
        if (timeSphere != null)
        {
            timeSphere.OnSphereSizeChanged -= UpdateSphereRadius;
        }
    }

    private void UpdateSphereRadius(float newSize)
    {
        if (particleSystems.Count <= 0 || timeSphere == null) return;

        foreach (ParticleSystem p in particleSystems)
        {
            p.Stop();
            p.Clear();
            var shape = p.shape;
            shape.radius = newSize;

            var mainModule = p.main;
            mainModule.startColor = timeSphere.BubbleMaterialInstance.GetColor("_MainColor");

            p.Play();

        }
    }
 
}
