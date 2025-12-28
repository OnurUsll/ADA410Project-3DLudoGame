using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Sahne yönetimi için şart

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Piyon Kontrol Butonları")]
    public Button[] piyonButonlari; // 4 adet butonu buraya sürükleyeceğiz

    [Header("HUD Elemanları")]
    public TextMeshProUGUI siraText;     
    public Image siraRengiImage; 
    public TextMeshProUGUI bildirimText;  
    public Button zarButonu;
    public Image zarResmi;
    
    [Header("Paneller")]
    public GameObject pauseMenuPanel;
    public GameObject winPanel;        // EKSİK OLAN BUYDU
    public TextMeshProUGUI winnerText; // EKSİK OLAN BUYDU

    [Header("Zar Resimleri")]
    public Sprite[] zarSprites; 

    [Header("Ayarlar")]
    public Color normalButonRenk = Color.white;
    public Color yanipSonenRenk = Color.yellow;

    private Dictionary<int, Coroutine> blinkCoroutines = new Dictionary<int, Coroutine>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BildirimGoster("OYUN BAŞLIYOR...");
        PiyonButonlariniSifirla(); 
        
        // Panelleri gizle
        if(pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if(winPanel != null) winPanel.SetActive(false);
    }

    // --- OYUN SONU (WIN SCREEN) ---
    // GameManager'ın aradığı fonksiyon bu:
    public void OyunuBitir(string kazananIsim)
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if(winnerText != null) winnerText.text = $"{kazananIsim.ToUpper()} KAZANDI!";
        }
    }

    public void OyunuYenidenBaslat()
    {
        // Mevcut sahneyi yeniden yükler
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MenuyeDon()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MenuScene"); // Menü sahnesinin adı
    }

    public void OyunuDuraklat()
    {
        if(pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OyunuDevamEttir()
    {
        if(pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // --- PİYON BUTONLARI SİSTEMİ ---

    public void PiyonButonlariniAktifEt(List<int> aktifIndexler)
    {
        PiyonButonlariniSifirla();

        foreach (int index in aktifIndexler)
        {
            if (index >= 0 && index < piyonButonlari.Length)
            {
                Button btn = piyonButonlari[index];
                btn.interactable = true; 
                
                if (!blinkCoroutines.ContainsKey(index))
                {
                    blinkCoroutines[index] = StartCoroutine(ButonYanipSon(btn.image));
                }
            }
        }
    }

    public void PiyonButonlariniSifirla()
    {
        foreach (var kvp in blinkCoroutines)
        {
            if (kvp.Value != null) StopCoroutine(kvp.Value);
        }
        blinkCoroutines.Clear();

        foreach (Button btn in piyonButonlari)
        {
            btn.interactable = false;
            btn.image.color = normalButonRenk;
        }
    }

    private IEnumerator ButonYanipSon(Image btnImage)
    {
        while (true)
        {
            btnImage.color = yanipSonenRenk;
            yield return new WaitForSeconds(0.3f);
            btnImage.color = normalButonRenk;
            yield return new WaitForSeconds(0.3f);
        }
    }

    // --- GENEL UI ---

    public void BildirimGoster(string mesaj)
    {
        if (bildirimText != null) bildirimText.text = mesaj;
        // Debug.Log("UI: " + mesaj); // Konsolu kirletmesin diye kapattım
    }

    public void SiraGuncelle(string oyuncuAdi, Color renk)
    {
        // 1. Yazıyı güncelle ve Beyaz yap (Okunaklı olsun diye)
        if (siraText != null) 
        {
            siraText.text = $"SIRA: {oyuncuAdi.ToUpper()}";
            siraText.color = Color.white; // Yazı hep beyaz kalsın
        }

        // 2. Resmi güncelle (Asıl renk burada görünsün)
        // Eğer Image atamadıysan hata vermesin diye kontrol ediyoruz
        if (siraRengiImage != null)
        {
            siraRengiImage.color = renk; 
        }
        // Eğer Image yoksa ama panelin kendisi (SiraPaneli) renklensin istiyorsan:
        // else if (siraText != null && siraText.transform.parent != null)
        // {
        //     siraText.transform.parent.GetComponent<Image>().color = renk;
        // }
    }

    public void ZarSonucunuGoster(int zarDegeri)
    {
        if (zarSprites != null && zarSprites.Length >= 6)
        {
            zarResmi.sprite = zarSprites[zarDegeri - 1];
        }
        BildirimGoster($"ZAR {zarDegeri} GELDİ!");
    }

    public void ZarButonuAktiflik(bool acikMi)
    {
        if (zarButonu != null) zarButonu.interactable = acikMi;
    }
}