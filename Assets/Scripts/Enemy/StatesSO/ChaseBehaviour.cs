using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseBehaviour : MonoBehaviour
{
    public float Speed;
    private NavMeshAgent agent;
    private Rigidbody _rb;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        agent.speed = Speed;
    }
    public void Chase(Transform target)
    {
        agent.SetDestination(target.position);
    }
    public void Run(Transform target, Transform self)
    {
        _rb.velocity = (target.position - self.position).normalized * -Speed;
    }

    public void StopChasing()
    {
        agent.ResetPath();
        _rb.velocity = Vector3.zero;
    }
}
