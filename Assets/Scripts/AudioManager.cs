using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Ses Kaynakları")]
    public AudioSource musicSource; // Müzik
    public AudioSource sfxSource;   // Efektler

    void Awake()
    {
        // Singleton: Her yerden ulaşabilmek için
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Slider bu fonksiyonu çağıracak (0.0 ile 1.0 arası)
    public void SetMusicVolume(float volume)
    {
        if(musicSource != null) musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if(sfxSource != null) sfxSource.volume = volume;

        
    }

    

    [Header("Ses Dosyaları")]
    public AudioClip clickSound; // Tıklama sesi dosyası buraya sürüklenecek

    // Butona basınca bu çağrılacak
    public void PlayClick()
    {
        if (sfxSource != null && clickSound != null)
        {
            // PlayOneShot: Kısa efektler için en iyisidir, üst üste çalabilir
            sfxSource.PlayOneShot(clickSound); 
        }
    }
}
