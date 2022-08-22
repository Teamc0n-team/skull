using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlatform : MonoBehaviour
{
    [SerializeField]
    public Material OriginMat;
    [SerializeField]
    public Material SelectedMat;

    Renderer Renderer;

    [SerializeField]
    bool IsSelected = false;

    private void Start()
    {
        Renderer = GetComponent<Renderer>();
    }

    private void OnMouseDown()
    {
        IsSelected = !IsSelected;
        if(IsSelected)
        CameraController.instance.LookOnSelected(gameObject);
        else
        CameraController.instance.DisableLookAt();

    }

    private void OnMouseEnter()
    {
        if(!IsSelected)
        Renderer.material = SelectedMat;
    }

    private void OnMouseExit()
    {
        if(!IsSelected)
        Renderer.material = OriginMat;
    }

}
