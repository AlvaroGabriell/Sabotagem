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

    public PlayerController player1, player2;

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
    }

    void Start()
    {
        instancedAmbSounds.Add(AudioManager.Instance.PlayInstancedSound(AudioEvents.Amb.Beep, Vector3.zero));
        instancedAmbSounds.Add(AudioManager.Instance.PlayInstancedSound(AudioEvents.Amb.Fans, Vector3.zero));
        instancedAmbSounds.Add(AudioManager.Instance.PlayInstancedSound(AudioEvents.Amb.Steelcreak, Vector3.zero));

        player1.PlayerInput.actions.FindActionMap("Player").Enable();
        player2.PlayerInput.actions.FindActionMap("Player").Enable();

        GameController.Instance.StartGame();
    }

    void Update()
    {
        
    }
    
    void OnDestroy()
    {
        UIController.Instance.UICamera.SetActive(true);
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
}
