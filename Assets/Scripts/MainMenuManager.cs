using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject settingsPanel;
    public GameObject tutorialPanel;
    public GameObject creditsPanel;
    public GameObject teamSelectPanel; // YENİ: Takım Seçim Paneli (Inspector'dan ata)

    // --- OYUN BAŞLATMA VE TAKIM SEÇİMİ ---

    // 1. Ana Menüdeki "OYNA" butonuna bunu bağla
    public void OpenTeamSelection()
    {
        if(teamSelectPanel != null) teamSelectPanel.SetActive(true);
    }

    // 2. Takım Seçim Panelindeki "GERİ" butonuna bunu bağla
    public void CloseTeamSelection()
    {
        if(teamSelectPanel != null) teamSelectPanel.SetActive(false);
    }

    // 3. Kırmızı, Yeşil, Sarı ve Mavi butonlarına bunu bağla
    // Parametreler: 0=Kırmızı, 1=Yeşil, 2=Sarı, 3=Mavi
    public void SelectTeamAndStart(int selectedTeamIndex)
    {
        // Önce GameSettings dizisini sıfırlayalım (Güvenlik)
        if (GameSettings.isBotArr == null || GameSettings.isBotArr.Length != 4)
        {
            GameSettings.isBotArr = new bool[] { true, true, true, true };
        }

        // Adım 1: Herkesi BOT (true) yap
        for (int i = 0; i < 4; i++)
        {
            GameSettings.isBotArr[i] = true;
        }

        // Adım 2: Seçilen takımı İNSAN (false) yap
        if (selectedTeamIndex >= 0 && selectedTeamIndex < 4)
        {
            GameSettings.isBotArr[selectedTeamIndex] = false;
            Debug.Log($"Takım {selectedTeamIndex} seçildi. Diğerleri Bot olarak ayarlandı.");
        }

        // Adım 3: Oyunu Yükle
        // Not: Build Settings'de oyun sahnenin adının "GameScene" olduğundan emin ol!
        SceneManager.LoadScene("GameScene");
    }

    // --- DİĞER BUTON FONKSİYONLARI ---

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkıldı!"); 
        Application.Quit();
    }
}