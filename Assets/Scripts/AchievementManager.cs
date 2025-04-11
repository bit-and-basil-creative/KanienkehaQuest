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
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject returnToMenuButton;

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

            if (id == "bracelet_complete") // use your actual ID here
            {
                continueButton.SetActive(false);
                returnToMenuButton.SetActive(true);
            }
            else
            {
                continueButton.SetActive(true);
                returnToMenuButton.SetActive(false);
            }

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

    public void ContinueJourneyClicked()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager != null)
        {
            string nextTopic = dialogueManager.GetNextLesson();

            if (!string.IsNullOrEmpty(nextTopic))
            {
                string chapterId = nextTopic.StartsWith("Final") ? "Final_Scene" : nextTopic.Split('_')[0];

                //start transition advance the lesson and chapter visuals
                GameManager.instance.ShowTransitionScreen(chapterId, () =>
                {
                    //hide the achievement panel
                    achievementPanel.SetActive(false);

                    //lload the next lesson
                    dialogueManager.ForceLoadLessonIndex(dialogueManager.GetCurrentLessonIndex() + 1);

                    //set the new chapter visuals
                    GameManager.instance.SetChapter(chapterId);

                    //turn next button back on
                    dialogueManager.ShowNextButton();
                });
            }
        }
    }

    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}