using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public static class AchievementManager
{
    public enum AchievementType
    {
        Jump10Times,
        Attack10Times,
        Run100Meters
    }

    private static Dictionary<AchievementType, bool> achievementProgress = new Dictionary<AchievementType, bool>();
    private static string saveFilePath;

    public static int totalJumps = 0;
    public static int totalAttacks = 0;
    public static float totalDistanceRan = 0f;

    private static bool saveScheduled = false;

    static AchievementManager()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "achievements.json");
        LoadAchievements();
    }

    public static void AddRunDistance(float distance)
    {
        totalDistanceRan += distance;
        CheckProgress();
        ScheduleSave();
    }

    public static void ScheduleSave()
    {
        if (!saveScheduled)
        {
            saveScheduled = true;
            PlayFabDelaySave.Instance.StartDelayedSave();
        }
    }

    public static void UnlockAchievement(AchievementType achievement)
    {
        if (!achievementProgress.ContainsKey(achievement) || !achievementProgress[achievement])
        {
            achievementProgress[achievement] = true;
            Debug.Log("Achievement unlocked: " + achievement);
            SaveAchievements();
            SaveAchievementsToCloud();
        }
    }

    private static void SaveAchievements()
    {
        AchievementSaveData saveData = new AchievementSaveData
        {
            achievements = new AchievmentDictionarySerialize(achievementProgress),
            // position = position,
            totalJumps = totalJumps,
            totalAttacks = totalAttacks,
            totalDistanceRan = totalDistanceRan
        };

        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, jsonData);
    }


    private static void LoadAchievements()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            AchievementSaveData saveData = JsonUtility.FromJson<AchievementSaveData>(jsonData);

            if (saveData != null)
            {
                if (saveData.achievements != null)
                    achievementProgress = saveData.achievements.ToDictionary();
                else
                    InitializeAchievements(); 


                // position = saveData.position;

                totalJumps = saveData.totalJumps;
                totalAttacks = saveData.totalAttacks;
                totalDistanceRan = saveData.totalDistanceRan;
            }
            else
            {
                Debug.LogWarning("Save data was null. Initializing achievements...");
                InitializeAchievements();
                totalJumps = 0;
                totalAttacks = 0;
                totalDistanceRan = 0f;
            }
        }
        else
        {
            Debug.LogWarning("No save file found. Initializing achievements...");
            InitializeAchievements();
            totalJumps = 0;
            totalAttacks = 0;
            totalDistanceRan = 0f;
        }
    }


    private static void InitializeAchievements()
    {
        foreach (AchievementType achievement in Enum.GetValues(typeof(AchievementType)))
        {
            achievementProgress[achievement] = false;
        }
    }

    public static bool IsAchievementUnlocked(AchievementType achievement)
    {
        return achievementProgress.ContainsKey(achievement) && achievementProgress[achievement];
    }

    public static void CheckProgress()
    {
        if (totalJumps >= 10)
            UnlockAchievement(AchievementType.Jump10Times);

        if (totalAttacks >= 10)
            UnlockAchievement(AchievementType.Attack10Times);

        if (totalDistanceRan >= 100f)
            UnlockAchievement(AchievementType.Run100Meters);
    }

    public static void SaveAchievementsToCloud()
    {
        PlayFabAchievementSync.SaveAchievementsToPlayFab(achievementProgress);
    }

    public static void LoadAchievementsFromCloud()
    {
        PlayFabAchievementSync.LoadAchievementsFromPlayFab(loadedProgress =>
        {
            if (loadedProgress.Count > 0)
            {
                achievementProgress = loadedProgress;
                Debug.Log("Achievements successfully loaded from PlayFab!");
            }
            else
            {
                Debug.LogWarning("No achievements loaded from PlayFab, initializing...");
                InitializeAchievements();
            }
        });
    }

    public static void SaveAchievementsLocally()
    {
        SaveAchievements();
    }

    public static void ResetSaveSchedule()
    {
        saveScheduled = false;
    }
}
