using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool IsPaused { get; private set; } = false;
    public bool GameStarted { get; private set; } = false;

    // -- Scene Management ----------------------------------------
    public Scene LoadedLevel {get; set;} = default;
    public bool IsLoading { get; private set; } = false;

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
        StartCoroutine(BootGame());
    }

    IEnumerator BootGame()
    {
        yield return SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
        GameStarted = false;
        GlobalVolume.Instance.DisableDOF();
        //UIController.Instance.HUD.setActive(false);
        UIController.Instance.OpenMenu(UIController.Instance.mainMenu);
    }

    public void StartGame()
    {
        GameStarted = true;
        //UIController.Instance.HUD.setActive(true);
    }

    public void ReturnToMenu()
    {
        if(IsPaused) ResumeGame();
        GameStarted = false;
        StartCoroutine(ReturnToMenuCoroutine());
    }

    IEnumerator ReturnToMenuCoroutine()
    {
        if(IsLoading) yield break;
        IsLoading = true;
    
        if(IsPaused) ResumeGame();
        UIController.Instance.CloseAllMenus();
        AudioManager.Instance.StopAllAudio();

        if (LoadedLevel.IsValid() && LoadedLevel.isLoaded) yield return SceneManager.UnloadSceneAsync(LoadedLevel);
        LoadedLevel = default;

        GameStarted = false;
        GlobalVolume.Instance.DisableDOF();
        //UIController.Instance.HUD.setActive(false);
        UIController.Instance.OpenMenu(UIController.Instance.mainMenu);
        IsLoading = false;
    }

    public void ReloadCurrentLevel()
    {
        if(LoadedLevel.IsValid() && LoadedLevel.isLoaded) StartCoroutine(LoadLevelCoroutine(LoadedLevel.name));
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadLevelCoroutine(sceneName));
    }
    IEnumerator LoadLevelCoroutine(string sceneName)
    {
        if(IsLoading) yield break;
        IsLoading = true;
        UIController.Instance.CloseAllMenus();

        if(LoadedLevel.IsValid() && LoadedLevel.isLoaded) yield return SceneManager.UnloadSceneAsync(LoadedLevel);

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        LoadedLevel = SceneManager.GetSceneByName(sceneName);

        IsLoading = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        //UIController.Instance.HUD.setActiveScreenHUD(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        //UIController.Instance.HUD.setActiveScreenHUD(true);
    }

    public int GetCurrentLevelNumber()
    {
        string name = LoadedLevel.name;
        if (!int.TryParse(name.Split('_')[1], out int number)) return -1;
        return number;
    }

    public bool IsFirstLevel()
    {
        string name = LoadedLevel.name;
        if (!int.TryParse(name.Split('_')[1], out int number)) return false;

        string previousLevel = $"Level_{number - 1:D2}";
        // retorna -1 se a cena não existir
        return SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/Levels/{previousLevel}.unity") == -1;
    }

    public bool IsLastLevel()
    {
        string name = LoadedLevel.name;
        if (!int.TryParse(name.Split('_')[1], out int number)) return false;

        string nextLevel = $"Level_{number + 1:D2}";
        // retorna -1 se a cena não existir
        return SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/Levels/{nextLevel}.unity") == -1;
    }
}
