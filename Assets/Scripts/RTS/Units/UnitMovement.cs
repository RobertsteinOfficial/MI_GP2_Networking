using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Targeter targeter;
    [SerializeField] private float fireRange = 10f;

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
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        agent.SetDestination(hit.position);
    }

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.Target;
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > fireRange * fireRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }

            return;
        }

        if (!agent.hasPath) return;
        if (agent.remainingDistance > agent.stoppingDistance) return;

        agent.ResetPath();
    }

    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    #endregion

    #region Client


    #endregion
}
