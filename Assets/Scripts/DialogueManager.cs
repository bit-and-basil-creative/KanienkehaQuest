using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText; //text box that stores dialogue
    public GameObject nextButton;
    public ResponseHandler responseHandler; //reference to ResponseHandler.cs

    [Header("Dialogue Data")]
    private Dictionary<string, string[]> lessonDialogues; //stores lesson specific questions
    private Dictionary<string, string[]> introductionDialogues; //stores intro dialogue for each lesson

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
    };
    
    private int lessonIndex = 0; //to keep track of which lesson the player is currently on
    private string currentTopic; //stores the current lesson's name as a string
    private int currentLine = 0; //tracks the current line of text being displayed
    private bool isIntroSection = true; //bool to track if text is an intro or not

    void Start()
    {
        InitializeDialogues(); //populate the dictionaries with text
        StartLesson("Ch1_Hello_Intro"); //start first lesson
    }

    //method to populate the dictionaries with intro dialogue & questions
    void InitializeDialogues()
    {
        introductionDialogues = new Dictionary<string, string[]>
        {
            { "Ch1_Hello_Intro", new string[] {
                "Shé:kon! Welcome, traveler. I am Ákat, a Clan Mother of this village.",
                "Here, we greet each other with respect and kindness.",
                "Let me teach you how we say hello in our language.",
            }},
            { "Ch1_HowAreYou_Intro", new string[] {
                "After greeting someone, it is polite to ask about their well-being.",
                "In our village, we care for one another and check in often.",
                "We have a way of asking how someone is doing. Let me teach it to you."
            }},
            { "Ch1_ThankYou_Intro", new string[] {
                "In our village, we always show gratitude — even for the smallest kindness.",
                "Saying thank you connects us, and reminds us that we are never alone.",
                "Let me teach you how we express appreciation in our language."
            }},
            { "Ch1_Goodbye_Intro", new string[] {
                "Parting is never final — we always hope to meet again.",
                "When we leave, we speak words of kindness and carry good thoughts with us.",
                "Let me show you how we say goodbye in our language."
            }},
        };

        lessonDialogues = new Dictionary<string, string[]>
        {
            { "Ch1_Hello_Lesson", new string[] { "What do you say when you meet someone for the first time?" }},
            { "Ch1_HowAreYou_Lesson", new string[] { "If I wanted to say ask how you are what do you think I might say?" }},
            { "Ch1_ThankYou_Lesson", new string[] { "If someone helps you or offers you food, what do you think you should say?" }},
            { "Ch1_Goodbye_Lesson", new string[] { "When it’s time to go, what do you think we might say?" }},
        };
    }

    //start an intro or lesson
    public void StartLesson(string topic)
    {
        //check if topic is an introduction or a lesson(question)
        if (introductionDialogues.ContainsKey(topic)) //if it's an introduction 
        {
            currentTopic = topic; //set current topic
            currentLine = 0; //reset current line
            isIntroSection = true; //set to true
            ShowDialogue(); //call Show Dialogue
        }
        else if (lessonDialogues.ContainsKey(topic)) //if it's a lesson(question)
        {
            currentTopic = topic; //set current topic
            currentLine = 0;//reset current line
            isIntroSection = false; //set to false
            ShowDialogue(); //call Show Dialogue
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

            if (introductionDialogues.ContainsKey(currentTopic)) // If it's an intro
            {
                isIntroSection = true;
                ShowDialogue();
            }
            else if (lessonDialogues.ContainsKey(currentTopic)) // If next part is a question
            {
                isIntroSection = false;
                ShowDialogue(); // Show the question first
                responseHandler.ShowResponses(currentTopic); // Then show responses
            }
        }
    }

    //display a line of dialogue
    public void ShowDialogue()
    {
        if (isIntroSection) //if it is an intro section
        {
            if (currentLine < introductionDialogues[currentTopic].Length)  //if there are more lines left in the intro
            {
                dialogueText.text = introductionDialogues[currentTopic][currentLine]; //display intro line
                currentLine++; //move to the next line
            }
            else //if there are no more intro lines left
            {
                lessonIndex++;
                if (lessonIndex < lessonOrder.Count)
                {
                    StartLesson(lessonOrder[lessonIndex]); // Next is the corresponding lesson question
                }
                else
                {
                    dialogueText.text = "Great job! You have completed all lessons.";
                    nextButton.SetActive(false);
                }
            }
        }
        else if (!isIntroSection) //if it's not an intro section it must be a lesson/question
        {
            if (currentLine < lessonDialogues[currentTopic].Length) //if there are more lines left in the lesson
            {
                dialogueText.text = lessonDialogues[currentTopic][currentLine]; //display the lesson line
                currentLine++; //move to the next line

                if (currentLine == lessonDialogues[currentTopic].Length) //if there are no more lines left in the lesson
                {
                    responseHandler.ShowResponses(currentTopic); //show responses for the current topic
                    nextButton.SetActive(false); //hide the next button
                }
            }
        }
    }

    //HELPER METHODS
    public string GetLessonQuestion(string topic)
    {
        if (lessonDialogues.ContainsKey(topic))
        {
            return lessonDialogues[topic][0]; // Returns the first question line
        }
        return ""; // Return empty string if the topic doesn't exist
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
