using System.Collections;
using UnityEngine;

public class LaserShot : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private float speed = 50f;
    [SerializeField] private float lifetime = 2f;

    [SerializeField] private bool isActive = false;

    public float Lifetime { get => lifetime; set => lifetime = value; }
    public float Speed { get => speed; set => speed = value; }

    private void OnEnable()
    {
        isActive = true;
        StartCoroutine(DisableAfterLifetime());
    }

    private void Update()
    {
        if (!isActive) return;

        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    private IEnumerator DisableAfterLifetime()
    {
        yield return new WaitForSeconds(Lifetime);
        DisableLaser("Lifetime expired");
    }

    private void OnCollisionEnter(Collision collision)
    {
        DisableLaser("Hit object: " + collision.gameObject.name);
    }

    private void DisableLaser(string reason)
    {
        Debug.Log("Laser disabled - Reason: " + reason);

        isActive = false;
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 2);
    }
}
