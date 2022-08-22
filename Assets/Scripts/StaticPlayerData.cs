using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPlayerData : MonoBehaviour
{
    public string nickname;
    public bool IsGameStarted = false;

    static public StaticPlayerData SingleTone;

    StaticPlayerData() {  }

    private void Awake()
    {
        SingleTone = this;  
        DontDestroyOnLoad(this);
    }
}

