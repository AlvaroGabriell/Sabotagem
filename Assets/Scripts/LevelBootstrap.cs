using System.Collections.Generic;
using FMOD.Studio;
using Unity.Cinemachine;
using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    public Transform playerSpawnPoint;

    public CinemachineCamera cinemachineCamera;
    public Camera mainCamera;

    readonly List<EventInstance> instancedAmbSounds = new();

    void Awake()
    {
        UIController.Instance.UICamera.SetActive(false);
        // TODO: Create player 1 and 2
        // Set players as tracking target for cinemachine camera
        UIController.Instance.CloseAllMenus();

    }

    void Start()
    {
        instancedAmbSounds.Add(AudioManager.Instance.PlayInstancedSound(AudioEvents.Amb.Beep, Vector3.zero));
        instancedAmbSounds.Add(AudioManager.Instance.PlayInstancedSound(AudioEvents.Amb.Fans, Vector3.zero));
        instancedAmbSounds.Add(AudioManager.Instance.PlayInstancedSound(AudioEvents.Amb.Steelcreak, Vector3.zero));
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
}
