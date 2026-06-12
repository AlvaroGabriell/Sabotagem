using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    //public HUDController HUD;
    public GameObject UICamera;

    // -- References ----------------------------------------
    public GameObject background, mainMenu, pauseMenu, configMenu, soundMenu, levelWonMenu, gameWonMenu, gameLostMenu, creditsMenu;

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

        bool noLevel = GameController.Instance.LoadedLevel == null;
        bool isOverlayMenu = menu == configMenu || menu == soundMenu || menu == creditsMenu;
        
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
        if (mainMenu.activeSelf || levelWonMenu.activeSelf || gameWonMenu.activeSelf || gameLostMenu.activeSelf)
        {
            // Se estiver em um desses menus, não faz nada
            return;
        }
        else if (pauseMenu.activeSelf)
        {
            // Se estiver no menu de pausa, resume o jogo
            OnResume();
        }
        else if (configMenu.activeSelf || soundMenu.activeSelf || creditsMenu.activeSelf)
        {
            OnBack();
        }
        else
        {
            // Se estiver no jogo normal, pausa o jogo
            OnPause();
        }
    }

    public void OnPlay()
    {
        OnSelectLevel("SampleScene");
    }

    public void OnSettings()
    {
        OpenMenu(configMenu);
    }

    public void OnCredits()
    {
        OpenMenu(creditsMenu);
    }

    public void OnSounds()
    {
        OpenMenu(soundMenu);
    }

    public void OnPause()
    {
        OpenMenu(pauseMenu);
        GameController.Instance.PauseGame();
        //if(Utils.TryGetPlayer(out GameObject player)) player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
    }
    public void OnResume()
    {
        CloseCurrentMenu();
        GameController.Instance.ResumeGame();
        //if(Utils.TryGetPlayer(out GameObject player)) player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
    }
    public void OnQuit()
    {
        Application.Quit();
    }
    public void OnRestart()
    {
        GameController.Instance.ReloadCurrentLevel();
    }
    public void OnMainMenu()
    {
        GameController.Instance.ReturnToMenu();
    }
    public void OnBack()
    {
        CloseCurrentMenu();
    }

    public void OnSelectLevel(string levelName)
    {
        //SFXManager.Instance.PlaySFX("ui_click");
        GameController.Instance.LoadLevel(levelName);
    }

    public void OnNextLevel()
    {
        //SFXManager.Instance.PlaySFX("ui_click");
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
}
