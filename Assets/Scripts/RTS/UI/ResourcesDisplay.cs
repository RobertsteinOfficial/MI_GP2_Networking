using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText;
    private RTSPlayer player;


    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        player.ClientActionResourceUpdated += ClientHandleResourcesUpdated;
    }


    private void OnDisable()
    {
        player.ClientActionResourceUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources : {resources}";
    }
}
