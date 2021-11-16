using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardData : ScriptableObject
{
    public string PlayerNickName;
    public int PlayerScore;
    public int playersConsumed;
    public int MinionsConsumed;
    public int PlayerDeaths;
    public bool IslocalPlayer;
    public Sprite EvolutionSprite;
    public bool AmITheDemonKing;
    public string currentModelName;
}
