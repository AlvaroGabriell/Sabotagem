using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenu : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private GameObject levelCompletedPrefab;
    [SerializeField] private GameObject levelLockedPrefab;

    [SerializeField] private Transform levelContainer;

    [SerializeField] private List<string> levelsSceneNames;

    void OnEnable()
    {
        RefreshLevels();
    }

    public void RefreshLevels()
    {
        foreach (Transform child in levelContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (string sceneName in levelsSceneNames)
        {
            GameObject levelUIObj;
            if (LevelProgress.IsLevelCompleted(sceneName))
            {
                levelUIObj = Instantiate(levelCompletedPrefab, levelContainer);
            }
            else if (LevelProgress.IsLevelUnlocked(sceneName))
            {
                levelUIObj = Instantiate(levelPrefab, levelContainer);
            }
            else
            {
                levelUIObj = Instantiate(levelLockedPrefab, levelContainer);
            }

            if (levelUIObj.TryGetComponent<LevelUI>(out var levelUI))
            {
                levelUI.Setup(sceneName);
            }
        }
    }
}
