using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_OnHover : MonoBehaviour
{
    bool Animate = false;

    [SerializeField]
    AnimationInfo animationInfo;

    private void OnMouseEnter()
    {
        Animate = true;
    }

    private void OnMouseExit()
    {
        Animate = false;
    }

    private void Update()
    {
        UIAnimator.Animate(Animate, transform, ref animationInfo, 5.0f);
    }
}
