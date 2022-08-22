using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sk_InputManager : MonoBehaviour
{
    static public sk_InputManager Singletone;

    public utils.UpdaterDelegate Updater;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Updater = utils.EmptyUpdate;
    }

    public Vector2 m_MousePos;
    public Vector2 m_Move;

    sk_InputManager()
    {
        Singletone = this;
    }

    public void Enable()
    {
        Updater = WorkUpdater;
    }

    void WorkUpdater()
    {
        m_MousePos.x = Input.GetAxisRaw("Mouse X");
        m_MousePos.y = Input.GetAxisRaw("Mouse Y");

        m_Move.x = Input.GetAxis("Horizontal");
        m_Move.y = Input.GetAxis("Vertical");
    }

    private void Update()
    {
        Updater();
    }


}
