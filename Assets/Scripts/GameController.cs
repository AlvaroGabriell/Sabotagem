using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool IsPaused { get; private set; } = false;
    public bool GameStarted { get; private set; } = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            EventBusAutoSubscriber.ScanAndSubscribe(); // Escaneia e inscreve os eventos automaticamente
        }
        else Destroy(gameObject); // Garante que apenas uma instância exista
    }

    void OnDestroy()
    {
        EventBusAutoSubscriber.UnsubscribeAll(); // Desinscreve todos os eventos para evitar vazamentos de memória
    }

    // --

    void Start()
    {
        BootGame();
    }

    public void BootGame()
    {
        
    }

    public void StartGame()
    {
        
    }

    public void RestartGame(bool toMainMenu)
    {
        
    }

    // -- 

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        //UIController.Instance.HUDScreen.SetActive(false);
        //OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        //UIController.Instance.HUDScreen.SetActive(true);
        //OnGameResumed?.Invoke();
    }
}
