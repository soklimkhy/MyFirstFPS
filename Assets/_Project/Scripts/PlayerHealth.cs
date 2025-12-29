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
        // Update Text (This works for you)
        if (healthText != null)
            healthText.text = currentHealth.ToString();

        // Update Bar (This is the part to fix)
        if (healthBarFill != null)
        {
            // FillAmount needs a decimal between 0.0 and 1.0
            // currentHealth (80) / maxHealth (100) = 0.8f
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        isDead = true;
        if (deathScreen != null) deathScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player movement script
        if (GetComponent<PlayerMovement>() != null)
            GetComponent<PlayerMovement>().enabled = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}