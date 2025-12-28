using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Oyuncu
{
    public string oyuncuAdi;
    public OyuncuRengi renk;
    public bool isBot; // Bot kontrolü için bu değişkeni ekledik
    public List<Piyon> piyonlar;
    public List<Transform> kaleNoktalari;
}