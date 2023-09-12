using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Class with chase-related AI methods
/// </summary>
public class AiChasePlayerState : AiState
{
    
    float _timer = 0.0f;

    public AiStateId GetId() {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent) {

    }

    public void Update(AiAgent agent) {
        if (!agent.enabled) {
            return;
        }

        _timer -= Time.deltaTime;
        if (!agent.navMeshAgent.hasPath) {
            agent.navMeshAgent.destination = agent.playerTransform.position;
        }

        if (_timer < 0.0f) {
            Vector3 direction = (agent.playerTransform.position - agent.navMeshAgent.destination);
            direction.y = 0;
            if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance) {
                if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial) {
                    agent.navMeshAgent.destination = agent.playerTransform.position;
                }
            }
            _timer = agent.config.maxTime;
        }
    }

    public void Exit(AiAgent agent) {
    }
}
