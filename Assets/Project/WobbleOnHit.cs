using System.Collections;
using UnityEngine;

public class WobbleEffectOnHit : MonoBehaviour
{
    [Header("Shader Settings")]
    Renderer _renderer;
    Material _material;
    [SerializeField] AnimationCurve _DisplacementCurve;
    [SerializeField] float _DisplacementMagnitude;
    [SerializeField] float _LerpSpeed;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 hitPos = other.ClosestPoint(transform.position);
        HitShield(hitPos);
    }

    public void HitShield(Vector3 hitPos)
    {
        _renderer.material.SetVector("_HitPosition", hitPos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());
    }

    IEnumerator Coroutine_HitDisplacement()
    {
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_DisplacementStrength", _DisplacementCurve.Evaluate(lerp) * _DisplacementMagnitude);
            lerp += Time.deltaTime * _LerpSpeed;
            yield return null;
        }
    }
}
