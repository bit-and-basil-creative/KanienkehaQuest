using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

public class GameSaver : MonoBehaviour
{

    [SerializeField] private BeadTracker beadTracker;
    [SerializeField] private WordBasketManager wordBasketManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueDatabase dialogueDatabase;

    public void SaveGame()
    {
        // Save bead count
        int currentBeads = FindObjectOfType<BeadTracker>().GetBeadCount(); // Update if you're storing bead count elsewhere
        PlayerPrefs.SetInt("BeadCount", currentBeads);

        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        PlayerPrefs.SetInt("LessonIndex", dialogueManager.GetCurrentLessonIndex());

        SaveWordBasket();

        PlayerPrefs.Save();

        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        LoadWordBasket();
        PlayerPrefs.SetInt("ShouldLoadFromSave", 1);
        SceneManager.LoadScene("Game");
    }

    public void SaveLearnedWords()
    {
        string savedKeys = string.Join(",", FindObjectOfType<WordBasketManager>().GetLearnedKeys());
        PlayerPrefs.SetString("LearnedWords", savedKeys);
    }

    public void LoadLearnedWords()
    {
        string savedKeys = PlayerPrefs.GetString("LearnedWords", "");
        if (!string.IsNullOrEmpty(savedKeys))
        {
            string[] keys = savedKeys.Split(',');

            foreach (string key in keys)
            {
                DialogueEntry entry = dialogueDatabase.entries.FirstOrDefault(e => e.topicKey == key);

                if (entry != null)
                {
                    wordBasketManager.AddWord(entry);
                }
            }
        }
    }

    public void SaveWordBasket()
    {
        WordBasketManager basket = FindObjectOfType<WordBasketManager>();
        List<string> wordKeys = basket.GetLearnedKeys();
        string joined = string.Join(",", wordKeys);
        PlayerPrefs.SetString("LearnedWords", joined);
    }

    public void LoadWordBasket()
    {
        WordBasketManager basket = FindObjectOfType<WordBasketManager>();
        string saved = PlayerPrefs.GetString("LearnedWords", "");

        if (!string.IsNullOrEmpty(saved))
        {
            List<string> keys = new List<string>(saved.Split(','));
            basket.LoadFromKeys(keys);
        }
    }

}
