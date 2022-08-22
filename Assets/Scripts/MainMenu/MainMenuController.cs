using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{

    [SerializeField]
    AnimationInfo StartPanelAnimationInfo;

    [SerializeField]
    GameObject StartPanelGameObject;
    [SerializeField]
    bool IsStartPanelActive = false;

    [SerializeField]
    float Speed;

    [SerializeField]
    TMP_InputField AddressInput;
    [SerializeField]
    TMP_InputField NickNameInput;

    private void Start()
    {
    }

    public void StartButtonHanlder()
    {
        IsStartPanelActive = !IsStartPanelActive;
    }

    public void Update()
    {
        UIAnimator.Animate(IsStartPanelActive, StartPanelGameObject.transform,  ref StartPanelAnimationInfo, Speed);
    }

    public void OnConnectButton()
    {
        StaticPlayerData.SingleTone.nickname = NickNameInput.text;
        fc_NetManager.singleton.StartClient(AddressInput.text);
    }

    
    public void OnHostButton()
    {
        StaticPlayerData.SingleTone.nickname = NickNameInput.text;
        fc_NetManager.singleton.StartHost();
    }

}
