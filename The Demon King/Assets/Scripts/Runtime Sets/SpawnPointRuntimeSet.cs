using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RuntimeSets/SpawnPoints")]
public class SpawnPointRuntimeSet : RuntimeSet<GameObject>
{
    public int CurrentSpawnIndex = 0;
}
