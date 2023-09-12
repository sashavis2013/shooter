using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class with state machine AI methods
/// </summary>
public class AiStateMachine
{
    public AiState[] States;
    public AiAgent Agent;
    public AiStateId CurrentState;

    public AiStateMachine(AiAgent agent) {
        this.Agent = agent;
        int numStates = System.Enum.GetNames(typeof(AiStateId)).Length;
        States = new AiState[numStates];
    }

    public void RegisterState(AiState state) {
        int index = (int)state.GetId();
        States[index] = state;
    }

    public AiState GetState(AiStateId stateId) {
        int index = (int)stateId;
        return States[index];
    }

    public void Update() {
        GetState(CurrentState)?.Update(Agent);
    }

    public void ChangeState(AiStateId newState) {
        var state = GetState(newState);
        if (state == null) {
            Debug.LogError($"{newState} State has not been registered");
        }

        if (newState != CurrentState) {
            GetState(CurrentState)?.Exit(Agent);
            CurrentState = newState;
            GetState(CurrentState)?.Enter(Agent);
        }
    }
}
