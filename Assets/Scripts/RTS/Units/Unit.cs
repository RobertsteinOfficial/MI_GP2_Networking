using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] UnitMovement unitMovement;
    public UnitMovement Movement { get { return unitMovement; } }

    [SerializeField]
    private Targeter targeter;
    public Targeter Targeter { get { return targeter; } }
    [SerializeField] Health health;


    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    #region Server


    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleDie;

    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    [Client]
    public void Select()
    {
        if (!isOwned) return;
        onSelected?.Invoke();
    }


    [Client]
    public void DeSelect()
    {
        if (!isOwned) return;
        onDeselected?.Invoke();
    }


    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned) return;
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    #endregion
}
