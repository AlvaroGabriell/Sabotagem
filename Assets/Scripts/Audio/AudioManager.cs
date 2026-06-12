using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    // -- FMOD VCAs ---------------------------------------------
    [SerializeField] private string musicVCAPath = "vca:/Music_VCA", sfxVCAPath = "vca:/SFX_VCA", masterVCAPath = "vca:/Master_VCA";
    private VCA masterVCA, musicVCA, sfxVCA;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            masterVCA = RuntimeManager.GetVCA(masterVCAPath);
            musicVCA = RuntimeManager.GetVCA(musicVCAPath);
            sfxVCA = RuntimeManager.GetVCA(sfxVCAPath);

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