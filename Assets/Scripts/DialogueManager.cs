using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI dialogueText; //text box that stores dialogue
    [SerializeField] private GameObject nextButton;
    [SerializeField] private ResponseHandler responseHandler; //reference to ResponseHandler.cs

    [Header("Dialogue Data")]
    [SerializeField] private DialogueDatabase dialogueDatabase;
    private Dictionary<string, DialogueEntry> dialogueLookup;

    [Header("Achievement Data")]
    [SerializeField] private AchievementManager achievementManager;

    //to store the order of lessons
    private List<string> lessonOrder = new List<string> { 
        "Ch1_Hello_Intro",
        "Ch1_Hello_Lesson",
        "Ch1_HowAreYou_Intro",
        "Ch1_HowAreYou_Lesson",
        "Ch1_ThankYou_Intro",
        "Ch1_ThankYou_Lesson",
        "Ch1_Goodbye_Intro",
        "Ch1_Goodbye_Lesson",
        "Ch1_Outro",
    };
    
    private int lessonIndex = 0; //to keep track of which lesson the player is currently on
    private string currentTopic; //stores the current lesson's name as a string
    private int currentLine = 0; //tracks the current line of text being displayed
    private bool isIntroSection = true; //bool to track if text is an intro or not

    void Awake()
    {
        dialogueLookup = new Dictionary<string, DialogueEntry>();

        foreach (var entry in dialogueDatabase.entries)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.topicKey))
            {
                dialogueLookup[entry.topicKey] = entry;
            }
        }
    }

    void Start()
    {
        StartLesson("Ch1_Hello_Intro"); //start first lesson
    }

    //start an intro or lesson
    public void StartLesson(string topic)
    {
        if (dialogueLookup.ContainsKey(topic))
        {
            currentTopic = topic;
            currentLine = 0;

            if (dialogueLookup[topic].introLines != null && dialogueLookup[topic].introLines.Length > 0)
            {
                isIntroSection = true;
            }
            else
            {
                isIntroSection = false;
            }

            ShowDialogue();
        }
    }

    //progress to the next lesson
    public void StartNextLesson()
    {
        if (lessonIndex < lessonOrder.Count - 1)
        {
            lessonIndex++;
            currentTopic = lessonOrder[lessonIndex];
            currentLine = 0;

            if (dialogueLookup[currentTopic].introLines != null && dialogueLookup[currentTopic].introLines.Length > 0)
            {
                isIntroSection = true;
                ShowDialogue();
            }
            else
            {
                isIntroSection = false;
                ShowDialogue();
                responseHandler.ShowResponses(currentTopic);
            }
        }
    }

    //display a line of dialogue
    public void ShowDialogue()
    {
        var entry = dialogueLookup[currentTopic];

        if (isIntroSection)
        {
            if (currentLine < entry.introLines.Length)
            {
                dialogueText.text = entry.introLines[currentLine];
                currentLine++;
            }
            else
            {
                var currentEntry = dialogueLookup[currentTopic];

                //check if topic triggers an achievement
                if (currentEntry.triggersAchievement && !string.IsNullOrEmpty(currentEntry.achievementId))
                {
                    //call the AchievementManager
                    achievementManager.ShowAchievement(currentEntry.achievementId);
                    nextButton.SetActive(false);
                    return;
                }

                lessonIndex++;
                if (lessonIndex < lessonOrder.Count)
                {
                    StartLesson(lessonOrder[lessonIndex]);
                }
                else
                {
                    dialogueText.text = "Great job! You have completed all lessons.";
                    nextButton.SetActive(false);
                }
            }

        }
        else
        {
            if (currentLine == 0)
            {
                dialogueText.text = entry.lessonQuestion;
                currentLine++;
                nextButton.SetActive(false); // Wait for answer
                responseHandler.ShowResponses(currentTopic);
            }
        }
    }

    //-----------------HELPER METHODS-----------------//
    public string GetLessonQuestion(string topic)
    {
        if (dialogueLookup.ContainsKey(topic))
        {
            return dialogueLookup[topic].lessonQuestion;
        }
        return "";
    }


    public bool HasMoreLessons()
    {
        return lessonIndex < lessonOrder.Count;
    }

    public bool IsIntroActive()
    {
        return isIntroSection;
    }

    public string GetNextLesson()
    {
        int nextIndex = lessonOrder.IndexOf(currentTopic) + 1;
        return nextIndex < lessonOrder.Count ? lessonOrder[nextIndex] : null;
    }
}
