using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RuntimeSets/PlayerControllers")]
public class PlayerControllerRuntimeSet : RuntimeSet<PlayerController>
{

    public PlayerController GetPlayer(int playerId)
    {
        for (int i = 0; i < Length(); i++)
        {
            if (GetItemIndex(i) != null && GetItemIndex(i).id == playerId)
                return GetItemIndex(i);
        }
        return null;
    }


    public PlayerController GetPlayer(GameObject playerObject)
    {
        for (int i = 0; i < Length(); i++)
        {
            if (GetItemIndex(i) != null && GetItemIndex(i).gameObject == playerObject)
                return GetItemIndex(i);
        }
        return null;
    }
}
