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

    public CharacterEntry[] characters;

    public CharacterSelector(PlayerController owner, CharacterEntry[] characters)
    {
        this.owner = owner;
        this.characters = characters;
        SelectedCharacter = GetCharacterEntry(CharacterType.Capybara);
    }

    public void SetCharacter(CharacterType type)
    {
        CharacterEntry entry = GetCharacterEntry(type);

        if(entry.prefab == null) return;

        SelectedCharacter = entry;

        if (owner.ModelParent.childCount > 0){
            Object.Destroy(owner.ModelParent.GetChild(0).gameObject);
            var (center, radius) = Utils.GetVisualBounds(owner.gameObject);
            ParticleManager.Instance.SpawnParticle("clouds", center + new Vector3(0, 0, -0.72f), radius);
            owner.PlaySfxFromPlayer(AudioEvents.SFX.Character.ChangeCharacter);
            if(owner.IsCamouflaged) owner.SetCamouflaged(false);
        }

        Object.Instantiate(entry.prefab, owner.ModelParent);
        owner.Animator.avatar = entry.prefab.GetComponent<Animator>().avatar;
        owner.Animator.Rebind();
        owner.Animator.Update(0f);
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
    [FMODAudioEvent] public string voiceSound;

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