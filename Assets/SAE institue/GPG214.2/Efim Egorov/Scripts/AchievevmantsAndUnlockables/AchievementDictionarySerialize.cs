using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievmentDictionarySerialize : MonoBehaviour
{
    public List<AchievementEntry> entries = new List<AchievementEntry>();

    public AchievmentDictionarySerialize(Dictionary<AchievementManager.AchievementType, bool> dictionary)
    {
        foreach (var pair in dictionary)
        {
            entries.Add(new AchievementEntry
            {
                key = pair.Key,
                value = pair.Value
            });
        }
    }

    public Dictionary<AchievementManager.AchievementType, bool> ToDictionary()
    {
        var dict = new Dictionary<AchievementManager.AchievementType, bool>();
        foreach (var entry in entries)
        {
            dict[entry.key] = entry.value;
        }
        return dict;
    }
}

[Serializable]
public class AchievementEntry
{
    public AchievementManager.AchievementType key;
    public bool value;
}
