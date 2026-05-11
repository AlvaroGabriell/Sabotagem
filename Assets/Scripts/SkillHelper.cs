public class SkillHelper
{
    public static void HandleSkill(PlayerController player)
    {
        switch (player.CharacterSelector.SelectedCharacter)
        {
            case CharacterType.Capybara:
                // Inútil
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
}
