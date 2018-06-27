using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Frog : MonoBehaviour {
    public float radius;
    public float timer;

    private Transform target;
    private NavMeshAgent agent;
    private float currentTimer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        currentTimer = timer;
    }

    private void Update()
    {
        currentTimer += Time.deltaTime;

        if(currentTimer >= timer)
        {
            Vector3 newPosition = RandomNavSphere(transform.position, radius, -1);
            agent.SetDestination(newPosition);
            currentTimer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layerMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layerMask);

        return navHit.position;
    }
}
