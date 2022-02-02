using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform destination;

    void Start()
    {
        StartCoroutine(MoveToFinish());
    }

    private IEnumerator MoveToFinish()
    {
        yield return new WaitForSeconds(2);
        agent.SetDestination(destination.position);
    }
}
