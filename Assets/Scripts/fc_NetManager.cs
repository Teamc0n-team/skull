using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/
public class fc_NetManager : NetworkRoomManager
{
    // Overrides the base singleton so we don't
    // have to cast to this type everywhere.
    public static new fc_NetManager singleton { get; private set; }

    public bool IsLobby = false;
    public Transform LobbyStartPos;

    [SerializeField]
    public
    bool IsGameStated = false;

    public void GameStarted()
    {
        IsGameStated = true;
        ServerChangeScene(GameplayScene);
    }

    #region Unity Callbacks

    public override void OnValidate()
    {
        base.OnValidate();
    }


    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        singleton = this;
        base.Start();
    }


    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region Start & Stop

    public void StartClient(string Address)
    {
        this.networkAddress = Address;
        StartClient();
    }

    public void StartHost()
    {
        base.StartHost();
    }


    public override void ConfigureHeadlessFrameRate()
    {
        base.ConfigureHeadlessFrameRate();
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

#endregion

#region Scene Management

    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerChangeScene(string newSceneName) { }

    public override void OnServerSceneChanged(string sceneName) { }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) { }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }

#endregion

#region Server System Callbacks

    public override void OnServerConnect(NetworkConnectionToClient conn) { }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = gameObject;
        Transform startPos = GetStartPosition();
        if (playerPrefab != null)
        {
            player = startPos != null
                 ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                 : Instantiate(playerPrefab);
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        }
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }


    public override void OnServerError(NetworkConnectionToClient conn, Exception exception) { }

#endregion

#region Client System Callbacks

    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
    }

    public override void OnClientNotReady() { }

    public override void OnClientError(Exception exception) { }

#endregion

#region Start & Stop Callbacks

    public override void OnStartHost() { }

    public override void OnStartServer() { }

    public override void OnStartClient() { }

    public override void OnStopHost() { }

    public override void OnStopServer() { }

    public override void OnStopClient() { }

        #endregion
}
