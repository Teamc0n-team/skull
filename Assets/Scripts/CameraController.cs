using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    public GameObject LookAt;

    [SerializeField]
    public GameObject DefaultLookAt;

    [SerializeField]
    public Vector3 Offset;

    [SerializeField]
    float Speed = 10.0f;

    bool UseLookAt = false;

    static public CameraController instance;

    [SerializeField]
    float LookAtFow = 4.0f;

    float DefaultFow = 10.0f;

    [SerializeField]
    Quaternion DefaultRotation = Quaternion.identity;
    [SerializeField]
    Vector3 DefaultPos;

    CameraController()
    {
        instance = this;
    }

    private void Start()
    {
        DefaultFow = Camera.main.fieldOfView;
        DefaultRotation = transform.rotation;
        DefaultPos = transform.position;
    }

    private void Update()
    {
        float H = Input.GetAxisRaw("Horizontal");
        float V = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(H, V, 0.0f);

        if (UseLookAt)
        {
            transform.Translate(movement * Speed * Time.deltaTime);
            transform.LookAt(LookAt.transform);
        }
        else
        {
            transform.Translate(movement * Speed * Time.deltaTime);
            ///transform.LookAt(DefaultLookAt.transform);
            if(LookAt != null)
            {
                transform.position = new Vector3(LookAt.transform.position.x, Offset.y, LookAt.transform.position.z);
                LookAt = null;
            }

            transform.rotation = Quaternion.Lerp(DefaultRotation, transform.rotation, Time.deltaTime / 2);
            transform.position = new Vector3(transform.position.x + Offset.x, Offset.y, transform.position.z + Offset.z);
        }

        
        if (UseLookAt)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, LookAtFow, Speed * Time.deltaTime);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, DefaultFow, Speed * Time.deltaTime);
        }
    }

    public void LookOnSelected(GameObject LookAt)
    {
        UseLookAt = true;
        this.LookAt = LookAt;
    }

    public void DisableLookAt() => UseLookAt = false;

}
