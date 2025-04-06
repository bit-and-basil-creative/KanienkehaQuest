using System.Collections.Generic;
using UnityEngine;

public class WordBasketManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject wordEntryPrefab;
    [SerializeField] private DialogueDatabase dialogueDatabase;

    private List<DialogueEntry> learnedWords = new List<DialogueEntry>(); //to store learned words

    //method to add a new word to the basket and create the UI objects for it
    public void AddWord(DialogueEntry entry)
    {
        //check if the word is already in the list
        if (!learnedWords.Contains(entry))
        {
            //if not, add it to the list
            learnedWords.Add(entry);

            //instantiate UI
            GameObject newEntry = Instantiate(wordEntryPrefab, contentParent);

            //get the script that handles displaying the word info
            WordEntryUI entryUI = newEntry.GetComponent<WordEntryUI>();

            //set up the UI with english word, mohawk word and audio button/cip
            entryUI.Setup(entry.englishWord, entry.mohawkWord, entry.wordAudio);
        }
    }

    //remove all UI elements from the word basket and clear the list of learned words
    public void ClearBasket()
    {
        //loop through and destroy all child UI elements in the parent container
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        //clear the list of learned words
        learnedWords.Clear();
    }

    //returns a list of the topic keys for learned words
    public List<string> GetLearnedKeys()
    {
        List<string> keys = new List<string>();

        //loop through each learned word and add its topic key to the list
        foreach (DialogueEntry entry in learnedWords)
        {
            keys.Add(entry.topicKey);
        }

        return keys; //return the list of keys for saving purposes
    }


    //rebuild the word basket from a list of topic keys for loading purposes
    public void LoadFromKeys(List<string> keys)
    {
        ClearBasket(); //clear the existing UI and word list first

        //look up each word's topic key in the dialogue database and add it back to the basket
        foreach (string key in keys)
        {
            //find the dialogue entry using the topic key
            DialogueEntry entry = System.Array.Find(dialogueDatabase.entries, e => e.topicKey == key);

            if (entry != null)
            {
                //add the word and rebuild the UI entry for it
                AddWord(entry);
            }
        }
    }
}
