using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; //reference to audio source component

    [Header("Chapter Settings")]
    [SerializeField] private TextMeshProUGUI chapterNumber;
    [SerializeField] private TextMeshProUGUI chapterTitle;
    [SerializeField] private Image chapterBackground;

    public static GameManager instance;

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

    private void Start()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
