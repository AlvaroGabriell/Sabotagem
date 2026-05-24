/// <summary>
/// Guarda o personagem selecionado do player.
/// Esse script deve ser instanciado no PlayerControoler.
/// Pode ser usado para trocar o personagem em runtime por outros scripts (ex: tela de seleção).
/// </summary>
public class CharacterSelector
{
    public readonly PlayerController owner;
    public CharacterType SelectedCharacter {get; private set;} = CharacterType.Rabbit;

    public CharacterSelector(PlayerController owner)
    {
        this.owner = owner;
    }

    public void SetCharacter(CharacterType character)
    {
        SelectedCharacter = character;
    }

    public void Talk()
    {
        AudioManager.Instance.PlayOneShot($"{SelectedCharacter.ToString().ToLower()}Sound", owner.transform.position);
    }
}

public enum CharacterType
{
    Capybara,
    Rabbit,
    Raccoon,
    Chameleon,
}