using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    // Bibliotecas separadas pra facilitar a população dos sons pelo inspetor
    [SerializeField] private FMODAudioLibrary sfxLib, musicLib, ambienceLib;

    // Biblioteca centralizada pra facilitar a programação... Sim, é uma gambiarra, eu sei
    private FMODAudioLibrary[] allLibs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            allLibs = new[] { sfxLib, musicLib, ambienceLib };
            DontDestroyOnLoad(gameObject);

            PlayOneShot("fans", Vector3.zero);
            PlayOneShot("beep", Vector3.zero);
            PlayOneShot("steelCreak", Vector3.zero);
        }
        else
        {
            Debug.LogError("Já tem um AudioManager na cena!");
            Destroy(gameObject);
        }
    }

    public bool HasSound(string key)
    {
        foreach (var lib in allLibs) if (lib.TryGet(key, out _)) return true;

        return false;
    }

    private bool TryGetFromAny(string key, out EventReference reference)
    {
        foreach (var lib in allLibs) if (lib.TryGet(key, out reference)) return true;

        Debug.LogError($"Som com a chave \''{key}'\' não encontrado em nenhuma biblioteca!");
        reference = default;
        return false;
    }

    public void PlayOneShot(string key, Vector3 pos)
    {
        if(TryGetFromAny(key, out EventReference reference)) RuntimeManager.PlayOneShot(reference, pos);
    }
    public void PlayOneShot(EventReference reference, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(reference, pos); 
    }

    public bool PlayInstancedSound(string key, Vector3 pos, out EventInstance instance)
    {
        if(TryGetFromAny(key, out EventReference reference))
        {
            instance = RuntimeManager.CreateInstance(reference);
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
            instance.start();
            return true;
        }

        instance = default;
        return false;
    }
}