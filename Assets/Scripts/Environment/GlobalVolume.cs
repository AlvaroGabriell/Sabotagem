using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class GlobalVolume : MonoBehaviour
{
    public static GlobalVolume Instance { get; private set; }
    public static Volume VolumeGlobal { get; private set; }

    private DepthOfField depthOfField;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            VolumeGlobal = GetComponent<Volume>();

            VolumeGlobal.profile.TryGet(out depthOfField);
        }
        else
        {
            Destroy(gameObject); // Garante que apenas uma instância exista
        }
    }

    public void EnableDOF()
    {
        if (depthOfField != null)
            depthOfField.active = true;
    }

    public void DisableDOF()
    {
        if (depthOfField != null)
            depthOfField.active = false;
    }
}
