using UnityEngine;
using TMPro;

public class BeadTracker : MonoBehaviour
{
    public static BeadTracker instance; // Singleton instance
    public TextMeshProUGUI beadText;
    private int beadsCollected = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (beadText == null)
        {
            Debug.LogError("BeadTracker: beadText is not assigned in the Inspector!");
            return;
        }

        UpdateBeadUI();
    }

    public void AddBead()
    {
        beadsCollected++;
        UpdateBeadUI();
    }

    private void UpdateBeadUI()
    {
        if (beadText != null)
        {
            beadText.text = "Beads: " + beadsCollected;
        }
        else
        {
            Debug.LogError("BeadTracker: beadText is null! Assign it in the Unity Inspector.");
        }
    }

}
