using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StateManager : MonoBehaviourPun
{
    public State currentState;
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            RunStateMachine();
    }

    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState();

        if (nextState != null)
        {
            //switch to next state
            SwitchToTheNextState(nextState);
        }
    }

    private void SwitchToTheNextState(State nextState)
    {
        currentState = nextState;
    }
}
