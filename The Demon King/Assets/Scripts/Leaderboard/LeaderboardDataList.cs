using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu]
public class LeaderboardDataList : ScriptableObject
{
    public List<LeaderboardData> Data;

    public int Length()
    {
        return Data.Count;
    }

    public LeaderboardData GetItemIndex(int index)
    {
        return Data[index];
    }
}
