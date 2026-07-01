using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private GameObject levelTimerObj;
    [SerializeField] private TextMeshProUGUI levelTimer;

    public void ShowTimer()
    {
        levelTimerObj.SetActive(true);
        SetTimer(0f);
    }

    public void HideTimer()
    {
        if(levelTimerObj == null) return;
        levelTimerObj.SetActive(false);
    }

    public void SetTimer(float time)
    {
        levelTimer.text = LevelUI.FormatTime(time);
    }
}
