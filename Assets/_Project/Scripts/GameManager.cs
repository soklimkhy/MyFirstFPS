using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed for standard UI
using TMPro; // Needed for TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int targetKills = 20; // Set this to 20 in Inspector
    private int currentKills = 0;

    [Header("UI References")]
    public TMP_Text killCountText; // Drag your "0/20" Text here
    public GameObject winPanel;    // Drag "You Win" Panel here
    public GameObject losePanel;   // Drag "Game Over" Panel here

    private bool gameEnded = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1; // Unpause game
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        UpdateUI();
    }

    public void AddKill()
    {
        if (gameEnded) return;

        currentKills++;
        UpdateUI();

        if (currentKills >= targetKills)
        {
            WinGame();
        }
    }

    public void PlayerDied()
    {
        if (gameEnded) return;
        LoseGame();
    }

    void UpdateUI()
    {
        if (killCountText != null)
        {
            killCountText.text = "Kills: " + currentKills + " / " + targetKills;
        }
    }

    void WinGame()
    {
        gameEnded = true;
        Debug.Log("YOU WIN!");
        if (winPanel != null) winPanel.SetActive(true);
        UnlockCursor();
        Time.timeScale = 0; // Pause game
    }

    void LoseGame()
    {
        gameEnded = true;
        Debug.Log("GAME OVER");
        if (losePanel != null) losePanel.SetActive(true);
        UnlockCursor();
        Time.timeScale = 0; // Pause game
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game...");
    }
}