using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu]
public class LeaderboardDataList : ScriptableObject
{
    public List<LeaderboardData> Data = new List<LeaderboardData>();

    public int Length()
    {
        return Data.Count;
    }
}
