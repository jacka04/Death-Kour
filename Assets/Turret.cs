using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform  firePoint;
    [SerializeField] private float      fireInterval = 2f;

    private void Start()
    {
        StartCoroutine(ShootLoop());
    }

    private IEnumerator ShootLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireInterval);
            Shoot();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // La dirección es simplemente hacia donde apunta el FirePoint
        // Rota el GameObject de la torreta en el editor para cambiar la dirección
        Vector3 dir = firePoint.right;

        GameObject go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        go.GetComponent<TurretBullet>().Launch(dir);
    }
}