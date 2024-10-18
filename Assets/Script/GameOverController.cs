using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Button restartButton;
    public Button mainMenuButton;
    public TMP_Text gameOverText;
    public GameObject gameOverPanel;

    void Start()
    {
        restartButton.onClick.AddListener(GameManager.Instance.StartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        GameManager.Instance.OnGameEnd += ShowGameOverPanel;
        gameOverPanel.SetActive(false);
    }

    void ShowGameOverPanel(float timer)
    {
        
        // Timer to 0:00 format
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        
        gameOverText.text = "Survival Time: " + $"{minutes:00}:{seconds:00}";
    }

    void ReturnToMainMenu()
    {
        gameOverPanel.SetActive(false);
        GameManager.Instance.ReturnToMainMenu();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnd -= ShowGameOverPanel;
        }
    }
}