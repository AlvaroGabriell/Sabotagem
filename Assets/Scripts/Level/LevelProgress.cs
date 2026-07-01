using UnityEngine;

public class LevelProgress
{
    static string CompletedKey(string scene) => $"{scene}_Completed";
    static string BestTimeKey(string scene) => $"{scene}_BestTime";

    public static bool IsLevelCompleted(string scene)
    {
        return PlayerPrefs.GetInt(CompletedKey(scene), 0) == 1;
    }

    public static bool IsLevelUnlocked(string scene)
    {
        if(scene == "Level_01") return true;

        string[] split = scene.Split('_');

        if(split.Length != 2) return false;

        if(!int.TryParse(split[1], out int levelNumber)) return false;

        return IsLevelCompleted($"Level_{levelNumber - 1:D2}");
    }

    public static void CompleteLevel(string scene)
    {
        PlayerPrefs.SetInt(CompletedKey(scene), 1);
        PlayerPrefs.Save();
    }

    public static void SaveBestTime(string scene, float time)
    {
        CompleteLevel(scene);

        float oldTime = PlayerPrefs.GetFloat(BestTimeKey(scene), -1f);

        if (oldTime < 0 || time < oldTime)
        {
            PlayerPrefs.SetFloat(BestTimeKey(scene), time);
            PlayerPrefs.Save();
        }
    }

    public static float GetBestTime(string scene)
    {
        return PlayerPrefs.GetFloat(BestTimeKey(scene), -1f);
    }

    public static void ClearProgress()
    {
        for(int i = 1; i <= 99; i++)
        {
            string sceneName = $"Level_{i:D2}";
            PlayerPrefs.DeleteKey(CompletedKey(sceneName));
            PlayerPrefs.DeleteKey(BestTimeKey(sceneName));
        }

        PlayerPrefs.Save();
    }
}
