using UnityEngine;
using UnityEngine.UI;

public class CharacterWheel : MonoBehaviour
{
    [System.Serializable]
    public struct WheelEntry
    {
        public CharacterType type;
        public Image highlight;
    }

    [SerializeField] private WheelEntry[] entries;
    [SerializeField] private PlayerController player;

    public bool IsOpen { get; private set; } = false;
    private CharacterType hoveredType;

    private static readonly (Vector2 dir, CharacterType type)[] directionMap =
    {
        (Vector2.up,    CharacterType.Rabbit),
        (Vector2.right, CharacterType.Capybara),
        (Vector2.down,  CharacterType.Chameleon),
        (Vector2.left,  CharacterType.Raccoon),
    };

    void Start()
    {
        gameObject.SetActive(false);
        hoveredType = player.CharacterSelector.SelectedCharacter.type;
    }

    public void Toggle()
    {
        IsOpen = !IsOpen;
        gameObject.SetActive(IsOpen);

        if (IsOpen)
        {
            hoveredType = player.CharacterSelector.SelectedCharacter.type;
            UpdateHighlight();
        } else
        {
            if(hoveredType != player.CharacterSelector.SelectedCharacter.type) player.CharacterSelector.SetCharacter(hoveredType);
        }
    }

    public void OnDirectionInput(Vector2 input)
    {
        if(!IsOpen || input == Vector2.zero) return;

        float bestDot = -Mathf.Infinity;
        CharacterType best = hoveredType;

        foreach(var (dir, type) in directionMap)
        {
            float dot = Vector2.Dot(input.normalized, dir);
            if(dot > bestDot)
            {
                bestDot = dot;
                best = type;
            }
        }

        hoveredType = best;
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        foreach(var entry in entries)
        {
            entry.highlight.gameObject.SetActive(entry.type == hoveredType);
        }
    }
}
