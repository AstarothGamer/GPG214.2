using UnityEngine;

public class AchievementUsing : MonoBehaviour
{
    public PlayerInput player;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = player.transform.position;

        PlayFabLogin.OnLoginSuccess += LoadAchievementsAfterLogin;    
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            DebugAchievements();
        }
    }

    private void LoadAchievementsAfterLogin()
    {
        AchievementManager.LoadAchievementsFromCloud();
    }

    public void OnPlayerJump()
    {
        AchievementManager.totalJumps++;
        AchievementManager.CheckProgress();
        AchievementManager.ScheduleSave();
    }

    public void OnPlayerAttack()
    {
        AchievementManager.totalAttacks++;
        AchievementManager.CheckProgress();
        AchievementManager.ScheduleSave();
    }

    public void TrackDistance()
    {
        float distance = Vector3.Distance(player.transform.position, lastPosition);

        if (distance > 0.1f)
        {
            AchievementManager.AddRunDistance(distance);
            lastPosition = player.transform.position;
        }
    }

    void DebugAchievements()
    {
        bool jumpAchieved = AchievementManager.IsAchievementUnlocked(AchievementManager.AchievementType.Jump10Times);
        bool attackAchieved = AchievementManager.IsAchievementUnlocked(AchievementManager.AchievementType.Attack10Times);
        bool runAchieved = AchievementManager.IsAchievementUnlocked(AchievementManager.AchievementType.Run100Meters);

        Debug.Log($"Jump Achievement: {jumpAchieved}");
        Debug.Log($"Attack Achievement: {attackAchieved}");
        Debug.Log($"Run Achievement: {runAchieved}");
    }
}
