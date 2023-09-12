using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class with target finding related AI methods
/// </summary>
public class AiFindTargetState : AiState
{
    public AiStateId GetId() {
        return AiStateId.FindTarget;
    }

    public void Enter(AiAgent agent) {
        agent.navMeshAgent.speed = agent.config.findTargetSpeed;
    }

    public void Update(AiAgent agent) {
        // Wander
        if (!agent.navMeshAgent.hasPath) {
            WorldBounds worldBounds = Object.FindObjectOfType<WorldBounds>();
            agent.navMeshAgent.destination = worldBounds.RandomPosition();
        }

        if (agent.targeting.HasTarget) {
            agent.stateMachine.ChangeState(AiStateId.AttackTarget);
        }
    }

    public void Exit(AiAgent agent) {
    }
}
