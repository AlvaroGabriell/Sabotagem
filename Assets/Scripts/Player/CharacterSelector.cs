using UnityEngine;

/// <summary>
/// Guarda o personagem selecionado do player.
/// Esse script deve ser instanciado no PlayerControoler.
/// Pode ser usado para trocar o personagem em runtime por outros scripts (ex: tela de seleção).
/// </summary>
public class CharacterSelector
{
    public readonly PlayerController owner;
    public CharacterType SelectedCharacter {get; private set;} = CharacterType.Rabbit;

    private readonly Transform modelParent;

    public CharacterEntry[] characters;

    public CharacterSelector(PlayerController owner, CharacterEntry[] characters)
    {
        this.owner = owner;
        this.characters = characters;
        modelParent = owner.transform.Find("Model");
    }

    public void SetCharacter(CharacterType type)
    {
        SelectedCharacter = type;

        GameObject prefab = GetPrefab(type);
        if(prefab == null) return;

        if (modelParent.childCount > 0){
            Object.Destroy(modelParent.GetChild(0).gameObject);
            ParticleManager.Instance.SpawnParticle("clouds", Utils.GetVisualCenter(owner.gameObject) + new Vector3(0, 0, -0.72f));
        }

        Object.Instantiate(prefab, modelParent);
    }

    private GameObject GetPrefab(CharacterType type)
    {
        foreach(var entry in characters)
        {
            if(entry.type == type) return entry.prefab;
        }
        return null;
    }
}

[System.Serializable]
public struct CharacterEntry
{
    public CharacterType type;
    public GameObject prefab;
}

public enum CharacterType
{
    Capybara,
    Rabbit,
    Raccoon,
    Chameleon,
}