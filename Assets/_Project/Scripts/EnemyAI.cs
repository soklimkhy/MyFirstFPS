using UnityEngine;
using UnityEngine.AI; // Required for NavMesh

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find the object tagged "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Tell the bot to move to the player's position every frame
            agent.SetDestination(playerTransform.position);
        }
    }
}