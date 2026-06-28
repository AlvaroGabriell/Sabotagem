public static class InputEvents
{
    public class PlayerAssignedEvent
    {
        public int playerIndex; // 1 ou 2
        public PlayerInputData inputData;
    }

    public class AssignmentCompleteEvent
    {
        public PlayerInputData player1;
        public PlayerInputData player2;
    }
}