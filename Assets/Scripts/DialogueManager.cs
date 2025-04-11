using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI dialogueText; //text box that stores dialogue
    [SerializeField] private GameObject nextButton;
    [SerializeField] private ResponseHandler responseHandler; //reference to ResponseHandler.cs
    [SerializeField] private AudioSource audioSource;

    [Header("Dialogue Data")]
    [SerializeField] private DialogueDatabase dialogueDatabase;
    private Dictionary<string, DialogueEntry> dialogueLookup;
    [SerializeField] private GameManager gameManager;

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
        "Ch2_Onkwehonwe_Intro",
        "Ch2_Onkwehonwe_Lesson",
        "Ch2_Kanienkeha_Intro",
        "Ch2_Kanienkeha_Lesson",
        "Ch2_Bear_Intro",
        "Ch2_Bear_Lesson",
        "Ch2_Turtle_Intro",
        "Ch2_Turtle_Lesson",
        "Ch2_Wolf_Intro",
        "Ch2_Wolf_Lesson",
        "Ch2_Outro",
        "Ch3_Red_Intro",
        "Ch3_Red_Lesson",
        "Ch3_Black_Intro",
        "Ch3_Black_Lesson",
        "Ch3_Yellow_Intro",
        "Ch3_Yellow_Lesson",
        "Ch3_White_Intro",
        "Ch3_White_Lesson",
        "Ch3_Outro",
        "Final_Scene"
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
        int loadFromSave = PlayerPrefs.GetInt("ShouldLoadFromSave", 0);

        var beadText = GameObject.Find("BeadText")?.GetComponent<TextMeshProUGUI>();

        if (BeadTracker.instance != null && beadText != null)
        {
            BeadTracker.instance.AssignBeadText(beadText);
        }

        if (loadFromSave == 1)
        {
            int savedLessonIndex = PlayerPrefs.GetInt("LessonIndex", 0);
            lessonIndex = savedLessonIndex;
            PlayerPrefs.SetInt("ShouldLoadFromSave", 0);

            //load saved word basket
            FindObjectOfType<GameSaver>().LoadWordBasket();

            //load saved bead count
            int savedBeads = PlayerPrefs.GetInt("BeadCount", 0);
            BeadTracker.instance.SetBeadCount(savedBeads);
        }
        else
        {
            lessonIndex = 0;
        }

        if (gameManager != null)
        {
            gameManager.SetChapter(lessonOrder[lessonIndex].Split('_')[0]);
        }

        StartLesson(lessonOrder[lessonIndex]);
    }

    public void StartLessonFromBeginning()
{
    lessonIndex = 0;
    StartLesson(lessonOrder[lessonIndex]);
}

    public void ForceLoadLessonIndex(int index)
    {
        lessonIndex = index;
        StartLesson(lessonOrder[lessonIndex]);
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

            string newChapterId = lessonOrder[lessonIndex].Split('_')[0];

            if (gameManager != null)
            {
                gameManager.SetChapter(newChapterId);
            }

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
                    string finalKey = "Final_Scene";

                    if (dialogueLookup.ContainsKey(finalKey))
                    {
                        currentTopic = finalKey;
                        currentLine = 0;

                        if (dialogueLookup[finalKey].introLines != null && dialogueLookup[finalKey].introLines.Length > 0)
                        {
                            isIntroSection = true;
                        }
                        else
                        {
                            isIntroSection = false;
                        }

                        ShowDialogue(); // recursively show final scene
                    }
                    else
                    {
                        // fallback just in case the FinalScene key is missing
                        dialogueText.text = "Great job! You have completed all lessons.";
                        nextButton.SetActive(false);
                    }
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

    public int GetCurrentLessonIndex()
    {
        return lessonIndex;
    }

    public void ShowNextButton()
    {
        nextButton.SetActive(true);
    }
}
