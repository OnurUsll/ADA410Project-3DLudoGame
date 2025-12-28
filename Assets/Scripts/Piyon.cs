using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piyon : MonoBehaviour
{
    [Header("Piyon Ayarları")]
    public OyuncuRengi piyonRengi; 
    public int baslangicNoktasiIndexi; 
    public int eveGirisNoktasiIndexi; 

    [Header("Zemin ve Animasyon")]
    public LayerMask zeminKatmani;
    public Animator piyonAnimator;
    public float hareketHizi = 5.0f;
    public event System.Action OnMovementFinished;

    [Header("Görsel Efekt")]
    public GameObject highlightVisual; 
    private Coroutine blinkCoroutine; 

    private PathManager pathManager;
    private List<Transform> anaYol; 
    private List<Transform> evYolu; 

    private int mevcutYolIndexi;
    private bool hareketHalinde = false;
    private bool evYolunda = false; 
    private bool _kalede = true; 
    private bool _bitirdi = false; 

    public bool KaledeMi => _kalede;
    public bool BitirdiMi => _bitirdi;
    public bool EvYolundaMi => evYolunda;
    public int MevcutYolIndexi => mevcutYolIndexi;
    public List<Transform> AnaYol => anaYol;
    public List<Transform> EvYolu => evYolu;

    void Start()
    {
        pathManager = FindFirstObjectByType<PathManager>();
        if (piyonAnimator == null) piyonAnimator = GetComponentInChildren<Animator>();
        anaYol = pathManager.genelYol; 
        switch (piyonRengi)
        {
            case OyuncuRengi.Kirmizi: evYolu = pathManager.kirmiziEvYolu; break;
            case OyuncuRengi.Yesil: evYolu = pathManager.yesilEvYolu; break;
            case OyuncuRengi.Sari: evYolu = pathManager.sariEvYolu; break;
            case OyuncuRengi.Mavi: evYolu = pathManager.maviEvYolu; break;
        }
    }

    public void OyunaGir()
    {
        Debug.Log($"[Piyon] {name} kaleden çıkıyor...");
        mevcutYolIndexi = baslangicNoktasiIndexi;
        _kalede = false;
        evYolunda = false;
        
        if (anaYol != null && anaYol.Count > mevcutYolIndexi)
            ZemineOturt(anaYol[mevcutYolIndexi].position);

        // Kilitlenmeyi önlemek için haberi bir kare sonra gönder
        StartCoroutine(HaberVerGecikmeli());
    }

    private IEnumerator HaberVerGecikmeli()
    {
        yield return new WaitForEndOfFrame();
        OnMovementFinished?.Invoke();
    }

    public void HareketiBaslat(int adimSayisi)
    {
        if (hareketHalinde || _kalede || _bitirdi)
        {
            OnMovementFinished?.Invoke();
            return;
        }
        if (piyonAnimator != null) piyonAnimator.SetBool("isMoving", true);
        StartCoroutine(AdimAdimIlerle(adimSayisi));
    }

    private IEnumerator AdimAdimIlerle(int adimSayisi)
    {
        hareketHalinde = true;
        for (int i = 0; i < adimSayisi; i++)
        {
            if (evYolunda)
            {
                if (mevcutYolIndexi < evYolu.Count - 1) mevcutYolIndexi++;
                yield return StartCoroutine(TekNoktaIlerle(evYolu[mevcutYolIndexi]));
            }
            else
            {
                if (mevcutYolIndexi == eveGirisNoktasiIndexi)
                {
                    evYolunda = true;
                    mevcutYolIndexi = 0;
                    yield return StartCoroutine(TekNoktaIlerle(evYolu[mevcutYolIndexi]));
                }
                else
                {
                    mevcutYolIndexi++;
                    if (mevcutYolIndexi >= anaYol.Count) mevcutYolIndexi = 0;
                    yield return StartCoroutine(TekNoktaIlerle(anaYol[mevcutYolIndexi]));
                }
            }
        } 
        hareketHalinde = false;
        if (piyonAnimator != null) piyonAnimator.SetBool("isMoving", false);
        OnMovementFinished?.Invoke();
    }

    private IEnumerator TekNoktaIlerle(Transform hedefNokta)
    {
        Vector3 hedef = hedefNokta.position;
        while (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(hedef.x, 0, hedef.z)) > 0.05f)
        {
            Vector3 yeniPozisyon = Vector3.MoveTowards(transform.position, hedef, hareketHizi * Time.deltaTime);
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(yeniPozisyon.x, 100f, yeniPozisyon.z), Vector3.down, out hit, 200f, zeminKatmani))
                yeniPozisyon.y = hit.point.y; 
            transform.position = yeniPozisyon;
            transform.LookAt(new Vector3(hedef.x, transform.position.y, hedef.z));
            yield return null; 
        }
        ZemineOturt(hedef);
    }

    private void ZemineOturt(Vector3 hedefPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(hedefPos.x, 100f, hedefPos.z), Vector3.down, out hit, 200f, zeminKatmani))
            transform.position = hit.point;
        else
            transform.position = hedefPos;
    }

    public void SetHighlight(bool isActive)
    {
        if (highlightVisual == null) return;
        if (isActive) { if (blinkCoroutine == null) blinkCoroutine = StartCoroutine(BlinkEffect()); }
        else { if (blinkCoroutine != null) { StopCoroutine(blinkCoroutine); blinkCoroutine = null; } highlightVisual.SetActive(false); }
    }

    private IEnumerator BlinkEffect() { while (true) { highlightVisual.SetActive(true); yield return new WaitForSeconds(0.3f); highlightVisual.SetActive(false); yield return new WaitForSeconds(0.3f); } }

    public void EvineDon(Transform kalePozisyonu)
    {
        transform.position = kalePozisyonu.position;
        if (piyonAnimator != null) piyonAnimator.SetBool("isMoving", false);
        _kalede = true; evYolunda = false; mevcutYolIndexi = 0; hareketHalinde = false; _bitirdi = false;
    }

    public void Bitir() { _bitirdi = true; if (piyonAnimator != null) piyonAnimator.SetBool("isMoving", false); }
}