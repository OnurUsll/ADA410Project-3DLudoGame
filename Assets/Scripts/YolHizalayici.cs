using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // Ctrl+Z yapabilmek için gerekli
#endif

public class YolHizalayici : MonoBehaviour
{
    [ContextMenu("Fizik Kullanarak Zemine Dusur")]
    void Hizala()
    {
        // Tüm çocukları ve torunları bul
        Transform[] tumObjeler = GetComponentsInChildren<Transform>();
        int duzenlenenSayisi = 0;

        foreach (Transform obje in tumObjeler)
        {
            // 1. KURAL: Ana objeyi (Scriptin takılı olduğu) hareket ettirme
            if (obje == this.transform) continue;

            // 2. KURAL: Klasörleri hareket ettirme (Sadece çocuğu olmayan uç noktalar)
            if (obje.childCount > 0) continue;

            // Ctrl+Z ile geri alabilmek için kaydet (Sadece Editörde çalışır)
            #if UNITY_EDITOR
            Undo.RecordObject(obje, "Zemine Hizalama");
            #endif

            // === FİZİKSEL IŞINLAMA (RAYCAST) ===
            
            // Noktanın X ve Z'sini al, Y'sini gökyüzüne (1000 metre yukarı) çek
            Vector3 gokyuzuPozisyonu = new Vector3(obje.position.x, 500f, obje.position.z);

            // Aşağı doğru lazer sık
            RaycastHit hit;
            // Terrain'in collider'ı olduğu için fizik ışını ona çarpacaktır.
            if (Physics.Raycast(gokyuzuPozisyonu, Vector3.down, out hit, 1000f))
            {
                // Çarptığı yerin çok azıcık üstüne koy (gömülmesin diye)
                obje.position = hit.point + new Vector3(0, 0.05f, 0);
                duzenlenenSayisi++;
            }
        }

        Debug.Log($"İşlem Tamam! {duzenlenenSayisi} nokta gökten zemine düşürüldü.");
    }
}