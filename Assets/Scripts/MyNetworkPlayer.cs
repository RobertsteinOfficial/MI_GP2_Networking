using Mirror;
using Org.BouncyCastle.Asn1.Cmp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))]
    [SerializeField]
    private string displayName = "Missing Name";

    [SyncVar(hook = nameof(HandleDisplayColourUpdated))]
    [SerializeField]
    private Color displayColour = Color.black;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TextMeshProUGUI nameText;

    #region Server

    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

    public void SetDisplayColour(Color newDisplayColour)
    {
        displayColour = newDisplayColour;
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        RpcLogNewName(displayName);

        SetDisplayName(newDisplayName);
    }

    #endregion

    #region Client


    private void HandleDisplayColourUpdated(Color oldColour, Color newColour)
    {
        meshRenderer.material.SetColor("_BaseColor", newColour);
    }

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        nameText.text = newName;
    }

    [ContextMenu("Set My Name")]
    private void SetMyName()
    {
        CmdSetDisplayName("My New Name");
    }

    
    [ClientRpc]
    private void RpcLogNewName(string newDisplayName)
    {
        Debug.Log(newDisplayName);
    }

    #endregion
}
