using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class InputAssignmentManager : MonoBehaviour
{
    public static InputAssignmentManager Instance { get; private set; }

    public PlayerInputData Player1 { get; private set; } = new();
    public PlayerInputData Player2 { get; private set; } = new();

    private bool waitingForPlayer1 = true;
    private bool waitingForPlayer2 = false;

    private static readonly string[] keyboardLeftKeys = { "w", "a", "s", "d", "e", "space", "f" };
    private static readonly string[] keyboardRightKeys = { "upArrow", "downArrow", "leftArrow", "rightArrow", "shift", "enter", "oem2" };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject); // Garante que apenas uma instância exista
    }

    void OnEnable()
    {
        InputUser.listenForUnpairedDeviceActivity = 1;
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
        InputSystem.onDeviceChange -= OnDeviceChange;
        InputUser.listenForUnpairedDeviceActivity = 0;
    }

    // Chamado quando qualquer botão é pressionado em dispositivo sem usuário atribuído
    private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr)
    {
        if(!waitingForPlayer1 && !waitingForPlayer2) return;

        InputDevice device = control.device;

        InputSlot slotChosen;
        if(device is Keyboard)
        {
            // Determina se foi WASD (KeyboardLeft) ou setas (KeyboardRight)
            // Para simplicidade, verificamos qual controle foi pressionado:
            // Em vez de ler Key, vamos pegar o controle usável.
            // Por exemplo, se W/A/S/D
            if (System.Array.IndexOf(keyboardLeftKeys, control.name) >= 0)
                slotChosen = InputSlot.Keyboard_Left;
            else if (System.Array.IndexOf(keyboardRightKeys, control.name) >= 0)
                slotChosen = InputSlot.Keyboard_Right;
            else
                slotChosen = InputSlot.None; // tecla não relevante
        }
        else if (device is Gamepad)
        {
            slotChosen = InputSlot.Gamepad;
        }
        else
        {
            slotChosen = InputSlot.None;
        }

        if (slotChosen == InputSlot.None) return;

        if (slotChosen == InputSlot.Gamepad)
        {
            if (Player1.Device == device || Player2.Device == device) return;
        }
        else
        {
            if ((slotChosen == Player1.Slot && waitingForPlayer2) || (slotChosen == Player2.Slot && waitingForPlayer1)) return;
        }

        // Conclui a atribuição para o jogador atual
        if (waitingForPlayer1)
        {
            Player1.Slot = slotChosen;
            Player1.Device = device;
            waitingForPlayer1 = false;
            waitingForPlayer2 = true;

            eventPtr.handled = true;

            EventBus<InputEvents.PlayerAssignedEvent>.Publish(new InputEvents.PlayerAssignedEvent
            {
                playerIndex = 1,
                inputData = Player1
            });
        }
        else if (waitingForPlayer2)
        {
            Player2.Slot = slotChosen;
            Player2.Device = device;
            waitingForPlayer2 = false;

            eventPtr.handled = true;
            
            EventBus<InputEvents.PlayerAssignedEvent>.Publish(new InputEvents.PlayerAssignedEvent
            {
                playerIndex = 2,
                inputData = Player2
            });
            EventBus<InputEvents.AssignmentCompleteEvent>.Publish(new InputEvents.AssignmentCompleteEvent
            {
                player1 = Player1,
                player2 = Player2
            });
        }
    }

    // Se um dispositivo for desconectado, pausa o jogo e avisa
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Disconnected)
        {
            // Verifica se o device era usado por algum jogador
            if ((Player1.Device != null && Player1.Device == device) ||
                (Player2.Device != null && Player2.Device == device))
            {
                GameController.Instance.PauseGame();
                // TODO: Abrir tela avisando que jogador foi desconectado.
            }
        }
    }

    public void ResetAssignments()
    {
        Player1 = new PlayerInputData();
        Player2 = new PlayerInputData();
        waitingForPlayer1 = true;
        waitingForPlayer2 = false;
    }

    public bool AreAllPlayersAssigned()
    {
        return Player1.Slot != InputSlot.None && Player2.Slot != InputSlot.None;
    }
}
