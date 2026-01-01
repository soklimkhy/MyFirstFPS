using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))] // Ensures the agent is always there
public class EnemyAI : MonoBehaviour
{
    // Professional Naming: Use PascalCase for public fields
    public NavMeshAgent Agent { get; private set; }

    [Header("AI Settings")]
    public float DetectionRange = 10f;
    public float AttackRange = 2f;
    public float DamageAmount = 20f;
    public float AttackSpeed = 1.5f;

    [Header("Patrol Settings")]
    public Transform[] PatrolPoints;

    private Transform m_Player;
    private float m_NextAttackTime = 0f;
    private int m_CurrentPatrolIndex = 0;

    // CS Concept: Enum for State Machine
    public enum AIState { Patrolling, Chasing, Attacking }
    public AIState CurrentState { get; private set; } = AIState.Patrolling;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // Finding objects by tag is okay for MVP, but in Enterprise we often 
        // use a "GameManager" or "PlayerManager" singleton to get the reference.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) m_Player = playerObj.transform;

        InitializePatrol();
    }

    void Update()
    {
        if (m_Player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, m_Player.position);
        UpdateState(distanceToPlayer);
        ExecuteState();
    }

    private void UpdateState(float distance)
    {
        // Simple and clear state transition logic
        if (distance <= AttackRange)
            CurrentState = AIState.Attacking;
        else if (distance <= DetectionRange)
            CurrentState = AIState.Chasing;
        else
            CurrentState = AIState.Patrolling;
    }

    private void ExecuteState()
    {
        switch (CurrentState)
        {
            case AIState.Patrolling:
                PerformPatrol();
                break;
            case AIState.Chasing:
                Agent.SetDestination(m_Player.position);
                break;
            case AIState.Attacking:
                PerformAttack();
                break;
        }
    }

    private void PerformPatrol()
    {
        if (PatrolPoints == null || PatrolPoints.Length == 0) return;

        if (!Agent.pathPending && Agent.remainingDistance < 0.5f)
        {
            m_CurrentPatrolIndex = (m_CurrentPatrolIndex + 1) % PatrolPoints.Length;
            Agent.SetDestination(PatrolPoints[m_CurrentPatrolIndex].position);
        }
    }

    private void PerformAttack()
    {
        // Stop moving while attacking for "AK Online" style recoil/aim
        Agent.SetDestination(transform.position);

        if (Time.time >= m_NextAttackTime)
        {
            PlayerHealth pHealth = m_Player.GetComponent<PlayerHealth>();
            if (pHealth != null)
            {
                pHealth.TakeDamage(DamageAmount);
                m_NextAttackTime = Time.time + AttackSpeed;
            }
        }
    }

    private void InitializePatrol()
    {
        if (PatrolPoints != null && PatrolPoints.Length > 0)
            Agent.SetDestination(PatrolPoints[0].position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}