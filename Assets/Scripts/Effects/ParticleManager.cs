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

    public void SpawnParticle(string key, Vector3 pos, float radius = 1f)
    {
        if(!library.TryGet(key, out ParticleSystem prefab))
        {
            Debug.LogError($"Partícula com a chave \''{key}'\' não encontrada na biblioteca!");
            return;
        }

        ParticleSystem particle = Instantiate(prefab, pos, Quaternion.identity);

        float scale = radius / 0.7f;
        var shape = particle.shape;
        shape.scale = new Vector3(0.3f * scale, 0.8f, 0.3f * scale) ;

        Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
    }
}
