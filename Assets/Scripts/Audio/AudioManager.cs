using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    // -- FMOD VCAs ---------------------------------------------
    [SerializeField] private string musicVCAPath = "vca:/Music_VCA", sfxVCAPath = "vca:/SFX_VCA", masterVCAPath = "vca:/Master_VCA";
    private VCA masterVCA, musicVCA, sfxVCA;

    // -- FMOD Buses --------------------------------------------
    [SerializeField] private string musicBusPath = "bus:/MUSIC", sfxBusPath = "bus:/SFX";
    private Bus musicBus, sfxBus;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            masterVCA = RuntimeManager.GetVCA(masterVCAPath);
            musicVCA = RuntimeManager.GetVCA(musicVCAPath);
            sfxVCA = RuntimeManager.GetVCA(sfxVCAPath);

            musicBus = RuntimeManager.GetBus(musicBusPath);
            sfxBus = RuntimeManager.GetBus(sfxBusPath);

        }
        else
        {
            Debug.LogError("Já tem um AudioManager na cena!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    // -- Volume ---------------------------------------------

    public void SetMasterVolume(float volume)
    {
        masterVCA.setVolume(volume);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        musicVCA.setVolume(volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        sfxVCA.setVolume(volume);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    // -- Bus Control -----------------------------------------

    public void StopAllSFX(bool immediate = false)
    {
        sfxBus.stopAllEvents(immediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
    }

    public void StopAllMusic(bool immediate = false)
    {
        musicBus.stopAllEvents(immediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
    }

    public void StopAllAudio(bool immediate = false)
    {
        StopAllSFX(immediate);
        StopAllMusic(immediate);
    }

    // -- Sound Player ---------------------------------------------

    public void PlayOneShot(string eventPath, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(eventPath, pos);
    }
    public void PlayOneShot(EventReference reference, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(reference, pos); 
    }

    public EventInstance PlayInstancedSound(string eventPath, Vector3 pos)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventPath);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
        instance.start();
        return instance;
    }
    public EventInstance PlayInstancedSound(EventReference reference, Vector3 pos)
    {
        EventInstance instance = RuntimeManager.CreateInstance(reference);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
        instance.start();
        return instance;
    }
}