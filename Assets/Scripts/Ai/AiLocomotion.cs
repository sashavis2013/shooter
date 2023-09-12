using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Class with movement-related AI methods
/// </summary>
public class AiLocomotion : MonoBehaviour
{
    NavMeshAgent _agent;
    Animator _animator;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (_agent.hasPath) {
            _animator.SetFloat("speed", _agent.velocity.magnitude);
        } else {
            _animator.SetFloat("speed", 0);
        }
    }
}
