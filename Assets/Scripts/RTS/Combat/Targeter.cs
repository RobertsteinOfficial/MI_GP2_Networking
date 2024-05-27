using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;
    public Targetable Target { get { return target; } }

    #region Server

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

    #endregion
}
