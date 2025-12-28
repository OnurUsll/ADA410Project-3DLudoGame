using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Oyun Ayarları")]
    public Dice dice;
    public List<Oyuncu> oyuncular;

    [Header("Durum Bilgisi")]
    [SerializeField] private GameState currentState;
    private int mevcutOyuncuIndexi = 0;
    private Oyuncu mevcutOyuncu;
    private int sonZarSonucu = 0;
    
    private List<Piyon> oynanabilirPiyonlar = new List<Piyon>();
    private Piyon enSonHareketEdenPiyon; 
    private bool isGameFinished = false;

    void Start()
    {
        Time.timeScale = 1.0f;

        // Ayarları Hafızadan Yükle
        for (int i = 0; i < oyuncular.Count; i++)
        {
            if (i < GameSettings.isBotArr.Length)
                oyuncular[i].isBot = GameSettings.isBotArr[i];

            foreach (Piyon p in oyuncular[i].piyonlar)
            {
                p.OnMovementFinished -= HandlePawnMovementFinished;
                p.OnMovementFinished += HandlePawnMovementFinished;
            }
        }

        if (dice != null) dice.OnDiceRolled += HandleDiceRoll;

        if (oyuncular.Count > 0)
        {
            mevcutOyuncu = oyuncular[mevcutOyuncuIndexi];
            SiraKimdeKontrolu(); 
        }
    }

    // === INPUT CONTROLLER HATALARINI DÜZELTEN FONKSİYONLAR ===
    public GameState GetCurrentState() => currentState;

    public void HandlePawnClick(Piyon tiklananPiyon)
    {
        if (mevcutOyuncu.isBot || currentState != GameState.WaitingForPawnMove) return;

        if (mevcutOyuncu.piyonlar.Contains(tiklananPiyon))
        {
            int index = mevcutOyuncu.piyonlar.IndexOf(tiklananPiyon);
            PiyonButonunaBasildi(index);
        }
    }
    // ========================================================

    void SiraKimdeKontrolu()
    {
        StopAllCoroutines();
        if (UIManager.Instance != null)
        {
            Color renk = GetColor(mevcutOyuncu.renk);
            UIManager.Instance.SiraGuncelle(mevcutOyuncu.oyuncuAdi, renk);
            UIManager.Instance.BildirimGoster($"SIRA: {mevcutOyuncu.oyuncuAdi.ToUpper()}");
            UIManager.Instance.PiyonButonlariniSifirla(); 
        }

        if (mevcutOyuncu.isBot)
        {
            currentState = GameState.BotTurn;
            UIManager.Instance?.ZarButonuAktiflik(false);
            StartCoroutine(BotZarAt());
        }
        else
        {
            currentState = GameState.WaitingForRoll;
            UIManager.Instance?.ZarButonuAktiflik(true);
        }
    }

    private Color GetColor(OyuncuRengi r) {
        switch (r) {
            case OyuncuRengi.Kirmizi: return Color.red;
            case OyuncuRengi.Yesil: return Color.green;
            case OyuncuRengi.Sari: return Color.yellow;
            default: return Color.cyan;
        }
    }

    IEnumerator BotZarAt() { yield return new WaitForSeconds(1.0f); dice.Roll(); }
    IEnumerator BotKararVer()
    {
        yield return new WaitForSeconds(1.2f); 
        Piyon secilen = (sonZarSonucu == 6) ? oynanabilirPiyonlar.FirstOrDefault(p => p.KaledeMi) : null;
        if (secilen == null && oynanabilirPiyonlar.Count > 0) secilen = oynanabilirPiyonlar[Random.Range(0, oynanabilirPiyonlar.Count)];
        if (secilen != null) PiyonSecildi(secilen);
    }

    public void RequestDiceRoll()
    {
        if (!isGameFinished && currentState == GameState.WaitingForRoll && !mevcutOyuncu.isBot)
        {
            UIManager.Instance?.ZarButonuAktiflik(false);
            dice.Roll();
        }
    }

    public void PiyonButonunaBasildi(int index)
    {
        if (currentState == GameState.WaitingForPawnMove && !mevcutOyuncu.isBot)
        {
            Piyon p = mevcutOyuncu.piyonlar[index];
            if (oynanabilirPiyonlar.Contains(p)) {
                UIManager.Instance?.PiyonButonlariniSifirla();
                PiyonSecildi(p);
            }
        }
    }

    private void HandleDiceRoll(int res)
    {
        sonZarSonucu = res;
        UIManager.Instance?.ZarSonucunuGoster(res);
        oynanabilirPiyonlar.Clear();

        foreach (Piyon p in mevcutOyuncu.piyonlar)
            if (IsMoveValid(p, res)) oynanabilirPiyonlar.Add(p);

        if (oynanabilirPiyonlar.Count == 0) StartCoroutine(SiraGeciktirici(res == 6));
        else if (mevcutOyuncu.isBot) StartCoroutine(BotKararVer());
        else BekleVePiyonSectir();
    }

    private void BekleVePiyonSectir()
    {
        currentState = GameState.WaitingForPawnMove;
        List<int> aktifler = new List<int>();
        for (int i = 0; i < mevcutOyuncu.piyonlar.Count; i++)
            if (oynanabilirPiyonlar.Contains(mevcutOyuncu.piyonlar[i])) aktifler.Add(i);
        UIManager.Instance?.PiyonButonlariniAktifEt(aktifler);
    }

    private void PiyonSecildi(Piyon p) { currentState = GameState.PawnMoving; enSonHareketEdenPiyon = p; if (p.KaledeMi) p.OyunaGir(); else p.HareketiBaslat(sonZarSonucu); }

    private void HandlePawnMovementFinished()
    {
        if (enSonHareketEdenPiyon != null) {
            CheckIfPawnFinished(enSonHareketEdenPiyon);
            if (!enSonHareketEdenPiyon.KaledeMi) CheckForPawnCapture(enSonHareketEdenPiyon);
        }
        if (CheckForWinCondition(mevcutOyuncu)) { EndGame(mevcutOyuncu); return; }

        if (sonZarSonucu == 6) {
            currentState = GameState.WaitingForRoll;
            SiraKimdeKontrolu();
        } else SiradakiOyuncuyaGec();
    }

    private void SiradakiOyuncuyaGec() { mevcutOyuncuIndexi = (mevcutOyuncuIndexi + 1) % oyuncular.Count; mevcutOyuncu = oyuncular[mevcutOyuncuIndexi]; SiraKimdeKontrolu(); }
    private IEnumerator SiraGeciktirici(bool t) { yield return new WaitForSeconds(1.0f); if (t) SiraKimdeKontrolu(); else SiradakiOyuncuyaGec(); }

    private void CheckForPawnCapture(Piyon p)
    {
        if (p.EvYolundaMi) return;
        foreach (Oyuncu o in oyuncular) {
            foreach (Piyon d in o.piyonlar) {
                if (d == p || d.piyonRengi == p.piyonRengi || d.KaledeMi || d.EvYolundaMi || d.BitirdiMi) continue;
                if (Vector3.Distance(p.transform.position, d.transform.position) < 0.5f) {
                    UIManager.Instance?.BildirimGoster("RAKİP KIRILDI!");
                    d.EvineDon(o.kaleNoktalari[o.piyonlar.IndexOf(d)]);
                }
            }
        }
    }

    private bool IsMoveValid(Piyon p, int r)
    {
        if (p.BitirdiMi) return false;
        if (p.KaledeMi) return r == 6 && !IsOccupied(p.AnaYol[p.baslangicNoktasiIndexi], p.piyonRengi);
        int idx = p.MevcutYolIndexi; bool ev = p.EvYolundaMi;
        for (int i = 0; i < r; i++) {
            if (ev) { if (++idx >= p.EvYolu.Count) return false; }
            else { if (idx == p.eveGirisNoktasiIndexi) { ev = true; idx = 0; } else { idx = (idx + 1) % p.AnaYol.Count; } }
        }
        return !IsOccupied(ev ? p.EvYolu[idx] : p.AnaYol[idx], p.piyonRengi);
    }

    private bool IsOccupied(Transform t, OyuncuRengi c) => oyuncular.First(o => o.renk == c).piyonlar.Any(p => !p.KaledeMi && Vector3.Distance(p.transform.position, t.position) < 0.5f);
    private void CheckIfPawnFinished(Piyon p) { if (p.EvYolundaMi && p.MevcutYolIndexi == p.EvYolu.Count - 1) p.Bitir(); }
    private bool CheckForWinCondition(Oyuncu o) => o.piyonlar.All(p => p.BitirdiMi);
    private void EndGame(Oyuncu k) { UIManager.Instance?.OyunuBitir(k.oyuncuAdi); isGameFinished = true; }

    void OnDestroy() { 
        if (dice != null) dice.OnDiceRolled -= HandleDiceRoll;
        foreach (Oyuncu o in oyuncular) foreach (Piyon p in o.piyonlar) p.OnMovementFinished -= HandlePawnMovementFinished;
    }
}