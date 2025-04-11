using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct ChapterInfo
{
    public string id;
    public string number;
    public string title;
    public Sprite background;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; //reference to audio source component

    [Header("Chapter Settings")]
    [SerializeField] private TextMeshProUGUI chapterNumber;
    [SerializeField] private TextMeshProUGUI chapterTitle;
    [SerializeField] private UnityEngine.UI.Image chapterBackground;
    [SerializeField] private List<ChapterInfo> chapters;

    [Header("Transition Screen Settings")]
    [SerializeField] private GameObject transitionScreen;
    [SerializeField] private TextMeshProUGUI transitionText;
    [SerializeField] private CanvasGroup transitionCanvasGroup;

    [Header("Fade In Settings")]
    [SerializeField] private CanvasGroup sceneFadeCanvas;
    [SerializeField] private float fadeInDuration = 1.5f;

    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

        StartCoroutine(FadeInScene());
    }

    public void SetChapter(string chapterId)
    {
        ChapterInfo info = chapters.Find(c => c.id == chapterId);

        chapterNumber.text = info.number;
        chapterTitle.text = info.title;
        chapterBackground.sprite = info.background;
    }

    public void ShowTransitionScreen(string chapterId, System.Action onComplete = null)
    {
        ChapterInfo info = chapters.Find(c => c.id == chapterId);
        transitionText.text = $"Loading \n {info.number} \n {info.title}";
        StartCoroutine(HandleTransition(onComplete));
    }

    private IEnumerator HandleTransition(System.Action onComplete)
    {
        transitionScreen.SetActive(true);
        transitionCanvasGroup.alpha = 0f;
        transitionCanvasGroup.blocksRaycasts = true;

        float duration = 2f;

        //fade in
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            transitionCanvasGroup.alpha = t / duration;
            yield return null;
        }
        transitionCanvasGroup.alpha = 1f;

        //wait
        yield return new WaitForSeconds(1f);

        onComplete?.Invoke();

        yield return new WaitForSeconds(0.5f);

        //fade out
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            transitionCanvasGroup.alpha = 1f - (t / duration);
            yield return null;
        }
        transitionCanvasGroup.alpha = 0f;

        //hide it again
        transitionCanvasGroup.blocksRaycasts = false;
        transitionScreen.SetActive(false);
    }

    private IEnumerator FadeInScene()
    {
        float t = 0f;
        sceneFadeCanvas.alpha = 0f;

        while (t < fadeInDuration)
        {
            sceneFadeCanvas.alpha = 0f + (t / fadeInDuration);
            t += Time.deltaTime;
            yield return null;
        }

        sceneFadeCanvas.alpha = 1f;
    }
}
