using System.Diagnostics.CodeAnalysis;

namespace PwdGen.Utilities;

public class RC4
{
    private byte[] Sbox;
    private byte[] Kbox;
    private int i;
    private int j;

    public RC4(byte[] Key)
    {
        Init(Key);
    }

    [MemberNotNull(nameof(Sbox), nameof(Kbox))]
    public void Init(byte[] Key)
    {
        if (Key.Length > 256) throw new ArgumentException("Key.Length > 256");
        Sbox = new byte[256];
        Kbox = new byte[256];
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
        i = 0;
        j = 0;
    }

    public IEnumerator<byte> GetEnumerator(int maxCount = int.MaxValue)
    {
        for (int c = 0; c < maxCount; c++)
        {
            j = (j + Sbox[i]) % 256;
            (Sbox[j], Sbox[i]) = (Sbox[i], Sbox[j]);
            byte k = Sbox[(Sbox[i] + Sbox[j]) % 256];
            i = (i + 1) % 256;
            yield return k;
        }
    }
}