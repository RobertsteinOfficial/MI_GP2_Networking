using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Unity.VisualScripting;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;
    public Targetable Target { get { return target; } }

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }


    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Command]
    public void CmdSetTarget(GameObject targetGO)
    {
        if (!targetGO.TryGetComponent<Targetable>(out Targetable newTarget)) return;

        target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }


    #endregion
}
