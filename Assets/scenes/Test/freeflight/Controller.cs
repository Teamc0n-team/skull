using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Controller : NetworkBehaviour
{
    private void Start()
    {
        if(isLocalPlayer)
        {
            sk_InputManager.Singletone.Enable();
            sk_CameraControler.Singletone.SetupPlayer(gameObject);
            sk_PlayerMovment movemnt = GetComponent<sk_PlayerMovment>();
            movemnt.Enable();
        }
    }

}
