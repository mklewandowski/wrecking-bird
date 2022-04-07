using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public static bool PlayGame = false;

    public enum GameState {
        TitleScreen,
        GetReady,
        Playing,
        Dead,
        Score,
        ScoreRestart
    }
    public static GameState CurrentGameState = GameState.TitleScreen;

    // keep track of where we died so we can show death message
    public static float WreckedYpos = 0;

    // keep track of scoring
    public static int BestScore = 0;
    public static int CurrentScore = 0;

    public const string BestScorePlayerPrefsKey = "BestScore";
    public static void SaveToPlayerPrefs(string key, int val)
    {
        PlayerPrefs.SetInt(key, val);
    }
    public static int LoadFromPlayerPrefs(string key)
    {
        int val = PlayerPrefs.GetInt(key, 0);
        return val;
    }
}
