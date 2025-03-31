using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Text;

public class ResponseHandler : MonoBehaviour
{
    [Header("UI References")]
    public GameObject nextButton;
    public GameObject responseOptions; //container that holds all response text objects
    public TextMeshProUGUI[] responseTexts; //array of responses
    public TextMeshProUGUI dialogueText; //text box to show dialogue
    public GameObject correctWordGroup; //container that holds the Play button and Correct Word
    public TextMeshProUGUI correctWordText;
    public Button playButton;

    [Header("Audio")]
    public AudioSource audioSource; //reference to audio source component
    public AudioClip lastPlayedClip; //stores the last played audio clip to be replayed if needed

    [Header("Script References")]
    public BeadTracker beadTracker; //reference to bead tracker script
    public DialogueManager dialogueManager; //reference to dialogue manager script

    [Header("State Tracking")]
    private string currentTopic; //stores current topic being handled
    private bool isAnswerCorrect = false; //bool to track if response is correct/incorrect

    private Dictionary<string, string[]> responseOptionsData = new Dictionary<string, string[]>
    {
        //stores response choices for each lesson
        //link tags indicate which answer is correct/wrong
        //responseOptionsData is used in ShowResponses()

        { "Ch1_Hello_Lesson", new string[] { "<link=correct>Shé:kon!</link>", "<link=wrong>Ohkwá:ri</link>", "<link=wrong>What's up?</link>" } },
        { "Ch1_HowAreYou_Lesson", new string[] { "<link=wrong>How are you?</link>", "<link=correct>Skennenkó: ken?</link>", "<link=wrong>Nia:wen</link>" } },
        { "Ch1_ThankYou_Lesson", new string[] { "<link=wrong>Okay</link>", "<link=wrong>Ohkwá:ri</link>", "<link=correct>Nia:wen</link>" } },
        { "Ch1_Goodbye_Lesson", new string[] { "<link=correct>Ó:nen ki’wáhi</link>", "<link=wrong>Shé:kon!</link>", "<link=wrong>See ya!</link>" } },
    };

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); //assign the audio source reference
    }

    public void ShowResponses(string topic)
    {
        isAnswerCorrect = false;

        currentTopic = topic; //store current question

        nextButton.SetActive(false); //hide Next button while responses are being shown
        responseOptions.SetActive(true); //unhide response options container

        ClearResponses(); //clear old responses to prevent overlap

        if (responseOptionsData.ContainsKey(currentTopic))
        {
            string[] choicesText = responseOptionsData[currentTopic];

            for (int i = 0; i < responseTexts.Length; i++)
            {
                //loop through responseTexts array, populate the text fields and unhide the game objects

                responseTexts[i].text = choicesText[i]; //set response text inside each text object
                responseTexts[i].gameObject.SetActive(true); //unhide all response options text objects

                //assign click events to each response
                int choiceIndex = i; //store local copy of index

                //check if the response contains link = correct and assign each response accordingly
                string linkID = choicesText[i].Contains("<link=correct>") ? "correct" : "wrong";

                //call ClickableText on each response
                responseTexts[i].GetComponent<ClickableText>().Setup(this, choiceIndex, linkID);
            }

            //display the question in the dialogue box
            dialogueText.text = GetQuestionText(currentTopic);
        }
    }

    //method to process the player's answer
    public void HandleResponse(int choiceIndex, string linkID)
    {

        if (linkID == "correct") //if they click the correct response
        {
            isAnswerCorrect = true; //mark response as correct
            responseOptions.SetActive(false); //hide response options

            beadTracker.AddBead(); //update bead tracker
            
            //update the dialogue box
            dialogueText.text = $"Yes! You are learning well. Now let's try to say it out loud:\n";
            correctWordGroup.gameObject.SetActive(true);
            correctWordText.text= responseTexts[choiceIndex].text;

            //load the corresponding audio clip based on normalized string correctWord
            string correctWord = NormalizeAudioClipName(responseTexts[choiceIndex].text);
            lastPlayedClip = Resources.Load<AudioClip>($"Audio/{correctWord}");

            //add the play audio button
            if (lastPlayedClip != null) //if an audio clip is found
            {
                if (playButton != null) //if play button reference exists
                {
                    playButton.gameObject.SetActive(true); //make the play button active
                    playButton.onClick.RemoveAllListeners(); //remove all event listeners from play button
                    playButton.onClick.AddListener(() => ReplayAudio()); //allow user to replay audio clip
                }
            }

            nextButton.SetActive(true); //show Next button to continue
        }
        else if (linkID == "wrong") //if they click an incorrect response
            {
            dialogueText.text = "Not quite. Try again. " + GetQuestionText(currentTopic); //update dialogue text

            if (choiceIndex >= 0 && choiceIndex < responseTexts.Length)
            {
                //make the incorrect response they chose unclickable and greyed out
                GreyOutResponse(responseTexts[choiceIndex]);
                responseTexts[choiceIndex].GetComponent<ClickableText>().enabled = false;
            }
        }
    }

    //method to progress to the next lesson after a correct answer has been made
    public void NextButtonClicked()
    {
        if (correctWordGroup != null) //hide the correct word group and reset the text box
        {
            correctWordGroup.gameObject.SetActive(true);
            correctWordText.text = "";
        }

        if (playButton != null) //if a reference to the play button exists
        {
            playButton.gameObject.SetActive(false); //hide the play button
        }

        Debug.Log($"[NextButtonClicked] Button clicked. isAnswerCorrect: {isAnswerCorrect}");

        if (dialogueManager.IsIntroActive())
        {
            Debug.Log("[NextButtonClicked] Still in an intro. Continuing dialogue.");
            dialogueManager.ShowDialogue();
        }
        else if (isAnswerCorrect)
        {
            {
                dialogueText.text = ""; //clear the dialogue box

                Debug.Log($"[NextButtonClicked] Answer is correct. Calling StartNextLesson()...");
                dialogueManager.StartNextLesson(); //move to the next lesson

                //check if there are more lessons remaining
                if (!dialogueManager.HasMoreLessons())
                {
                    Debug.Log("[NextButtonClicked] No more lessons. Hiding next button.");
                    nextButton.SetActive(false);
                }
            }
        }
    }

    //-------HELPER METHODS---------//

    private string GetQuestionText(string topic)
    {
        return dialogueManager.GetLessonQuestion(topic);
    }

    public void ReplayAudio()
    {
        if (lastPlayedClip != null)
        {
            audioSource.PlayOneShot(lastPlayedClip);
        }
    }

    public void ClearResponses()
    {
        foreach (var response in responseTexts)
        {
            response.text = "";
            response.color = new Color32(60, 36, 21, 255); // original dark brown
            response.gameObject.SetActive(false);

            var clickable = response.GetComponent<ClickableText>();
            if (clickable != null)
            {
                clickable.enabled = true;
            }
        }
    }
    private void GreyOutResponse(TextMeshProUGUI text)
    {
        string cleaned = Regex.Replace(text.text, "<.*?>", "");
        text.text = $"<color=#999999>{cleaned}</color>";
    }

    private string NormalizeAudioClipName(string input)
    {
        // Remove TMP formatting (e.g., <color>, <link>, etc.)
        string cleanText = Regex.Replace(input, "<.*?>", "");

        // Normalize and remove accents
        cleanText = cleanText.Normalize(NormalizationForm.FormD); // Decomposes characters
        cleanText = new string(cleanText.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

        // Remove punctuation and special characters, keep only letters and numbers
        cleanText = Regex.Replace(cleanText, @"[^a-zA-Z0-9]", "");

        return cleanText;
    }
}
