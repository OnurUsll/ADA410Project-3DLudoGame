using UnityEngine;
using UnityEngine.InputSystem; // 'New Input System' için bu gerekli

public class InputController : MonoBehaviour
{
    public GameManager gameManager;
    private Camera mainCamera; // Tıklama için kamerayı saklayacağız

    private Vector2 mousePosition; // Farenin ekran pozisyonunu saklar

    void Start()
    {
        // Kamerayı bir kez bul ve sakla (performans için)
        mainCamera = Camera.main;

        // Player Input'u garantiye al
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.ActivateInput();
        }
    }

    // "OyunKontrolleri"ndeki "ZarAt" eylemi (Boşluk) tetiklendiğinde çalışır
    public void OnZarAt()
    {
        if (gameManager != null)
        {
            gameManager.RequestDiceRoll();
        }
    }

    // === YENİ FONKSİYONLAR ===

    /// <summary>
    /// "OyunKontrolleri"ndeki "MousePosition" eylemi tetiklendiğinde (her fare hareketinde) çalışır
    /// </summary>
    public void OnMousePosition(InputValue value)
    {
        // Farenin 2D ekran koordinatlarını al ve sakla
        mousePosition = value.Get<Vector2>();
    }

    /// <summary>
    /// "OyunKontrolleri"ndeki "Click" eylemi (Sol Tık) tetiklendiğinde çalışır
    /// </summary>
    public void OnClick()
    {
        // 1. GameManager'a şu an piyon seçimi bekleyip beklemediğini sor
        if (gameManager == null || gameManager.GetCurrentState() != GameState.WaitingForPawnMove)
        {
            return;
        }

        // 2. Farenin olduğu yerden 3D dünyaya bir ışın (Ray) gönder
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        // 3. Işın bir şeye çarptı mı?
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // === DEDEKTİF KODU ===
            // Tıklanan objenin ismini konsola yazdırıyoruz.
            // Eğer "Sari", "Kirmizi" gibi klasör objelerinin isimlerini görürsen,
            // o objelerin üzerindeki Collider'ları silmen gerektiğini anlarsın.
            Debug.Log($"RAYCAST ÇARPTI: {hit.collider.gameObject.name}");

            // 4. Çarptığı objenin kendisinde 'Piyon' script'i var mı?
            if (hit.collider.TryGetComponent<Piyon>(out Piyon tiklananPiyon))
            {
                Debug.LogWarning($"--- BAŞARILI: {tiklananPiyon.name} tıklandı! ---");
                gameManager.HandlePawnClick(tiklananPiyon);
            }
            // 5. YENİ ÖZELLİK: Script kendisinde yoksa, BABASINDA (Parent) var mı?
            // (Sen hiyerarşiyi değiştirdiğin için script üst objede veya alt objede kalmış olabilir)
            else
            {
                Piyon piyonInParent = hit.collider.GetComponentInParent<Piyon>();
                if (piyonInParent != null)
                {
                    Debug.LogWarning($"--- BAŞARILI (EBEVEYNDEN BULUNDU): {piyonInParent.name} tıklandı! ---");
                    gameManager.HandlePawnClick(piyonInParent);
                }
                else
                {
                    Debug.LogError($"HATA: '{hit.collider.gameObject.name}' objesine tıklandı ama ne kendisinde ne de ailesinde 'Piyon.cs' bulunamadı!");
                }
            }
        }
        else
        {
            Debug.Log("Boşluğa tıklandı.");
        }
    }
}