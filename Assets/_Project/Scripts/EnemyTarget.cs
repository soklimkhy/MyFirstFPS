using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Enemy hit! Health remaining: " + health);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // For now, just destroy the bot. Later you can add an explosion!
        Destroy(gameObject);
        Debug.Log("Enemy Destroyed!");
    }
}