using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMap : MonoBehaviour
{
    public static BuildingMap Map;

    BuildingMap()
    {
        Map = this;
    }

    public void ApplyOnGameobject(GameObject obj)
    {
        MeshRenderer Renderer = obj.GetComponent<MeshRenderer>();
        Renderer.material.color = Color.green;
    }

}
