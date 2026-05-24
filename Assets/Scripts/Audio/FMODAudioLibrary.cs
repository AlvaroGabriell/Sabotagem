using UnityEngine;
using FMODUnity;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/FMOD Audio Library")]
public class FMODAudioLibrary : ScriptableObject
{
    public List<SoundEntry> sounds;
    private Dictionary<string, EventReference> _cachedEntries;

    public bool TryGet(string key, out EventReference reference)
    {
        if(_cachedEntries == null)
        {
            _cachedEntries = new Dictionary<string, EventReference>();
            foreach(var entry in sounds) _cachedEntries[entry.key] = entry.reference;
        }

        return _cachedEntries.TryGetValue(key, out reference);
    }
}

[Serializable]
public struct SoundEntry
{
    public string key;
    public EventReference reference;
}