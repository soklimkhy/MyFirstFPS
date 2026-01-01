using UnityEngine;
using System; // Required for Action

public class EnemyTarget : MonoBehaviour
{
    public float MaxHealth = 50f;
    private float m_CurrentHealth;

    // Professional Event: Allows other scripts to "listen" for death
    public static event Action OnEnemyDeath;

    void Start()
    {
        m_CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;

        if (m_CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        OnEnemyDeath?.Invoke(); // Notify whoever is listening
        Destroy(gameObject);
    }
}