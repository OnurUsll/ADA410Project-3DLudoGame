using UnityEngine;

public class Dice : MonoBehaviour
{
    // Bu, zar atıldığında sonucu bildirmek için kullanılacak
    // 'System.Action<int>' -> "int tipinde bir parametre alan bir olay/aksiyon" demektir.
    public event System.Action<int> OnDiceRolled;

    /// <Temsili> Zar atma fonksiyonu.
    /// Dışarıdan (GameManager tarafından) çağrılacak.
    public void Roll()
    {
        // 1 (dahil) ile 7 (hariç) arasında bir tam sayı üret
        int rollResult = Random.Range(1, 7);

        // Konsola sonucu yazdıralım
        Debug.Log("ZAR ATILDI: " + rollResult);

        // Bu zara abone olan (dinleyen) var mı diye kontrol et
        // (Yani GameManager bizi dinliyor mu?)
        if (OnDiceRolled != null)
        {
            // Dinleyenlere (GameManager'a) sonucu (rollResult) bildir.
            OnDiceRolled(rollResult);
        }
    }
}