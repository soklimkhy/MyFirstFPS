using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    private Transform player;

    [Header("AI Settings")]
    public float detectionRange = 10f;    // Range to start chasing
    public float attackRange = 2f;       // Range to deal damage
    public float damage = 20f;           // How much health to take
    public float attackSpeed = 1.5f;     // Seconds between attacks
    private float nextAttackTime = 0f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;     // Array of points to walk between
    private int currentPatrolIndex = 0;

    // CS Concept: Enum for State Machine
    public enum AIState { Patrolling, Chasing }
    public AIState currentState = AIState.Patrolling;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find the player by Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Start patrolling if we have points
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // State Switching Logic
        if (distanceToPlayer < detectionRange)
        {
            currentState = AIState.Chasing;
        }
        else
        {
            currentState = AIState.Patrolling;
        }

        // Execute logic based on State
        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Chasing:
                ChaseAndAttack(distanceToPlayer);
                break;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // Move to next point if close enough to current one
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void ChaseAndAttack(float distance)
    {
        agent.SetDestination(player.position);

        // Attack Logic
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            PlayerHealth pHealth = player.GetComponent<PlayerHealth>();
            if (pHealth != null)
            {
                pHealth.TakeDamage(damage);
                nextAttackTime = Time.time + attackSpeed;
                Debug.Log("Bot Hit the Player!");
            }
        }
    }

    // This helps you see the detection range in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}