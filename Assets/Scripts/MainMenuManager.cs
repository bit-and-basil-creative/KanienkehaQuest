using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsUI;

    public void NewGame()
    {
        //delete any progress-related data
        PlayerPrefs.DeleteKey("BeadCount");
        PlayerPrefs.DeleteKey("LearnedWords");
        PlayerPrefs.DeleteKey("ChapterProgress");
        PlayerPrefs.DeleteKey("CurrentLesson");
        PlayerPrefs.DeleteKey("UnlockedAchievements");

        PlayerPrefs.Save();

        SceneManager.LoadScene("Game");
    }

    public void LoadGame()
    {
        string savedScene = PlayerPrefs.GetString("SavedScene", "Game");
        SceneManager.LoadScene(savedScene);
    }

    public void OpenOptions()
    {
        optionsUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}