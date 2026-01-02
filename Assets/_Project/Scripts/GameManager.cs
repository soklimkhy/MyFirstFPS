using UnityEngine;
using TMPro; // Needed for the UI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton pattern for easy access

    [Header("UI Settings")]
    public TMP_Text scoreText;
    private int killCount = 0;

    void Awake()
    {
        // ensuring there is only one Game Manager
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddKill()
    {
        killCount++;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Kills: " + killCount;
    }
}