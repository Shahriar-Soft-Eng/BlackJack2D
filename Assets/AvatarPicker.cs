using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AvatarPicker : MonoBehaviour
{
    public static Action<int> ActionCurrentAvatarIndex;
    [SerializeField] private Image imgAvatar;
    [SerializeField] private GameObject goSelectSprite;
    [SerializeField] private Button buttonAvatarSelect;

    private int index = 0;
    private bool isSelect = false;
    private void Start()
    {
        buttonAvatarSelect.onClick.AddListener(OnClickButtonAction);
    }
    public void Init(Sprite avatarSkin, int avatarIndex)
    {
        imgAvatar.sprite = avatarSkin;
        index = avatarIndex;
    }
    private void OnClickButtonAction()
    {
        SoundManager.Instance.Play("click");
        if (!goSelectSprite.gameObject.activeSelf) isSelect = false; //Because this 'goSelectSprite' can be Active/Deactive from another class. 
        if(!isSelect) 
        {//This logic for selected avatar
            isSelect = !isSelect;
            goSelectSprite.SetActive(isSelect);
            ActionCurrentAvatarIndex?.Invoke(index);
        }
        else
        {//This logic for Non selected avatar
            isSelect = !isSelect;
            goSelectSprite.SetActive(isSelect);
            ActionCurrentAvatarIndex?.Invoke(-1);
        }
    }
}
