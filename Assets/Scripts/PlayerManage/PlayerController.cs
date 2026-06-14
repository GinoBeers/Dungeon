using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void GoToDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    public void ResetToStart()
    {
        Vector3 startPos = new Vector3(1.5f, 0.5f, 1.5f);

        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        navMeshAgent.Warp(startPos);
    }
}
