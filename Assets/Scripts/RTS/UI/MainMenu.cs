using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        MyNetworkManager.singleton.StartHost();
    }
}
