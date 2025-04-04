using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Achievements/Achievement")]
public class AchievementData : ScriptableObject
{
    public string id;
    public string title;
    public string description;
    public string details;
    public Sprite icon;
    public bool isUnlocked;
}
