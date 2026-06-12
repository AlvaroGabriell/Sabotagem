using UnityEngine;

/// <summary>
/// Guarda o personagem selecionado do player.
/// Esse script deve ser instanciado no PlayerControoler.
/// Pode ser usado para trocar o personagem em runtime por outros scripts (ex: tela de seleção).
/// </summary>
public class CharacterSelector
{
    public readonly PlayerController owner;
    public CharacterEntry SelectedCharacter {get; private set;}

    private readonly Transform modelParent;

    public CharacterEntry[] characters;

    public CharacterSelector(PlayerController owner, CharacterEntry[] characters)
    {
        this.owner = owner;
        this.characters = characters;
        modelParent = owner.transform.Find("Model");
        SelectedCharacter = GetCharacterEntry(CharacterType.Capybara);
    }

    public void SetCharacter(CharacterType type)
    {
        CharacterEntry entry = GetCharacterEntry(type);

        if(entry.prefab == null) return;

        SelectedCharacter = entry;

        if (modelParent.childCount > 0){
            Object.Destroy(modelParent.GetChild(0).gameObject);
            ParticleManager.Instance.SpawnParticle("clouds", Utils.GetVisualCenter(owner.gameObject) + new Vector3(0, 0, -0.72f));
        }

        Object.Instantiate(entry.prefab, modelParent);
    }

    private CharacterEntry GetCharacterEntry(CharacterType type)
    {
        foreach (var entry in characters)
        {
            if (entry.type == type) return entry;
        }

        return CharacterEntry.Default;
    }
}

[System.Serializable]
public struct CharacterEntry
{
    public CharacterType type;
    public GameObject prefab;
    public string voiceSound;

    public static CharacterEntry Default => new()
    {
        type = default,
        prefab = default,
        voiceSound = ""
    };
}

public enum CharacterType
{
    Capybara,
    Rabbit,
    Raccoon,
    Chameleon,
}