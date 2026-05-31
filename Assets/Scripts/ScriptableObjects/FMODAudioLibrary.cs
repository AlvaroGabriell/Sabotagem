using UnityEngine;
using FMODUnity;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/FMOD Audio Library")]
public class FMODAudioLibrary : ScriptableObject
{
    public List<SoundEntry> sounds;
    
    private Dictionary<string, EventReference> cache;

    public bool TryGet(string key, out EventReference reference)
    {
        if(cache == null)
        {
            cache = new();

            foreach(var entry in sounds) cache[entry.key] = entry.reference;
        }

        return cache.TryGetValue(key, out reference);
    }
}

[Serializable]
public struct SoundEntry
{
    public string key;
    public EventReference reference;
}