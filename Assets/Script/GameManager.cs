using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEditor;
using TMPro;

public class GameManager : PersistentSingleton<GameManager>
{

    public event Action<int> OnScoreChanged;
    public event Action<float> OnTimerUpdated;
    public event Action OnGameStart;
    public event Action<float> OnGameEnd;
    public event Action OnPlayerDead;


    [SerializeField] string gameScene;
    private float timer = 0f;

    private bool isGameActive = false;

    protected override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += OnSceneLoaded;

    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == gameScene)
        {
            isGameActive = true;


            OnGameStart?.Invoke();

            
        }

    }

    void Update()
    {
        if (isGameActive)
        {
            timer += Time.deltaTime;
            OnTimerUpdated?.Invoke(timer);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(0);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/Game");
        timer = 0f;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Scenes/Start");
    }

    public void EndGame()
    {
        isGameActive = false;
        OnGameEnd?.Invoke(timer);
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        timer = 0f;
    }
    
    
}

   