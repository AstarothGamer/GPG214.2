using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BulletShooter : MonoBehaviour
{
    public GameObject bulletPrefab;  
    public Transform firePoint;      
    public float bulletSpeed = 20f; 
    public float fireRate = 40f;     
    public float bulletLifetime = 5f; 
    public int poolSize = 200;       
    private float fireCooldown = 0f;

    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            fireCooldown -= Time.deltaTime;

            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }
        }
        else
        {
            fireCooldown = 0f;
        }
    }

    void Shoot()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * bulletSpeed;
            }

            StartCoroutine(DisableBulletAfterTime(bullet, bulletLifetime));
        }
        else
        {
            Debug.LogWarning("Bullet pool exhausted.");
        }
    }

    private IEnumerator DisableBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}
