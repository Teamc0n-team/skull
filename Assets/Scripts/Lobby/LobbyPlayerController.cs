using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyPlayerController : NetworkRoomPlayer
{

    [SyncVar]
    public string nick_name;
    [SyncVar]
    public int player_id;

    void Start()
    {
        if (LobbyUIController.Singletone == null) return;

        if(isServer)
        {
            player_id = LobbyUIController.Singletone.Counter;
            LobbyUIController.Singletone.Counter++;
        }

        if(isLocalPlayer)
        {
            CmdInstallNick(StaticPlayerData.SingleTone.nickname);
            LobbyUIController.Singletone.AddPlayer(StaticPlayerData.SingleTone.nickname);
            LobbyUIController.Singletone.CurrentPlayerController = this;
        }
    }

    [Command]
    void CmdInstallNick(string nick)
    {

        nick_name = nick;

    }

    void UpdateNick(string nick)
    {
        gameObject.name = nick;
    }

    void Update()
    {
        UpdateNick(nick_name);
    }

    private void OnDestroy()
    {
        if(LobbyUIController.Singletone != null)
            LobbyUIController.Singletone.PlayerRemove(player_id);
    }
}
