using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelBootstrap : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;
    public Camera mainCamera;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform player1SpawnPos;
    [SerializeField] private Transform player2SpawnPos;

    [NonSerialized] public PlayerController player1, player2;

    public float LevelTime { get; private set; } = 0f;
    private bool levelCompleted = false;

    private int lastDisplayedSecond = -1;

    readonly List<EventInstance> instancedAmbSounds = new();

    void Awake()
    {
        UIController.Instance.UICamera.SetActive(false);
        (var p1, var p2) = SpawnPlayers();
        player1 = p1.GetComponent<PlayerController>();
        player2 = p2.GetComponent<PlayerController>();
        targetGroup.AddMember(p1.transform, 1f, 1f);
        targetGroup.AddMember(p2.transform, 1f, 1f);
        UIController.Instance.CloseAllMenus();
        UIController.Instance.LoadingScreen.StopLoadingScreen();
    }

    void Start()
    {
        instancedAmbSounds.Add(AudioManager.Instance.PlayInstancedSound(AudioEvents.Amb.Beep, Vector3.zero));
        instancedAmbSounds.Add(AudioManager.Instance.PlayInstancedSound(AudioEvents.Amb.Fans, Vector3.zero));
        instancedAmbSounds.Add(AudioManager.Instance.PlayInstancedSound(AudioEvents.Amb.Steelcreak, Vector3.zero));

        player1.PlayerInput.actions.FindActionMap("Player").Enable();
        player2.PlayerInput.actions.FindActionMap("Player").Enable();

        UIController.Instance.HUD.ShowTimer();

        GameController.Instance.StartGame();
    }

    void Update()
    {
        if(GameController.Instance.GameStarted && !GameController.Instance.IsPaused && !levelCompleted)
        {
            LevelTime += Time.deltaTime;

            int currentSecond = Mathf.FloorToInt(LevelTime);

            if (currentSecond != lastDisplayedSecond)
            {
                lastDisplayedSecond = currentSecond;
                UIController.Instance.HUD.SetTimer(LevelTime);
            }
        }
    }
    
    void OnDestroy()
    {
        if(UIController.Instance.UICamera != null) UIController.Instance.UICamera.SetActive(true);
        UIController.Instance.HUD.HideTimer();
        instancedAmbSounds.ForEach(instance => instance.stop(STOP_MODE.IMMEDIATE));
        instancedAmbSounds.Clear();
    }

    public (GameObject, GameObject) SpawnPlayers()
    {
        GameObject p1 = Instantiate(playerPrefab, player1SpawnPos.position, Quaternion.identity);
        var player1Input = p1.GetComponent<PlayerInput>();
        player1Input.SwitchCurrentControlScheme(
            InputAssignmentManager.Instance.Player1.Slot.ToString(),
            InputAssignmentManager.Instance.Player1.Device
        );
        player1Input.actions.FindActionMap("Player").Disable();
        
        GameObject p2 = Instantiate(playerPrefab, player2SpawnPos.position, Quaternion.identity);
        var player2Input = p2.GetComponent<PlayerInput>();
        player2Input.SwitchCurrentControlScheme(
            InputAssignmentManager.Instance.Player2.Slot.ToString(),
            InputAssignmentManager.Instance.Player2.Device
        );
        player2Input.actions.FindActionMap("Player").Disable();

        return (p1, p2);
    }

    public void CompleteLevel()
    {
        StartCoroutine(CompleteLevel(0.6f));
    }

    private IEnumerator CompleteLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        player1.PlayerInput.actions.FindActionMap("Player").Disable();
        player2.PlayerInput.actions.FindActionMap("Player").Disable();

        LevelProgress.SaveBestTime(GameController.Instance.LoadedLevel.name, LevelTime);
        levelCompleted = true;

        instancedAmbSounds.ForEach(instance => instance.stop(STOP_MODE.IMMEDIATE));
        instancedAmbSounds.Clear();

        UIController.Instance.HUD.HideTimer();
        UIController.Instance.levelWonTimeText.text = LevelUI.FormatTime(LevelTime);

        if (GameController.Instance.IsLastLevel())
        {
            UIController.Instance.OpenMenu(UIController.Instance.gameWonMenu);
        } else
        {
            UIController.Instance.OpenMenu(UIController.Instance.levelWonMenu);
        }
    }
}
