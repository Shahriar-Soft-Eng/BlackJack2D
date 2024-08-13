using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;

public class GameDelegate : Singleton<GameDelegate>
{
    public int BackgroundIndex { get; set; }
    public int PlayerSelectedAvatarIndex 
    {
        get
        {
            return PlayerPrefs.GetInt("PlayerAvatar", 0);
        }
        set
        {
            PlayerPrefs.SetInt("PlayerAvatar", value);
        }
    }
    public bool GamePause { get; set; }
    public int TotalPlayerMoney {
        get 
        { 
            return PlayerPrefs.GetInt("PlayerCredit", InitialMoney); 
        }
        set
        {
            PlayerPrefs.SetInt("PlayerCredit", value);
        }
    }
    public int InitialMoney { get; private set; }
    public int RoundCount = 0;
    public void Initialize()
    {
        InitialMoney = 1000;
        BackgroundIndex = 0;
        GamePause = false;
        Debug.Log("Delegate Initialize");
    }

}
