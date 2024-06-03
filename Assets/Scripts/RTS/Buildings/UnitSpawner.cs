using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject unitPrefab;
    [SerializeField] Transform unitSpawnPoint;
    [SerializeField] private Health health;


    #region Server

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    //public override void OnStartServer()
    //{
    //    health.ServerOnDie += ServerHandleDie;
    //}

    //public override void OnStopServer()
    //{
    //    health.ServerOnDie -= ServerHandleDie;
    //}

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!isOwned) return;

        CmdSpawnUnit();
    }

    #endregion
}
