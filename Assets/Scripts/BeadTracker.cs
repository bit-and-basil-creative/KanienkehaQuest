using UnityEngine;
using TMPro;

public class BeadTracker : MonoBehaviour
{
    public static BeadTracker instance; // Singleton instance
    [SerializeField] private TextMeshProUGUI beadText;
    private int beadCount = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
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
        beadCount++;
        UpdateBeadUI();
    }

    private void UpdateBeadUI()
    {
        if (beadText != null)
        {
            beadText.text = beadCount.ToString();
        }
        else
        {
            Debug.LogError("BeadTracker: beadText is null! Assign it in the Unity Inspector.");
        }
    }

    public int GetBeadCount()
    {
        return beadCount;
    }

    public void SetBeadCount(int count)
    {
        beadCount = count;
        UpdateBeadUI();
    }

    public void AssignBeadText(TextMeshProUGUI newText)
    {
        beadText = newText;
        UpdateBeadUI();
    }

}
