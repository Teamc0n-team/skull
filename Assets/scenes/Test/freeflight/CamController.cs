using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    GameObject Player;

    utils.UpdaterDelegate updater;

    private void Start()
    {
        updater = utils.EmptyUpdate;
    }

    void Setup(GameObject Player)
    {
        this.Player = Player;
        transform.parent = Player.transform;
    }

    void Update()
    {

    }
}
