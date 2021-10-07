using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class DemonKingInGameAnalytics : MonoBehaviour
{
    public bool ShouldPostAnalytics = true;

    //END GAME LEADER BOARD INFO
    public float MatchDuration;             //Match Duration                    //

    public int DemonKingScore;              //Demon king score                  //
    public int PlayerConsumed;              //Number of players consumed        //
    public int MinionsConsumed;             //Number of minions consumed        //
    public int PlayerDeaths;                //Number of times player died       //

    public float TimeSpentAsLionKing;        //Lion time                        //
    public float TimeSpentAsRayKing;         //Ray time                         //
    public float TimeSpentAsDragonKing;      //Dragon time                      //

    //Player habits
    public int SlimeDamageOutput;           //Slime damage
    public int LionDamageOutput;            //Lion damage
    public int RayDamageOutput;             //Ray damage
    public int DragonDamageOutput;          //Dragon damage

    public float TimeSpentAsSlime;           //Slime time                       //
    public float TimeSpentAsLion;            //Lion time                        //
    public float TimeSpentAsRay;             //Ray time                         //
    public float TimeSpentAsDragon;          //Dragon time                      //

    public static DemonKingInGameAnalytics instance;

    public void SendEndGameStatsAnalytics()
    {
        if (!ShouldPostAnalytics)
            return;

        Dictionary<string, object> EndGameStatsData = new Dictionary<string, object>
        {
            {"Match Duration",MatchDuration },
            {"Demon King Score",DemonKingScore },
            {"Players Consumed",PlayerConsumed },
            {"Minions Consumed",MinionsConsumed },
            {"Player Deaths",PlayerDeaths },
            {"Time Spent As Lion King",TimeSpentAsLionKing },
            {"Time Spent As Ray King",TimeSpentAsRayKing },
            {"Time Spent As Dragon King",TimeSpentAsDragonKing },
        };
        AnalyticsResult LeaderboardAnalytic = Analytics.CustomEvent(("End Game Stats"), EndGameStatsData);
        Debug.Log(LeaderboardAnalytic);
    }

    public void SendEvolutionAnalytics()
    {
        if (!ShouldPostAnalytics)
            return;

        Dictionary<string, object> EvolutionAnalyticData = new Dictionary<string, object>
        {
            {"Slime Damage Output",SlimeDamageOutput },
            {"Lion Damage Output",LionDamageOutput },
            {"Ray Damage Output",RayDamageOutput },
            {"Dragon Damage Output",DragonDamageOutput },
            {"Time Spent As Slime",TimeSpentAsSlime },
            {"Time Spent As Lion",TimeSpentAsLion },
            {"Time Spent As Ray",TimeSpentAsRay },
            {"Time Spent As Dragon",TimeSpentAsDragon },
        };
        AnalyticsResult EvolutionAnalytic = Analytics.CustomEvent(("Evolution Stats"), EvolutionAnalyticData);
        Debug.Log(EvolutionAnalytic);
    }

    void Awake()
    {
        instance = this;
    }
}
