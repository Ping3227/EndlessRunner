using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button startButton;
    public GameObject mainMenuPanel;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        ShowMainMenu();
    }

    void StartGame()
    {
        mainMenuPanel.SetActive(false);
        GameManager.Instance.StartGame();
    }

    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
    }

    
}