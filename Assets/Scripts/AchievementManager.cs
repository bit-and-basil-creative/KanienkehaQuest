using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI achieveTitle;
    [SerializeField] private TextMeshProUGUI achieveDescription;
    [SerializeField] private TextMeshProUGUI achieveDetails;
    [SerializeField] private Image achieveIcon;
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private Button continueButton;

    [Header("Achievement Data")]
    [SerializeField] private AchievementDatabase achievementDatabase;
    public List<AchievementData> allAchievements;

    public void ShowAchievement(string id)
    {
        AchievementData achievement = allAchievements.Find(a => a.id == id);

        if (achievement != null)
        {
            UnlockAchievement(id); //to make sure the achievement is unlocked

            //fill in the UI panel fields
            achieveTitle.text = achievement.title;
            achieveDescription.text = achievement.description;
            achieveDetails.text = achievement.details;
            achieveIcon.sprite = achievement.icon;

            achievementPanel.SetActive(true); //show the UI panel
        }
    }

    public void UnlockAchievement(string id)
    {
        AchievementData achievement = System.Array.Find(achievementDatabase.achievement, a => a.id == id);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.isUnlocked = true;
            PlayerPrefs.SetInt(achievement.id, 1);
            PlayerPrefs.Save();
            Debug.Log($"Achievement Unlocked: {achievement.title}");
        }
    }

    public bool IsAchievementUnlocked(string id)
    {
        return PlayerPrefs.GetInt(id, 0) == 1;
    }
}