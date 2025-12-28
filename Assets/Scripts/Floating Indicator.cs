
using UnityEngine;

public class FloatingIndicator : MonoBehaviour
{
    public float donmeHizi = 50f;
    public float yuzmeGenisligi = 0.1f;
    public float yuzmeHizi = 2f;

    Vector3 baslangicPozisyonu;

    void Start()
    {
        baslangicPozisyonu = transform.localPosition;
    }

    void Update()
    {
        // Kendi etrafında döndür
        transform.Rotate(Vector3.up, donmeHizi * Time.deltaTime, Space.World);

        // Aşağı yukarı yüzdür (Sinüs dalgası kullanarak)
        float yeniY = baslangicPozisyonu.y + Mathf.Sin(Time.time * yuzmeHizi) * yuzmeGenisligi;
        transform.localPosition = new Vector3(baslangicPozisyonu.x, yeniY, baslangicPozisyonu.z);
    }
}