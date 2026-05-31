using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticlesLibrary", menuName = "Particles/Library")]
public class ParticlesLibrary : ScriptableObject
{
    public List<ParticleEntry> particles;

    private Dictionary<string, ParticleSystem> cache;

    public bool TryGet(string key, out ParticleSystem particle)
    {
        if(cache == null)
        {
            cache = new();

            foreach(var entry in particles) cache[entry.key] = entry.prefab;
        }

        return cache.TryGetValue(key, out particle);
    }
}

[Serializable]
public struct ParticleEntry
{
    public string key;
    public ParticleSystem prefab;
}