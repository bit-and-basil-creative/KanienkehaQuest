using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueEntry", menuName = "Dialogue/Entry")]
public class DialogueEntry : ScriptableObject
{
    public string topicKey; // e.g., "Ch1_Hello_Intro"

    [TextArea(4, 10)]
    public string[] introLines; // used if it's an intro section

    [TextArea(3, 5)]
    public string lessonQuestion; // used if it's a lesson

    public string[] responseOptions; // e.g., "<link=correct>Shé:kon!</link>" etc.

    public string mohawkWord;

    public string englishWord;

    public AudioClip wordAudio;

    public bool triggersAchievement;

    public string achievementId;
}
