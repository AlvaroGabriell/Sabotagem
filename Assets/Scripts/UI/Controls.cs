using TMPro;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerText;

    public void Setup(string playerName)
    {
        playerText.text = playerName;
    }
}
