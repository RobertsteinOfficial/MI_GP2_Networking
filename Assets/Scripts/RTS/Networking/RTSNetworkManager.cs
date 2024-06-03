using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] GameObject unitSpawnerPrefab;
    [SerializeField] GameOverHandler gameOverHandlerPrefab;

    public override void OnServerSceneChanged(string newSceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map_"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnerInstance =
            Instantiate(unitSpawnerPrefab,
            conn.identity.transform.position,
            conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }
}
