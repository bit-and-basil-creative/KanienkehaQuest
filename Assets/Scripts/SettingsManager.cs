using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private float masterVolume;
    private float sfxVolume;
    private float musicVolume;

    void Start()
    {
        Debug.Log("[SettingsManager] Start called.");
        LoadSettings();
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        masterVolume = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixer.SetFloat("MasterVolume", masterVolume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        sfxVolume = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixer.SetFloat("SFXVolume", sfxVolume);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        musicVolume = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixer.SetFloat("MusicVolume", musicVolume);
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        float defaultVolume = 0.7f;

        float master = PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : defaultVolume;
        float sfx = PlayerPrefs.HasKey("SFXVolume") ? PlayerPrefs.GetFloat("SFXVolume") : defaultVolume;
        float music = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : defaultVolume;

        Debug.Log($"[SettingsManager] Loaded - Master: {master}, SFX: {sfx}, Music: {music}");

        //set slider values
        masterSlider.value = master;
        sfxSlider.value = sfx;
        musicSlider.value = music;

        //apply volumes to AudioMixer
        SetMasterVolume();
        SetSFXVolume();
        SetMusicVolume();
    }
}
