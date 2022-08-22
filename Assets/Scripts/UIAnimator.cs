using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AnimationInfo
{
    public Transform DefaultPos;
    public Transform ActivatedPos;
}

public class UIAnimator
{ 
   public static void Animate(bool IsActive, Transform TargetTransform, ref AnimationInfo Info, float nTime)
    {
        if(IsActive)
        {
            TargetTransform.position = Vector3.Lerp(TargetTransform.position, Info.ActivatedPos.position, nTime * Time.deltaTime);
            TargetTransform.localScale = Vector3.Lerp(TargetTransform.localScale, Info.ActivatedPos.localScale, nTime * Time.deltaTime);
            TargetTransform.rotation = Quaternion.Lerp(TargetTransform.rotation, Info.ActivatedPos.rotation, nTime * Time.deltaTime);
        }
        else
        {
            TargetTransform.position = Vector3.Lerp(TargetTransform.position, Info.DefaultPos.position, nTime * Time.deltaTime);
            TargetTransform.localScale = Vector3.Lerp(TargetTransform.localScale, Info.DefaultPos.localScale, nTime * Time.deltaTime);
            TargetTransform.rotation = Quaternion.Lerp(TargetTransform.rotation, Info.DefaultPos.rotation, nTime * Time.deltaTime);
        }

    }
}