using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;

public class GameDelegate : Singleton<GameDelegate>
{
    public int BackgroundIndex { get; set; }
    public int PlayerSelectedAvatarIndex { get; set; }
    public bool GamePause { get; set; }
    public int TotalPlayerCredit {
        get 
        { 
            return PlayerPrefs.GetInt("PlayerCredit", InitialCredit); 
        }
        set
        {   int credit = PlayerPrefs.GetInt("PlayerCredit", InitialCredit);
            credit += value;
            PlayerPrefs.SetInt("PlayerCredit", credit );
        }
    }
    public int InitialCredit = 100;
    public void Initialize()
    {
        BackgroundIndex = 0;
        PlayerSelectedAvatarIndex = 0;
        GamePause = false;
        Debug.Log("Delegate Initialize");
    }

}
