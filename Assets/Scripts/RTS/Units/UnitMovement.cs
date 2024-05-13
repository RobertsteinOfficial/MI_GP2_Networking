using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Mirror;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent;


    #region Server
    [Command]
    private void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        agent.SetDestination(hit.position);
    }

    #endregion

    #region Client

    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (!Mouse.current.rightButton.wasPressedThisFrame) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, 100)) return;

        CmdMove(hit.point);
    }

    #endregion
}
