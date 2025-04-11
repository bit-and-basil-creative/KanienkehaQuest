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
        //save bead count
        int currentBeads = FindObjectOfType<BeadTracker>().GetBeadCount();
        PlayerPrefs.SetInt("BeadCount", currentBeads);

        //save lesson progress
        PlayerPrefs.SetInt("LessonIndex", dialogueManager.GetCurrentLessonIndex());

        //save word basket
        SaveWordBasket();

        //save preferences
        PlayerPrefs.Save();
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
