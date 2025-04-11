using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicVolumeParam = "MusicVolume";

    public void NewGame()
    {
        StartCoroutine(FadeOutAndStart(true));
    }

    public void LoadGame()
    {
        StartCoroutine(FadeOutAndStart(false));
    }

    public void OpenOptions()
    {
        optionsUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    private IEnumerator FadeOutAndStart(bool isNewGame)
    {
        
        float t = 0f;

        float startVolume;
        audioMixer.GetFloat(musicVolumeParam, out startVolume);
        startVolume = Mathf.Clamp(startVolume, -80f, 0f);
        float endVolume = -80f;

        
        while (t < fadeDuration)
        {
            //fade out main menu
            mainMenuCanvasGroup.alpha = 1 - (t / fadeDuration);

            //fade music volume
            float musicFadeDuration = fadeDuration * 4.0f;
            float volume = Mathf.Lerp(startVolume, endVolume, (t / musicFadeDuration));
            audioMixer.SetFloat(musicVolumeParam, volume);

            t += Time.deltaTime;
            yield return null;
        }

        mainMenuCanvasGroup.alpha = 0f;
        mainMenuCanvasGroup.blocksRaycasts = false;
        audioMixer.SetFloat(musicVolumeParam, endVolume);

        //pause before scene load
        yield return new WaitForSeconds(0.3f);

        //check if new or saved game
        if (isNewGame)
        {
            PlayerPrefs.DeleteKey("BeadCount");
            PlayerPrefs.DeleteKey("LearnedWords");
            PlayerPrefs.DeleteKey("ChapterProgress");
            PlayerPrefs.DeleteKey("CurrentLesson");
            PlayerPrefs.DeleteKey("UnlockedAchievements");
            PlayerPrefs.SetInt("ShouldLoadFromSave", 0);
        }
        else
        {
            PlayerPrefs.SetInt("ShouldLoadFromSave", 1);
        }

        PlayerPrefs.Save();

        //load the game scene
        SceneManager.LoadScene("Game");
    }
}