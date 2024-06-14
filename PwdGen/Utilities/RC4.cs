using System;

namespace PwdGen.Utilities;

public class RC4
{
    private readonly byte[] Sbox;
    private readonly byte[] Kbox;
    private int i;
    private int j;
    public RC4()
    {
        Sbox = new byte[256];
        Kbox = new byte[256];
    }
    public void Init(byte[] Key)
    {
        if (Key.Length > 256) throw new ArgumentException("Key.Length > 256");
        
        for (int i = 0; i < 256; i++)
        {
            Sbox[i] = (byte)i;
            Kbox[i] = Key[i % Key.Length];
        }
        for (int i = 0, j = 0; i < 256; i++)
        {
            j = (j + Sbox[i] + Kbox[j]) % 256;
            (Sbox[j], Sbox[i]) = (Sbox[i], Sbox[j]);
        }
    }
    public byte Next()
    {
        j = (j + Sbox[i]) % 256;
        (Sbox[j], Sbox[i]) = (Sbox[i], Sbox[j]);
        byte k = Sbox[(Sbox[i] + Sbox[j]) % 256];
        i = (i + 1) % 256;
        return k;
    }
}
