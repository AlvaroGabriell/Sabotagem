using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    private string sceneName;
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private TextMeshProUGUI bestTimeText;

    void Start()
    {
        SetBestTime();
    }

    public void Setup(string sceneName)
    {
        this.sceneName = sceneName;
        levelNumberText.text = sceneName.Split('_')[1];

        SetBestTime();

        if(playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() =>
            {
                UIController.Instance.OnSelectLevel(sceneName);
            });
        }
    }

    public void SetBestTime()
    {
        if(bestTimeText == null) return;
        float bestTime = LevelProgress.GetBestTime(sceneName);

        if(bestTime < 0)
        {
            bestTimeText.text = "--:--";
        }
        else
        {
            bestTimeText.text = FormatTime(bestTime);
        }
    }

    public static string FormatTime(float seconds)
    {
        int min = Mathf.FloorToInt(seconds / 60f);
        int sec = Mathf.FloorToInt(seconds % 60f);

        return $"{min:00}:{sec:00}";
    }

}
