using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyUIController : NetworkBehaviour
{

    public static LobbyUIController Singletone;

    [System.Serializable]
    public struct PlayerViewFields
    {
        public TMPro.TMP_Text Nickname;
    }

    [System.Serializable]
    public struct PlayerViewData
    {
        public bool IsEmpty;
        public bool IsReady;
        public string Name;
    }

    [SerializeField]
    public PlayerViewFields[] PlayersView;
    public int Counter = 0;


    [SerializeField]
    public GameObject HostPanel;

    [SerializeField]
    public SyncList<PlayerViewData> LobbyPlayers = new SyncList<PlayerViewData>();

    public LobbyPlayerController CurrentPlayerController;

    bool CurrentIsReady = false;

    public void OnExitButtonPressed()
    {
        if(isServer)
        fc_NetManager.singleton.StopHost();
        else
        fc_NetManager.singleton.StopClient();
    }

    public void OnReadyButtonPressed()
    {
        CurrentIsReady = !CurrentIsReady;
        if (CurrentIsReady)
            PlayerReady(CurrentPlayerController.player_id);
        else
            PlayerNotReady(CurrentPlayerController.player_id);
    }

    public void OnStartButtonPressed()
    {
        if(IsAllPlayersReady())
        {

            StaticPlayerData.SingleTone.IsGameStarted = true;
            RpcGameStarted();

            fc_NetManager.singleton.ServerChangeScene(fc_NetManager.singleton.GameplayScene);
        }
    }

    [ClientRpc]
    public void RpcGameStarted()
    {
        StaticPlayerData.SingleTone.IsGameStarted = true;
    }

    private void Awake()
    {
        Singletone = this;
    }

    private void Start()
    {
        if(isServer)
        {
            HostPanel.SetActive(true);
        }
        else
        {
            HostPanel.SetActive(false);
        }
    }

    [Command(requiresAuthority = false)]
    public void AddPlayer(string NickName)
    {
        PlayerViewData newData = new PlayerViewData();
        newData.Name = NickName;
        newData.IsReady = false;
        newData.IsEmpty = false;

        for (int i = 0; i < LobbyPlayers.Count; i++)
        {
            if(LobbyPlayers[i].IsEmpty)
            {
                LobbyPlayers[i] = newData;
                return;
            }
        }

        LobbyPlayers.Add(newData);
    }

    [Command(requiresAuthority = false)]
    public void PlayerReady(int Id)
    {
        if (LobbyPlayers[Id].IsReady) return;
        PlayerViewData newData = new PlayerViewData();
        newData.Name = LobbyPlayers[Id].Name;
        newData.IsReady = true;
        newData.IsEmpty = false;
        LobbyPlayers[Id] = newData;
    }

    [Command(requiresAuthority = false)]
    public void PlayerNotReady(int Id)
    {
        if (!LobbyPlayers[Id].IsReady) return;
        PlayerViewData newData = new PlayerViewData();
        newData.Name = LobbyPlayers[Id].Name;
        newData.IsReady = false;
        newData.IsEmpty = false;
        LobbyPlayers[Id] = newData;
    }

    [Command(requiresAuthority = false)]
    public void PlayerRemove(int Id)
    {
        PlayerViewData newData = new PlayerViewData();
        newData.Name = string.Empty;
        newData.IsReady = false;
        newData.IsEmpty = true;
        LobbyPlayers[Id] = newData;
    }
    void Update()
    {
        for (int i = 0; i < PlayersView.Length && i < LobbyPlayers.Count; i++)
        {
            //Debug.Log(LobbyPlayers[i].Name);
            PlayersView[i].Nickname.text = LobbyPlayers[i].Name;
            PlayersView[i].Nickname.color = LobbyPlayers[i].IsReady ? Color.green : Color.red;
        }
    }
    bool IsAllPlayersReady()
    {
        bool allPlayersReady = LobbyPlayers[0].IsReady;

        for(int i = 1; i < LobbyPlayers.Count; i++)
        {
            allPlayersReady &= LobbyPlayers[i].IsReady;
        }

        return allPlayersReady;
    }
}
