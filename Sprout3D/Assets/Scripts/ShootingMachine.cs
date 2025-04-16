using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class ShootingMachine : MonoBehaviour, IActivatable
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float projectileSpeed = 10f;
    private bool isActive = true;

    void Start()
    {
        InvokeRepeating(nameof(FireProjectile), 1f, fireRate);
    }

    void FireProjectile()
    {
        if (!isActive) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<Projectile>().Initialize(projectileSpeed);
    }

    public void Activate()
    {
        isActive = true;
        InvokeRepeating(nameof(FireProjectile), 1f, fireRate);
    }

    public void Deactivate()
    {
        isActive = false;
        CancelInvoke(nameof(FireProjectile));
    }
}
