using System.Collections.Generic;
using UnityEngine;

public class WordBasketManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent; // Content object inside the scroll view
    [SerializeField] private GameObject wordEntryPrefab; // Your prefab that displays each word

    private List<DialogueEntry> learnedWords = new List<DialogueEntry>();

    public void AddWord(DialogueEntry entry)
    {
        // Prevent duplicates
        if (!learnedWords.Contains(entry))
        {
            learnedWords.Add(entry);

            // Instantiate UI
            GameObject newEntry = Instantiate(wordEntryPrefab, contentParent);
            WordEntryUI entryUI = newEntry.GetComponent<WordEntryUI>();

            entryUI.Setup(entry.englishWord, entry.mohawkWord, entry.wordAudio);
        }
    }

    public void ClearBasket()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        learnedWords.Clear();
    }
}
