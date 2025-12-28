using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 1. Slider'ları dinlemeye başla
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);

        // 2. Oyun açıldığında slider'lar en sonda (ses açık) başlasın
        musicSlider.value = 1f;
        sfxSlider.value = 1f;
    }

    // Slider oynadıkça burası çalışır
    void OnMusicChanged(float val)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(val);
    }

    void OnSFXChanged(float val)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(val);
    }
}