using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI levelNameText;

    private Coroutine loadingTextCoroutine;

    public void StartLoadingScreen(string levelText)
    {
        if (loadingTextCoroutine != null)
        {
            StopCoroutine(loadingTextCoroutine);
        }
        loadingScreen.SetActive(true);
        loadingTextCoroutine = StartCoroutine(AnimateLoadingText());
        levelNameText.text = levelText;
    }

    public void StopLoadingScreen()
    {
        if (loadingTextCoroutine != null)
        {
            StopCoroutine(loadingTextCoroutine);
            loadingTextCoroutine = null;
        }
        loadingScreen.SetActive(false);
    }

    private IEnumerator AnimateLoadingText()
    {
        string baseText = "Loading";
        int dotCount = 0;
        while (true)
        {
            loadingText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // Cycle through 0, 1, 2, 3
            yield return _waitForSeconds0_5; // Update every half second
        }
    }

    public static string GetLevelNameFromSceneName(string sceneName)
    {
        // Assuming the scene name is in the format "Level_X" where X is the level number
        if (sceneName.StartsWith("Level_"))
        {
            return "Level " + sceneName.Split('_')[1];
        }
        return sceneName; // Return the original name if it doesn't match the expected format
    }
}
