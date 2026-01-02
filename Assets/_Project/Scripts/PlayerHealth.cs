using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI References")]
    public Image healthBarFill;
    public Text healthText;
    // Note: We don't need 'deathScreen' here anymore because GameManager handles it,
    // but I left it in case you want a specific red flash effect.
    public GameObject deathScreen;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        UpdateUI();
        if (currentHealth <= 0) Die();
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = currentHealth.ToString();

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        isDead = true;

        // --- UPDATED: Tell GameManager we died ---
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        // ----------------------------------------

        // If you kept the specific death screen logic locally:
        if (deathScreen != null) deathScreen.SetActive(true);

        // Disable player movement
        // (Make sure the script name 'PlayerMovement' matches your actual script name)
        MonoBehaviour movementScript = GetComponent<MonoBehaviour>();
        // Or specifically: GetComponent<PlayerMovement>().enabled = false;
    }
}