using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int beadsCollected = 0;
    private int requiredBeads = 12;

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

    public void AddBead()
    {
        beadsCollected++;
        if (beadsCollected >= requiredBeads)
        {
            UnlockNextChapter();
        }
    }

    private void UnlockNextChapter()
    {
        Debug.Log("All beads collected! Unlocking next chapter...");
    }

    public int GetBeadCount()
    {
        return beadsCollected;
    }
}
