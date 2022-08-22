using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sk_CameraControler : MonoBehaviour
{
    public static sk_CameraControler Singletone;

    sk_CameraControler()
    {
        Singletone = this;
    }

    public GameObject Player;
    Camera main;

    public float SensX = 200.0f;
    public float SensY = 200.0f;

    [SerializeField]
    Vector2 Rotation;

    [SerializeField]
    Vector2 _MousePos;

    public utils.UpdaterDelegate updater;

    private void Start()
    {
        main = Camera.main;
        updater = utils.EmptyUpdate;
    }

    public void SetupPlayer(GameObject Player)
    {
        this.Player = Player;
        updater = WorkingUpdater;
    }

    void WorkingUpdater()
    {
        main.transform.position = Player.transform.position;

        Rotation.y += sk_InputManager.Singletone.m_MousePos.x;


        Rotation.x -= sk_InputManager.Singletone.m_MousePos.y;
        Rotation.x = Mathf.Clamp(Rotation.x, -90f, 90f);

        transform.rotation = Quaternion.Euler(Rotation.x, Rotation.y, 0f);
    }

    void Update()
    {
        updater();
    }
}
