using UnityEngine.InputSystem;

public class PlayerInputData
{
    public InputSlot Slot { get; set; } = InputSlot.None;
    public InputDevice Device { get; set; } = null;
}
