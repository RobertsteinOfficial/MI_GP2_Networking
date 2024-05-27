using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float bulletVelocity = 10;
    [SerializeField] private int damageToDeal = 20;

    private void Start()
    {
        body.velocity = transform.forward * bulletVelocity;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) return;
        }

        if (other.TryGetComponent<Health>(out Health health))
        {
            health.ReceiveDamage(damageToDeal);
        }

        DestroySelf();
    }
}
