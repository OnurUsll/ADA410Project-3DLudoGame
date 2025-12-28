public static class GameSettings
{
    // Varsayılan olarak 4 kişilik boş bir dizi oluşturuyoruz
    private static bool[] _isBotArr = new bool[] { true, true, true, true };

    public static bool[] isBotArr
    {
        get { return _isBotArr; }
        set { _isBotArr = value; }
    }
}