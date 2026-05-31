using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [SerializeField] private ParticlesLibrary library;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public bool HasParticle(string key)
    {
        return library.TryGet(key, out _);
    }

    public void SpawnParticle(string key, Vector3 pos)
    {
        if(!library.TryGet(key, out ParticleSystem prefab))
        {
            Debug.LogError($"Partícula com a chave \''{key}'\' não encontrada na biblioteca!");
            return;
        }

        ParticleSystem particle = Instantiate(prefab, pos, Quaternion.identity);

        Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
    }
}
