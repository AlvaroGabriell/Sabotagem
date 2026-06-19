public class SkillHelper
{
    public static void HandleSkill(PlayerController player)
    {
        switch (player.CharacterSelector.SelectedCharacter.type)
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
                player.SetCamouflaged(!player.IsCamouflaged);
                break;
        }
    }

    public static void Talk(CharacterEntry characterEntry, PlayerController player)
    {
        AudioManager.Instance.PlayOneShot(characterEntry.voiceSound, player.transform.position);
    }
}
