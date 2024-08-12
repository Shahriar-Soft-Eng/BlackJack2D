using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;

public class GameDelegate : Singleton<GameDelegate>
{
    public int LevelIndex { get; set; }
    public int TotalScore { get; set; }
    public int TotalCredit { get; set; }
    public int HandIndex { get; set; }
    public int HighScore { get; set; }
    public int PlayerScore { get; set; }
    public string PlayerName { get; set; }
    public bool GamePause { get; set; }
    
    public void Initialize()
    {
        LevelIndex = 0;
        TotalScore = 0;
        TotalCredit = 0;
        HighScore = 0;
        GamePause = false;
        PlayerName = "";
        PlayerScore = 0;
        HandIndex = 0;
        Debug.Log("Delegate Initialize");
    }
    public int GetCoin()
    {
        return PlayerPrefs.GetInt("TotalCoin",0);
    }   
    public void SetCoin(int coin)
    {
        int c = PlayerPrefs.GetInt("TotalCoin", 0);
        c += coin;
        PlayerPrefs.SetInt("TotalCoin",c);
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore",0);
    }
    public void SetHighScore(int currentScore)
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScore < currentScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
        }
    }
    public List<string> GetHighScoreList()
    {
        string strScores = PlayerPrefs.GetString("HighScores","null");
        string[] highScores = strScores.Split(',');
        List<string> listhighScores = highScores.ToList<string>();
        return listhighScores;
    }
    public void SetHighScoreIntoList(string playerName, int score)
    {
        string strScores = PlayerPrefs.GetString("HighScores", "null");
        if(strScores == "null")
        {
            string newData = $"{playerName}:{score},";
            PlayerPrefs.SetString("HighScores", newData);
            return;
        }
        string[] highScores = strScores.Split(',');
        List<string> listhighScores = highScores.Where(x => x != "").ToList();
        bool isHigher = false;
        for (int i = 0; i < listhighScores.Count; i++)
        {
            string[] data = listhighScores[i].Split(":");
            int compareScore = int.Parse(data[1]);
            if (score > compareScore)
            {
                string newData = $"{playerName}:{score}";
                listhighScores.Insert(i, newData);
                isHigher = true;
                break;
            }
            else
            {
                isHigher = false;
            }
        }
        if (!isHigher)
        {
            string newData = $"{playerName}:{score}";
            listhighScores.Add(newData);
        }
        string finalData = "";
        foreach (string data in listhighScores)
        {
            finalData += $"{data},";
        }
        PlayerPrefs.SetString("HighScores", finalData);
    }
}
