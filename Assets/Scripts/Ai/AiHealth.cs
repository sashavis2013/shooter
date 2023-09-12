using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class with lifecycle AI methods
/// </summary>
public class AiHealth : Health
{
    AiAgent _agent;

    protected override void OnStart() {
        _agent = GetComponent<AiAgent>();
    }

    protected override void OnDeath(Vector3 direction) {
        AiDeathState deathState = _agent.stateMachine.GetState(AiStateId.Death) as AiDeathState;
        if (deathState != null) deathState.Direction = direction;
        _agent.stateMachine.ChangeState(AiStateId.Death);
    }

    protected override void OnDamage(Vector3 direction) {

    }
}
