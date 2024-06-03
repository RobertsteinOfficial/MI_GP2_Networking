using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent;
    [SerializeField] private TMP_Text winnerNameText;

    private void OnEnable()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDisable()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text = $"{winner} wins";

        gameOverDisplayParent.SetActive(true);
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
