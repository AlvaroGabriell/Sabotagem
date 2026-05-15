using UnityEngine;
using FMODUnity; 

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}
    public EventReference fanSound, beepSound, creakSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayOneShot(fanSound, Vector3.zero);
            PlayOneShot(beepSound, Vector3.zero);
            PlayOneShot(creakSound, Vector3.zero);

        }
        else
        {
            Debug.LogError("Já tem um AudioManager na cena!");
            Destroy(gameObject);
        }
    }

    public void PlayOneShot(EventReference reference, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(reference, pos);
    }
    public void PlayOneShot(string path, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(path, pos);
    }

}
