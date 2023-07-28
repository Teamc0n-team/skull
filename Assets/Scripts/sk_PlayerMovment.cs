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

    [SerializeField]
    LayerMask surfaceLayerMask = 1;

    private void Awake()
    {
        updater = IdleUpdate;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Enable()
    {
        Debug.Log("Movement enabled");
        updater = WorkUpdate;
    }

    private Vector3 _movementVector
    {
        get
        {
            var horizontal = sk_InputManager.Singletone.m_Move.x;
            var vertical = sk_InputManager.Singletone.m_Move.y;

            return transform.forward * vertical + transform.right * horizontal;
        }
    }

    private bool IsGrounded
    {
        get
        {
            var bottomCenterPoint = new Vector3(collider.bounds.center.x, collider.bounds.min.y, collider.bounds.center.z);

            bool IsG = Physics.CheckCapsule(collider.bounds.center, bottomCenterPoint, collider.bounds.size.x / 2 * 0.9f, surfaceLayerMask);

            return IsG;
        }
    }

    void CmdJump()
    {
        if(IsGrounded && sk_InputManager.Singletone.JumpPressed)
        {
            rb.AddForce(Vector3.up * nJumpForce, ForceMode.Impulse);
        }
    }

    void IdleUpdate()
    {
    }

    void WorkUpdate()
    {
        CmdMove(_movementVector);
        CmdRotate();
        CmdJump();
    }

    void CmdMove(Vector3 Movement)
    {
        rb.AddForce(Movement * nSpeed, ForceMode.Impulse);
    }

    void CmdRotate()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, sk_CameraControler.Singletone.Rotation.y, transform.rotation.z);
    }

    private void FixedUpdate()
    {
        updater();
    }

}
