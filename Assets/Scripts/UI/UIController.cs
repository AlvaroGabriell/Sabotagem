using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public HUDController HUD;
    public LoadingScreenUI LoadingScreen;

    public GameObject UICamera;

    // -- Menus and Screens ----------------------------------------
    public GameObject background, mainMenu, levelSelectMenu, pauseMenu, configMenu, controlsMenu, soundMenu, levelWonMenu, gameWonMenu, gameLostMenu, creditsMenu;
    public GameObject inputAssignmentMenu;

    // -- Level Won Menu -------------------------------------------
    public TextMeshProUGUI levelWonTimeText;

    private readonly Stack<GameObject> menuStack = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Garante que apenas uma instância exista
        }
    }

    // Use this function whenever you need to open a menu
    // It ensures that only one menu is active at a time, closing the current menu before opening the new one
    // Maintaining a stack of menus to manage navigation between them.
    public void OpenMenu(GameObject menu)
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }

        menu.SetActive(true);
        GlobalVolume.Instance.EnableDOF();
        menuStack.Push(menu);

        bool noLevel = GameController.Instance.LoadedLevel == default || !GameController.Instance.LoadedLevel.IsValid();
        bool isOverlayMenu = menu == configMenu || menu == levelSelectMenu || menu == soundMenu || menu == creditsMenu || menu == inputAssignmentMenu;
        
        if (noLevel && isOverlayMenu) background.SetActive(true);
    }

    // Use this function to close the current menu and reactivate the previous one, if any.
    public void CloseCurrentMenu()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Pop().SetActive(false);
        }
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(true);
        }
        else {
            GlobalVolume.Instance.DisableDOF();
            background.SetActive(false);
        }
    }

    public void CloseAllMenus()
    {
        while(menuStack.Count > 0)
        {
            menuStack.Pop().SetActive(false);
        }
        if(menuStack.Count <= 0) {
            GlobalVolume.Instance.DisableDOF();
            background.SetActive(false);
        }
    }

    public void HandleEscape()
    {
        if (GameController.Instance.GameStarted && !GameController.Instance.IsPaused)
        {
            // Se estiver no jogo normal, pausa o jogo
            OnPause();
        }
        else if (pauseMenu.activeSelf)
        {
            // Se estiver no menu de pausa, resume o jogo
            OnResume();
        }
        else if (configMenu.activeSelf || soundMenu.activeSelf || creditsMenu.activeSelf || levelSelectMenu.activeSelf || controlsMenu.activeSelf)
        {
            OnBack();
        }
    }

    public void OnPlay()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);

        OpenMenu(levelSelectMenu);
        if (!InputAssignmentManager.Instance.AreAllPlayersAssigned()) OpenMenu(inputAssignmentMenu);
    }

    public void OnSettings()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        OpenMenu(configMenu);
    }

    public void OnCredits()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        OpenMenu(creditsMenu);
    }
    
    public void OnControls()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);

        OpenMenu(controlsMenu);
        if (!InputAssignmentManager.Instance.AreAllPlayersAssigned()) OpenMenu(inputAssignmentMenu);
    }

    public void OnSounds()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        OpenMenu(soundMenu);
    }

    public void OnPause()
    {
        OpenMenu(pauseMenu);
        GameController.Instance.PauseGame();
        if(Utils.TryGetPlayers(out GameObject[] players))
        {
            foreach (var player in players)
            {
                player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
            }
        }
    }
    public void OnResume()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        CloseCurrentMenu();
        GameController.Instance.ResumeGame();
        if(Utils.TryGetPlayers(out GameObject[] players))
        {
            foreach (var player in players)
            {
                player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
            }
        }
    }
    public void OnQuit()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        Application.Quit();
    }
    public void OnRestart()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        GameController.Instance.ReloadCurrentLevel();
    }
    public void OnMainMenu()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        GameController.Instance.ReturnToMenu();
    }
    public void OnBack()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        CloseCurrentMenu();
    }

    public void OnSelectLevel(string levelName)
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        LoadingScreen.StartLoadingScreen(LoadingScreenUI.GetLevelNameFromSceneName(levelName));
        GameController.Instance.LoadLevel(levelName);
    }

    public void OnNextLevel()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        if(GameController.Instance.LoadedLevel.IsValid())
        {
            string currentLevelName = GameController.Instance.LoadedLevel.name;
            if(currentLevelName.StartsWith("Level_"))
            {
                string[] parts = currentLevelName.Split('_');
                if(parts.Length == 2 && int.TryParse(parts[1], out int levelNumber))
                {
                    string nextLevelName = $"Level_{levelNumber + 1:D2}";
                    GameController.Instance.LoadLevel(nextLevelName);
                }
            }
        }
    }

    public void OnClearProgress()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
        LevelProgress.ClearProgress();
    }
}
