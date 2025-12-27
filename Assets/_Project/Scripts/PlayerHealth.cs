using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI References")]
    public Slider healthSlider;
    public GameObject deathScreen; // Drag your DeathScreen panel here

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthSlider != null) healthSlider.value = currentHealth / maxHealth;
    }

    void Die()
    {
        isDead = true;
        
        // 1. Show the Death Screen
        if (deathScreen != null) deathScreen.SetActive(true);

        // 2. Unlock the mouse so we can click the button
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Optional: Disable player movement so they can't walk while dead
        GetComponent<PlayerMovement>().enabled = false;
        
        Debug.Log("Player is Dead");
    }

    // This function will be called by the Button
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}