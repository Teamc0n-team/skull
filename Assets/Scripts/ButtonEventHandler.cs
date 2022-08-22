using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEventHandler : MonoBehaviour
{
    delegate void OnPressed(int nInx);

    public virtual void OnButtonPressed(int nInx)
    {
        Debug.Log($"Button pressed: {nInx} gameobject: {gameObject.name}");
    }

}
