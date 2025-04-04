using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI englishText;
    [SerializeField] private TextMeshProUGUI mohawkText;
    [SerializeField] private Button playButton;

    [SerializeField] private AudioClip clip;

    public void Setup(string english, string mohawk, AudioClip audio)
    {
        englishText.text = english;
        mohawkText.text = mohawk;
        clip = audio;

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(PlayAudio);
    }

    void PlayAudio()
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        }
    }
}
