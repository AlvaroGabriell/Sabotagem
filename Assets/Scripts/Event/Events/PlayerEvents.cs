public static class PlayerEvents
{
    public class CharacterSwapped : PlayerEventBase
    {
        public CharacterEntry previousCharacter;
        public CharacterEntry newCharacter;
    }
}

public abstract class PlayerEventBase : ICancelableEvent
{
    public bool Canceled { get; set; }
    public PlayerController player;
}
