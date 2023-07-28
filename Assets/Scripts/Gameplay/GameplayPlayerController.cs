using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameplayPlayerController : NetworkBehaviour    
{

    [SerializeField]
    public GameObject ModelGameObject;

    public utils.UpdaterDelegate updater;

    [SerializeField]
    [SyncVar]
    public string sName;

    [SerializeField]
    sk_PlayerMovment movment;

    public void Start()
    {
        movment = GetComponent<sk_PlayerMovment>();

        if (StaticPlayerData.SingleTone.IsGameStarted)
        {
            LobbyUIController.Singletone = null;
            GetComponent<LobbyPlayerController>().enabled = false;
            updater = WorkUpdater;
            ModelGameObject.SetActive(true);
            if (isLocalPlayer)
            {
                CmdInstallNick(StaticPlayerData.SingleTone.nickname);
                sk_InputManager.Singletone.Enable();

                movment.Enable();
                sk_CameraControler.Singletone.SetupPlayer(gameObject);
            }
        }
        else
        {
            ModelGameObject.SetActive(false);
            updater = utils.EmptyUpdate;
        }
    }

    [Command]
    public void CmdInstallNick(string NickName)
    {
        sName = NickName;
    }

    public void GameStarted()
    {
    }

    [Command(requiresAuthority = false)]
    void CmdActivateModel(bool bState)
    {
        ModelGameObject.SetActive(bState);
    }

    void WorkUpdater()
    {

    }

    private void Update()
    {
        updater();   
    }

}
