using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float damage = 20f;
    public float attackSpeed = 1f;
    private float nextAttackTime = 0f;

    public NavMeshAgent agent;
    public Transform player;

    [Header("AI Settings")]
    public float detectionRange = 10f;
    public Transform[] patrolPoints; // Array of points to walk between
    private int currentPatrolIndex = 0;

    // The State Machine
    public enum AIState { Patrolling, Chasing }
    public AIState currentState = AIState.Patrolling;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Switch States based on distance
        if (distanceToPlayer < detectionRange)
        {
            currentState = AIState.Chasing;
        }
        else
        {
            currentState = AIState.Patrolling;
        }

        // Execute logic based on the current state
        switch (currentState)
        {
            case AIState.Patrolling:
                PatrolLogic();
                break;
            case AIState.Chasing:
                ChaseLogic();
                break;
        }
    }

    void PatrolLogic()
    {
        if (patrolPoints.Length == 0) return;

        // Check if bot reached the current patrol point
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Move to the next point in the array
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void ChaseLogic()
    {
        agent.SetDestination(player.position);

        // Calculate distance to player
        float distance = Vector3.Distance(transform.position, player.position);

        // If close enough and cooldown is over
        if (distance <= agent.stoppingDistance + 0.5f && Time.time >= nextAttackTime)
        {
            PlayerHealth pHealth = player.GetComponent<PlayerHealth>();
            if (pHealth != null)
            {
                pHealth.TakeDamage(damage);
                nextAttackTime = Time.time + attackSpeed;
                Debug.Log("Bot attacked you!");
            }
        }
    }

    // Visual help in the Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}