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
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
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

    public override void OnStartClient()
    {
        if(!isOwned || !isClientOnly) return;

        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned || !isClientOnly) return;

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    #endregion
}
