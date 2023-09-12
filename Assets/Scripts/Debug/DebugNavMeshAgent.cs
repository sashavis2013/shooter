using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebugNavMeshAgent : MonoBehaviour
{
    public bool velocity;
    public bool desiredVelocity;
    public bool path;

    NavMeshAgent _agent;
    
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    void OnDrawGizmos()
    {
        if (!_agent) {
            _agent = GetComponent<NavMeshAgent>();
        }

        if (velocity) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + _agent.velocity);
        }

        if (desiredVelocity) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _agent.desiredVelocity);
        }

        if (path) {
            Gizmos.color = Color.black;
            var agentPath = _agent.path;
            Vector3 prevCorner = transform.position;
            foreach(var corner in agentPath.corners) {
                Gizmos.DrawLine(prevCorner, corner);
                Gizmos.DrawSphere(corner, 0.1f);
                prevCorner = corner;
            }
        }
    }
}
