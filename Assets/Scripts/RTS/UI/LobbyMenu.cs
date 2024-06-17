using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[0];

    private void OnEnable()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerUpdated += AuthorityOnPartyOwnerUpdated;
        RTSPlayer.ClientOnPlayerInfoUpdated += ClientHandlePlayerInfoUpdated;
    }

    
    private void OnDisable()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerUpdated -= AuthorityOnPartyOwnerUpdated;
        RTSPlayer.ClientOnPlayerInfoUpdated -= ClientHandlePlayerInfoUpdated;
    }
    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }
    private void AuthorityOnPartyOwnerUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    private void ClientHandlePlayerInfoUpdated()
    {
        List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).Players;

        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].PlayerName;
        }

        for (int i = players.Count; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for player...";
        }

        startGameButton.interactable = players.Count > 1;
    }


    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
