public class SkillHelper
{
    public static void HandleSkill(PlayerController player)
    {
        switch (player.CharacterSelector.SelectedCharacter)
        {
            case CharacterType.Capybara:
                // Gorda
                break;

            case CharacterType.Rabbit:
                player.JumpController.OnRabbitJumpPressed();
                break;

            case CharacterType.Raccoon:
                
                break;

            case CharacterType.Chameleon:

                break;
        }
    }

    public static void Talk(CharacterType type, PlayerController player)
    {
        AudioManager.Instance.PlayOneShot($"{type.ToString().ToLower()}Sound", player.transform.position);
    }
}
