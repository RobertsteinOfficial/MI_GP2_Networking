using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] UnitMovement unitMovement;
    public UnitMovement Movement { get { return unitMovement; } }


    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;

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


    #endregion
}
