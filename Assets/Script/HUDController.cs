using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject gameplayPanel;

    private void Start()
    {
        
        GameManager.Instance.OnTimerUpdated += UpdateTimerDisplay;
        GameManager.Instance.OnGameStart += ShowGameplayPanel;
        GameManager.Instance.OnGameEnd += HideGameplayPanel;
        gameplayPanel.SetActive(true);
        
        
    }

    
    private void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
    
    private void ShowGameplayPanel()
    {
        gameplayPanel.SetActive(true);
    }

    private void HideGameplayPanel(float timer)
    {
        gameplayPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimerUpdated -= UpdateTimerDisplay;
            GameManager.Instance.OnGameStart -= ShowGameplayPanel;
            GameManager.Instance.OnGameEnd -= HideGameplayPanel;
        }
    }
}