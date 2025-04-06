using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameSaver gameSaver;
    [SerializeField] private GameObject saveConfirmationPanel;
    [SerializeField] private float confirmationDisplayTime = 2f;

    private bool isPaused = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void SaveGame()
    {
        gameSaver.SaveGame();
        StartCoroutine(ShowSaveConfirmation());
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator ShowSaveConfirmation()
    {
        saveConfirmationPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(confirmationDisplayTime);
        saveConfirmationPanel.SetActive(false);
    }
}
