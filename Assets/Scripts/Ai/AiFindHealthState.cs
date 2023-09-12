using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class with health finding AI methods
/// </summary>
public class AiFindHealthState : AiState {

    GameObject _pickup;
    GameObject[] _pickups = new GameObject[3];

    public AiStateId GetId() {
        return AiStateId.FindHealth;
    }

    public void Enter(AiAgent agent) {
        _pickup = null;
        agent.navMeshAgent.speed = agent.config.findWeaponSpeed;
        agent.navMeshAgent.ResetPath();
    }

    public void Update(AiAgent agent) {
        // Find pickup
        if (!_pickup) {
            _pickup = FindPickup(agent);

            if (_pickup) {
                CollectPickup(agent, _pickup);
                return;
            }
        }

        // Wander
        if (!agent.navMeshAgent.hasPath && !agent.navMeshAgent.pathPending) {
            WorldBounds worldBounds = Object.FindObjectOfType<WorldBounds>();
            agent.navMeshAgent.destination = worldBounds.RandomPosition();
        }

        if (!agent.health.IsLowHealth()) {
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
        }
    }

    public void Exit(AiAgent agent) {
    }

    GameObject FindPickup(AiAgent agent) {
        int count = agent.sensor.Filter(_pickups, "Pickup", "Health");
        if (count > 0) {
            float bestAngle = float.MaxValue;
            GameObject bestPickup = _pickups[0];
            for (int i = 0; i < count; ++i) {
                GameObject pickup = _pickups[i];
                var transform = agent.transform;
                float pickupAngle = Vector3.Angle(transform.forward, pickup.transform.position - transform.position);
                if (pickupAngle < bestAngle) {
                    bestAngle = pickupAngle;
                    bestPickup = pickup;
                }
            }
            return bestPickup;
        }
        return null;
    }

    void CollectPickup(AiAgent agent, GameObject pickup) {
        agent.navMeshAgent.destination = pickup.transform.position;
    }
}
