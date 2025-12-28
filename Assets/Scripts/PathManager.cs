using System.Collections.Generic;
using UnityEngine;

// Bu, tüm yollarımızı bir arada tutan ve
// piyonlara yol tarifi veren yöneticidir.
public class PathManager : MonoBehaviour
{
    [Header("Genel Yol (40 Nokta)")]
    public List<Transform> genelYol;

    [Header("Eve Giriş Yolları")]
    public List<Transform> kirmiziEvYolu;
    public List<Transform> yesilEvYolu;
    public List<Transform> sariEvYolu;
    public List<Transform> maviEvYolu;
}