using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class sk_PlayerMovment : NetworkBehaviour
{
    public float nSpeed = 0.3f;
    public float nJumpForce = 1f;

    private Rigidbody rb;
    private CapsuleCollider collider;

    utils.UpdaterDelegate updater;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Enable()
    {
        updater = WorkUpdate;
    }

    private Vector3 _movementVector
    {
        get
        {
            var horizontal = sk_InputManager.Singletone.m_Move.x;
            var vertical = sk_InputManager.Singletone.m_Move.y;

            return new Vector3(horizontal, 0.0f, vertical);
        }
    }

    void WorkUpdate()
    {
        CmdMove(_movementVector);
    }

    void CmdMove(Vector3 Movement)
    {
        rb.AddForce(Movement * nSpeed, ForceMode.Impulse);
    }

    private void Update()
    {
        updater();
    }

}
