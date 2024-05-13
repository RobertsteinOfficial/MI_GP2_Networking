using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{

    [SerializeField]
    private string _name = "Missing Name";

    [Server]
    public void SetDisplayName(string newName)
    {
        _name = newName;
    }


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        player.SetDisplayName($"Player {numPlayers}");


        Color displayColour = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
            );

        player.SetDisplayColour(displayColour);
    }
}
