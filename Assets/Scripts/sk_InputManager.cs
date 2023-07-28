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

    private KeyCode JumpKeyCode = KeyCode.Space;

    public Vector2 m_MousePos;
    public Vector2 m_Move;

    public bool JumpPressed = false;
    sk_InputManager()
    {
        Singletone = this;
    }

    public void Enable()
    {
        Updater = WorkUpdater;

        ToogleMouse(false);
    }

    public void ToogleMouse(bool State)
    {
        if(State)
        Cursor.lockState = CursorLockMode.None;
        else
        Cursor.lockState = CursorLockMode.Locked;
    
        Cursor.visible = State;
    }

    bool MouseState = false;

    void WorkUpdater()
    {
        m_MousePos.x = Input.GetAxisRaw("Mouse X");
        m_MousePos.y = Input.GetAxisRaw("Mouse Y");

        m_Move.x = Input.GetAxis("Horizontal");
        m_Move.y = Input.GetAxis("Vertical");

        JumpPressed = Input.GetAxis("Jump") > 0;
        if(JumpPressed)
        {
            Debug.Log("jump");
        }
    }

    private void Update()
    {
        Updater();
    }


}
