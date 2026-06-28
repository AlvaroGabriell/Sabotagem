using TMPro;
using UnityEngine;

[EventBusSubscriber]
public class InputAssignmentUI : MonoBehaviour
{
    [Header("Referências de UI")]
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;

    private void OnEnable()
    {
        EventBus<InputEvents.PlayerAssignedEvent>.Subscribe(HandlePlayerAssigned);
        EventBus<InputEvents.AssignmentCompleteEvent>.Subscribe(HandleAssignmentComplete);

        InputAssignmentManager.Instance.ResetAssignments();
        UpdateUI();
    }

    void OnDisable()
    {
        EventBus<InputEvents.PlayerAssignedEvent>.Unsubscribe(HandlePlayerAssigned);
        EventBus<InputEvents.AssignmentCompleteEvent>.Unsubscribe(HandleAssignmentComplete);
    }

    private void UpdateUI()
    {
        if (InputAssignmentManager.Instance.Player1.Slot == InputSlot.None)
        {
            player1Text.text = "Pressione qualquer botão...";
            player1Text.color = new Color32(155, 10, 5, 255);
            player2Text.text = "Aguardando...";
            player2Text.color = new Color32(152, 144, 205, 255);
        }
        if (InputAssignmentManager.Instance.Player1.Slot != InputSlot.None && InputAssignmentManager.Instance.Player2.Slot == InputSlot.None)
        {
            player1Text.text = "Pronto!";
            player1Text.color = new Color32(152, 144, 205, 255);
            player2Text.text = "Pressione qualquer botão...";
            player2Text.color = new Color32(155, 10, 5, 255);
        }
        
        // Atualiza ícones de acordo com escolhas feitas até agora (opcional)
        // Por exemplo, se P1 escolheu Gamepad, mostre o ícone de gamepad no lado de P1.
        // TODO: consertar

        //iconKeyboardLeft.SetActive(false);
        //iconKeyboardRight.SetActive(false);
        //iconGamepad.SetActive(false);
        //
        //if (InputAssignmentManager.Instance.Player1.Slot == InputSlot.KeyboardLeft) iconKeyboardLeft.SetActive(true);
        //if (InputAssignmentManager.Instance.Player1.Slot == InputSlot.KeyboardRight) iconKeyboardRight.SetActive(true);
        //if (InputAssignmentManager.Instance.Player1.Slot == InputSlot.Gamepad) iconGamepad.SetActive(true);

        // TODO: fazer pro p2
    }

    private void HandlePlayerAssigned(InputEvents.PlayerAssignedEvent evt)
    {
        // Um jogador atribuiu seu controle: atualiza UI para o próximo
        UpdateUI();
    }

    private void HandleAssignmentComplete(InputEvents.AssignmentCompleteEvent evt)
    {
        // Ambos jogadores atribuíram: fecha tela e avança
        // TODO: fechar a tela de forma correta.
        UpdateUI();
        gameObject.SetActive(false);

        // Chama o próximo passo no GameController/UIController
        // Por exemplo, abre a seleção de nível:
        UIController.Instance.OnSelectLevel("SampleScene");
        //UIController.Instance.OpenMenu(UIController.Instance.levelSelectMenu);
    }
}
