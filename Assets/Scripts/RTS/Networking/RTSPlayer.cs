using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;
    private Color teamColor = new Color();
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerUpdated))]
    private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandlePlayerNameUpdated))]
    private string playerName;


    public int Resources { get { return resources; } }
    public Color TeamColor { get { return teamColor; } }
    public List<Unit> MyUnits { get { return myUnits; } }
    public List<Building> MyBuildings { get { return myBuildings; } }
    public bool IsPartyOwner { get { return isPartyOwner; } }
    public string PlayerName { get { return playerName; } }



    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private LayerMask buildingObstacleLayer;
    [SerializeField] private float buildingRangeLimit = 5f;

    public static event Action<bool> AuthorityOnPartyOwnerUpdated;
    public event Action<int> ClientActionResourceUpdated;
    public static event Action ClientOnPlayerInfoUpdated;

    #region Server

    [Server]
    public void SetIsPartyOwner(bool value)
    {
        isPartyOwner = value;
    }

    [Server]
    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        //if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {

        //if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myBuildings.Remove(building);
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center,
            buildingCollider.size / 2,
            Quaternion.identity,
            buildingObstacleLayer))
            return false;

        foreach (Building building in myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach (Building building in buildings)
        {
            if (building.Id == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) return;

        if (resources < buildingToPlace.Price) return;

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, point)) return;

        GameObject buildingInstace = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstace, connectionToClient);
        SetResources(resources - buildingToPlace.Price);
    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    [Command]
    public void CmdStartGame()
    {
        if (!IsPartyOwner) return;

        ((RTSNetworkManager)NetworkManager.singleton).StartGame();
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) return;

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) return;

        ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopClient()
    {
        ClientOnPlayerInfoUpdated?.Invoke();

        if (!isClientOnly) return;

        ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!isOwned) return;

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }

    private void AuthorityHandlePartyOwnerUpdated(bool oldState, bool newState)
    {
        if (!isOwned) return;

        AuthorityOnPartyOwnerUpdated?.Invoke(newState);
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientActionResourceUpdated?.Invoke(newResources);
    }

    private void ClientHandlePlayerNameUpdated(string oldPlayerName, string newPlayerName)
    {
        ClientOnPlayerInfoUpdated?.Invoke();
    }

    #endregion
}
