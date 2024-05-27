using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent;

    
    #region Server
    [Command]
    public void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        agent.SetDestination(hit.position);
    }

    [ServerCallback]
    private void Update()
    {
        if (!agent.hasPath) return;
        if(agent.remainingDistance > agent.stoppingDistance) return;

        agent.ResetPath();
    }

    #endregion

    #region Client


    #endregion
}
