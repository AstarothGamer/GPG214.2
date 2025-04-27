using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class PlayFabAchievementSync
{
    public static void SaveAchievementsToPlayFab(Dictionary<AchievementManager.AchievementType, bool> achievementProgress)
    {
        var userData = new Dictionary<string, string>();

        foreach (var achievement in achievementProgress)
        {
            userData.Add(achievement.Key.ToString(), achievement.Value.ToString());
        }

        userData.Add("TotalJumps", AchievementManager.totalJumps.ToString());
        userData.Add("TotalAttacks", AchievementManager.totalAttacks.ToString());
        userData.Add("TotalDistanceRan", AchievementManager.totalDistanceRan.ToString());

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = userData
        },
        result => Debug.Log("Achievements and player stats successfully saved to PlayFab!"),
        error => Debug.LogError("Error saving data to PlayFab: " + error.GenerateErrorReport()));
    }


    public static void LoadAchievementsFromPlayFab(Action<Dictionary<AchievementManager.AchievementType, bool>> onLoaded)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
        result =>
        {
            if (result.Data == null || result.Data.Count == 0)
            {
                Debug.LogWarning("No achievement data found on PlayFab.");
                onLoaded(new Dictionary<AchievementManager.AchievementType, bool>());
                return;
            }

            Dictionary<AchievementManager.AchievementType, bool> loadedAchievements = new Dictionary<AchievementManager.AchievementType, bool>();

            foreach (var entry in result.Data)
            {
                if (entry.Key == "PlayerPosition")
                {
                    // AchievementManager.position = JsonUtility.FromJson<Vector3>(entry.Value.Value);
                }
                else if (entry.Key == "TotalJumps")
                {
                    AchievementManager.totalJumps = int.Parse(entry.Value.Value);
                }
                else if (entry.Key == "TotalAttacks")
                {
                    AchievementManager.totalAttacks = int.Parse(entry.Value.Value);
                }
                else if (entry.Key == "TotalDistanceRan")
                {
                    AchievementManager.totalDistanceRan = float.Parse(entry.Value.Value);
                }
                else if (Enum.TryParse(entry.Key, out AchievementManager.AchievementType achievement))
                {
                    bool unlocked = bool.Parse(entry.Value.Value);
                    loadedAchievements.Add(achievement, unlocked);
                }
            }

            onLoaded(loadedAchievements);
        },
        error => Debug.LogError("Error loading achievements from PlayFab: " + error.GenerateErrorReport()));
    }
}
