using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievementDatabase", menuName = "Achievements/Database")]

public class AchievementDatabase : ScriptableObject
{
    public AchievementData[] achievement;
}
