using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    [ServerCallback]

    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            player.SetResources(player.Resources + resourcesPerInterval);
            timer += interval;
        }
    }

    public override void OnStartServer()
    {
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        health.ServerOnDie += ServeHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServeHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }

    private void ServeHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
}
