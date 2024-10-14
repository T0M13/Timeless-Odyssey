using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Header("Turret Settings")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint; 
    [SerializeField] private int poolSize = 50;
    [SerializeField] private float shootInterval = 0.5f;

    [SerializeField] private Queue<GameObject> laserPool = new Queue<GameObject>();
    [SerializeField] private float shootTimer;

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject laser = Instantiate(laserPrefab);
            laser.SetActive(false);
            laserPool.Enqueue(laser);
        }
    }

    private void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            ShootLaser();
            shootTimer = 0f;
        }
    }

    private void ShootLaser()
    {
        if (laserPool.Count == 0)
        {
            Debug.LogWarning("Out of lasers in the pool!");
            return;
        }

        GameObject laser = laserPool.Dequeue();
        laser.transform.position = firePoint.position;
        laser.transform.rotation = firePoint.rotation;
        laser.SetActive(true);

        LaserShot laserScript = laser.GetComponent<LaserShot>();
        StartCoroutine(ReturnToPoolAfterLifetime(laser));
    }

    private IEnumerator ReturnToPoolAfterLifetime(GameObject laser)
    {
        yield return new WaitForSeconds(laser.GetComponent<LaserShot>().Lifetime);
        laser.SetActive(false);
        laserPool.Enqueue(laser);
    }

    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(firePoint.position, firePoint.forward * 2);
        }
    }
}
