using UnityEngine;

public class PlayFabDelaySave : MonoBehaviour
{
    public static PlayFabDelaySave Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDelayedSave()
    {
        Invoke(nameof(Save), 5f); 
    }

    private void Save()
    {
        AchievementManager.SaveAchievementsToCloud();
        AchievementManager.SaveAchievementsLocally();
        AchievementManager.ResetSaveSchedule();
    }
}

