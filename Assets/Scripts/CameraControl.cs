using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Zoom Ayarları")]
    public float zoomHizi = 15f;
    public float minZoomY = 5f;
    public float maxZoomY = 40f;

    [Header("Gezinme (Pan) Ayarları")]
    public float panHizi = 30f;

    [Header("Rotasyon Ayarları")]
    public float rotasyonHizi = 100f;

    private Vector3 sonMousePozisyonu;

    void Update()
    {
        HandleZoom();
        HandlePan();
        HandleRotation();
    }

    // 1. ZOOM: Scroll ile ileri-geri (Bakış yönünde)
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 hareket = transform.forward * scroll * zoomHizi * 100f * Time.deltaTime;
            Vector3 yeniPozisyon = transform.position + hareket;

            // Yüksekliği sınırla (Yerin içine girmesin veya çok uzaklaşmasın)
            yeniPozisyon.y = Mathf.Clamp(yeniPozisyon.y, minZoomY, maxZoomY);
            transform.position = yeniPozisyon;
        }
    }

    // 2. PAN: Mouse tekerleğine basılı tutunca (Orta Tuş) kaydırma
    void HandlePan()
    {
        if (Input.GetMouseButtonDown(2))
        {
            sonMousePozisyonu = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - sonMousePozisyonu;
            
            // Kameranın baktığı yöne göre sağa ve yukarı hareket hesapla
            Vector3 hareket = transform.right * (-delta.x) + transform.up * (-delta.y);
            transform.position += hareket * panHizi * 0.01f * Time.deltaTime;
            
            sonMousePozisyonu = Input.mousePosition;
        }
    }

    // 3. ROTASYON: Sağ tık basılıyken dönme (Unity Editörü gibi)
    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Sağ Tık
        {
            float fareX = Input.GetAxis("Mouse X") * rotasyonHizi * Time.deltaTime;
            float fareY = -Input.GetAxis("Mouse Y") * rotasyonHizi * Time.deltaTime;

            // Yatayda (Y ekseni) dünya eksenine göre dön
            transform.Rotate(Vector3.up, fareX, Space.World);
            
            // Dikeyde (X ekseni) kendi eksenine göre dön
            transform.Rotate(Vector3.right, fareY, Space.Self);

            // Kameranın takla atmasını engelle (Z eksenini sıfırla)
            Vector3 euler = transform.rotation.eulerAngles;
            euler.z = 0;
            transform.rotation = Quaternion.Euler(euler);
        }
    }
}