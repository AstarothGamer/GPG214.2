using System;
using UnityEngine;

[Serializable]
public class AchievementSaveData
{
    public AchievmentDictionarySerialize achievements;
    public Vector3 position;

    public int totalJumps;
    public int totalAttacks;
    public float totalDistanceRan;
}