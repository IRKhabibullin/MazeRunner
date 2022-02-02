using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;

    void Start()
    {
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            agent.SetDestination(new Vector3(-9, 0.75f, 14));
        }
    }
}
