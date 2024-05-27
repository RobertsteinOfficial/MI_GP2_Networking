using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float range = 10f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFiredTime;

    [Server]
    private bool CanFire()
    {
        if (targeter.Target == null) return false;
        return (targeter.Target.transform.position - transform.position).sqrMagnitude <= range * range;
    }

    [ServerCallback]
    private void Update()
    {
        if (!CanFire()) return;

        Quaternion targetRotation = Quaternion.LookRotation(targeter.Target.transform.position - transform.position);
        turret.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / fireRate) + lastFiredTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(targeter.Target.AimPoint().position - muzzle.position);
            GameObject projectileInstance = Instantiate(projectilePrefab, muzzle.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFiredTime = Time.time;
        }

    }
}
