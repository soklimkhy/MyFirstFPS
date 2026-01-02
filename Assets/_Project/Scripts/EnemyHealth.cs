using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Image healthBarFill;
    public GameObject healthBarCanvas;

    [Header("Respawn")]
    public float respawnDelay = 3f;
    private Vector3 initialPosition;
    private NavMeshAgent agent;
    private Collider enemyCollider;
    private Renderer[] enemyRenderers;

    void Start()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponent<Collider>();
        enemyRenderers = GetComponentsInChildren<Renderer>();

        UpdateHealthUI();
    }

    void Update()
    {
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.LookAt(Camera.main.transform);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        // --- UPDATED: Add Score ---
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKill();
        }
        // --------------------------

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        enemyCollider.enabled = false;
        ToggleVisuals(false);
        if (healthBarCanvas != null) healthBarCanvas.SetActive(false);
        if (agent != null) agent.isStopped = true;

        yield return new WaitForSeconds(respawnDelay);

        transform.position = initialPosition;
        currentHealth = maxHealth;
        UpdateHealthUI();

        enemyCollider.enabled = true;
        ToggleVisuals(true);
        if (healthBarCanvas != null) healthBarCanvas.SetActive(true);

        if (agent != null)
        {
            agent.Warp(initialPosition);
            agent.isStopped = false;
        }
    }

    void ToggleVisuals(bool state)
    {
        foreach (Renderer r in enemyRenderers)
        {
            r.enabled = state;
        }
    }
}