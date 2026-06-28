using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    [SerializeField] private InputActionAsset actions;

    private InputAction escapeAction;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            escapeAction = actions.FindAction("Escape");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Garante que apenas uma instância exista
        }
    }

    void OnEnable()
    {
        escapeAction.Enable();
        escapeAction.performed += OnEscape;
    }

    void OnDisable()
    {
        escapeAction.performed -= OnEscape;
        escapeAction.Disable();
    }

    private void OnEscape(InputAction.CallbackContext context)
    {
        if (UIController.Instance != null)
        {
            UIController.Instance.HandleEscape();
        }
    }
}
