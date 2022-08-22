using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField]
    public Vector3 Offset;

    [SerializeField]
    Vector3 SelectedPosition;
    [SerializeField]
    Vector3 OldPos;

    bool IsSelected = false;

    [SerializeField]
    ButtonEventHandler Handler;

    [SerializeField]
    int nInx = 0;

    void Start()
    {
        OldPos = transform.position;
        SelectedPosition = transform.position + Offset;
    }

    void Update()
    {
        if(IsSelected)
        {
            transform.position = Vector3.Lerp(transform.position, SelectedPosition, Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, OldPos, Time.deltaTime);
        }
    }

    private void OnMouseEnter()
    {
        IsSelected = true;
    }

    private void OnMouseDown()
    {
        Handler.OnButtonPressed(nInx);
    }

    private void OnMouseExit()
    {
        IsSelected = false;
    }
}
