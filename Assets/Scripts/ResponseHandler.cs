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
    [SerializeField] private GameObject nextButton;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject responseOptions; //container that holds all response text objects
    [SerializeField] private TextMeshProUGUI[] responseTexts; //array of responses
    [SerializeField] private TextMeshProUGUI dialogueText; //text box to show dialogue
    [SerializeField] private GameObject correctWordGroup; //container that holds the Play button and Correct Word
    [SerializeField] private TextMeshProUGUI correctWordText;
    [SerializeField] private WordBasketManager wordBasketManager;
    [SerializeField] private GameObject achievementPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; //reference to audio source component
    [SerializeField] private AudioClip incorrectAnswerClip;
    [SerializeField] private AudioClip correctAnswerClip;
    private AudioClip lastPlayedClip; //stores the last played audio clip to be replayed if needed

    [Header("Script References")]
    [SerializeField] private BeadTracker beadTracker; //reference to bead tracker script
    [SerializeField] private DialogueManager dialogueManager; //reference to dialogue manager script
    [SerializeField] private DialogueDatabase dialogueDatabase; //reference to the dialogue database

    [Header("State Tracking")]
    private string currentTopic; //stores current topic being handled
    private bool isAnswerCorrect = false; //bool to track if response is correct/incorrect

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); //assign the audio source reference
    }

    public void ShowResponses(string topic)
    {
        isAnswerCorrect = false;
        currentTopic = topic;

        nextButton.SetActive(false);
        responseOptions.SetActive(true);
        ClearResponses();

        DialogueEntry entry = System.Array.Find(dialogueDatabase.entries, e => e.topicKey == currentTopic);

        if (entry != null && entry.responseOptions != null && entry.responseOptions.Length > 0)
        {
            string[] choicesText = entry.responseOptions;

            for (int i = 0; i < responseTexts.Length; i++)
            {
                responseTexts[i].text = choicesText[i];
                responseTexts[i].gameObject.SetActive(true);

                int choiceIndex = i;
                string linkID = choicesText[i].Contains("<link=correct>") ? "correct" : "wrong";

                responseTexts[i].GetComponent<ClickableText>().Setup(this, choiceIndex, linkID);
            }

            dialogueText.text = GetQuestionText(currentTopic);
        }
    }

    //method to process the player's answer
    public void HandleResponse(int choiceIndex, string linkID)
    {
        if (linkID == "correct") //if they click the correct response
        {
            if (correctAnswerClip != null)
            {
                audioSource.PlayOneShot(correctAnswerClip);
            }

            //add word to WordBasket and load audio
            DialogueEntry entry = System.Array.Find(dialogueDatabase.entries, e => e.topicKey == currentTopic);
            if (entry != null)
            {
                wordBasketManager.AddWord(entry);
                lastPlayedClip = entry.wordAudio;
            }

            isAnswerCorrect = true; //mark response as correct
            responseOptions.SetActive(false); //hide response options

            beadTracker.AddBead(); //update bead tracker
            
            //update the dialogue box
            dialogueText.text = $"Yes! You are learning well. Now let's try to say it out loud:\n";
            correctWordGroup.gameObject.SetActive(true);
            correctWordText.text= responseTexts[choiceIndex].text;

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

            if (incorrectAnswerClip != null)
            {
                audioSource.PlayOneShot(incorrectAnswerClip);
            }
        }
    }

    //method to progress to the next lesson after a correct answer has been made
    public void NextButtonClicked()
    {
        if (correctWordGroup != null) //hide the correct word group and reset the text box
        {
            correctWordGroup.gameObject.SetActive(false);
            correctWordText.text = "";
        }

        if (playButton != null) //if a reference to the play button exists
        {
            playButton.gameObject.SetActive(false); //hide the play button
        }

        if (dialogueManager.IsIntroActive())
        {
            dialogueManager.ShowDialogue();
        }
        else if (isAnswerCorrect)
        {
            {
                dialogueText.text = ""; //clear the dialogue box
                dialogueManager.StartNextLesson(); //move to the next lesson

                //check if there are more lessons remaining
                if (!dialogueManager.HasMoreLessons())
                {
                    nextButton.SetActive(false);
                }
            }
        }
    }

    //-------HELPER METHODS---------//

    private string GetQuestionText(string topic)
    {
        DialogueEntry entry = System.Array.Find(dialogueDatabase.entries, e => e.topicKey == currentTopic);
        return entry != null ? entry.lessonQuestion : "";
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
}
