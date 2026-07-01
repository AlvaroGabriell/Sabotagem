using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject LeftKeyboardPrefab;
    [SerializeField] private GameObject RightKeyboardPrefab;
    [SerializeField] private GameObject GamepadPrefab;

    [SerializeField] private Transform controlsContainer;

    void OnEnable()
    {
        EventBus<InputEvents.AssignmentCompleteEvent>.Subscribe(OnAssignmentComplete);

        RefreshControls();
    }

    void OnDisable()
    {
        EventBus<InputEvents.AssignmentCompleteEvent>.Unsubscribe(OnAssignmentComplete);
    }

    public void RefreshControls()
    {
        PlayerInputData player1 = InputAssignmentManager.Instance.Player1;
        PlayerInputData player2 = InputAssignmentManager.Instance.Player2;

        foreach (Transform child in controlsContainer)
        {
            Destroy(child.gameObject);
        }

        if(player1.Slot != InputSlot.None)
        {
            GameObject controlUIObj;
            if(player1.Slot is InputSlot.Keyboard_Left)
            {
                controlUIObj = Instantiate(LeftKeyboardPrefab, controlsContainer);
            }
            else if(player1.Slot is InputSlot.Keyboard_Right)
            {
                controlUIObj = Instantiate(RightKeyboardPrefab, controlsContainer);
            }
            else
            {
                controlUIObj = Instantiate(GamepadPrefab, controlsContainer);
            }

            if(controlUIObj.TryGetComponent<Controls>(out var controls))
            {
                controls.Setup("Jogador 1");
            }
        }

        if(player2.Slot != InputSlot.None)
        {
            GameObject controlUIObj;
            if(player2.Slot is InputSlot.Keyboard_Left)
            {
                controlUIObj = Instantiate(LeftKeyboardPrefab, controlsContainer);
            }
            else if(player2.Slot is InputSlot.Keyboard_Right)
            {
                controlUIObj = Instantiate(RightKeyboardPrefab, controlsContainer);
            }
            else
            {
                controlUIObj = Instantiate(GamepadPrefab, controlsContainer);
            }

            if(controlUIObj.TryGetComponent<Controls>(out var controls))
            {
                controls.Setup("Jogador 2");
            }
        }
    }

    private void OnAssignmentComplete(InputEvents.AssignmentCompleteEvent evt)
    {
        UIController.Instance.CloseCurrentMenu();
        if(!isActiveAndEnabled) return;
        RefreshControls();
    }
}