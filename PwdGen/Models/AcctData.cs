using System.Security.Cryptography;
using System.Text;
using System;
using PwdGen.Utilities;

namespace PwdGen.Models;

public class AcctData
{
    public string UserName { get; set; } = string.Empty;
    public string SubPwd { get; set; } = string.Empty;
    public int PwdLen { get; set; } = 10;

    public bool UseNumbers { get; set; } = true;
    public bool UseLowercaseLetters { get; set; } = true;
    public bool UseUppercaseLetters { get; set; } = true;
    public bool UseSpecialCharacters { get; set; } = true;

    public HashALG HashALG { get; set; } = HashALG.SHA512;

    public string Comment { get; set; } = "";
    public long CreateTime { get; set; }

    private static char[] SpecialCharacters = ['.', '@', '$', '!', '%', '*', '#', '_', '~', '?', '&', '^'];

    public AcctData()
    {
    }

    public AcctData ShallowCopy()
    {
        return (MemberwiseClone() as AcctData)!;
    }

    public string Generate(string MainPwd)
    {
        if (!(UseNumbers || UseLowercaseLetters || UseUppercaseLetters ||
            UseSpecialCharacters))
            throw new Exception("Use a character set at least");
        byte[] hashValue1;
        byte[] hashValue2;
        HashAlgorithm hashAlgorithm = HashALG switch
        {
            HashALG.SHA256 => SHA256.Create(),
            HashALG.SHA384 => SHA384.Create(),
            HashALG.SHA512 => SHA512.Create(),
            _ => throw new ArgumentException($"Input.hashALG = {HashALG}"),
        };
        byte[] sub = Encoding.UTF8.GetBytes(SubPwd);
        byte[] main = Encoding.UTF8.GetBytes(MainPwd);
        hashValue1 = hashAlgorithm.ComputeHash([.. sub, .. main]);
        byte[] userName = Encoding.UTF8.GetBytes(UserName);
        hashValue2 = hashAlgorithm.ComputeHash([.. userName, .. main]);
        hashAlgorithm.Dispose();

        RC4 rc4 = new();
        rc4.Init(XOR(hashValue1, hashValue2));

        StringBuilder sb = new(PwdLen);
        for (int i = 0; i < PwdLen;)
        {
            int k = -1;
            switch (rc4.Next() % 4)
            {
                case 0:
                    if (UseNumbers)
                    {
                        while (k == -1) k = Uniformly(rc4.Next(), 10);
                        sb.Append(k);
                        i++;
                    }
                    break;
                case 1:
                    if (UseLowercaseLetters)
                    {
                        while (k == -1) k = Uniformly(rc4.Next(), 26);
                        byte[] ks = [(byte)(k + 97)];
                        sb.Append(Encoding.ASCII.GetString(ks));
                        i++;
                    }
                    break;
                case 2:
                    if (UseUppercaseLetters)
                    {
                        while (k == -1) k = Uniformly(rc4.Next(), 26);
                        byte[] ks = [(byte)(k + 65)];
                        sb.Append(Encoding.ASCII.GetString(ks));
                        i++;
                    }
                    break;
                case 3:
                    if (UseSpecialCharacters)
                    {
                        while (k == -1) k = Uniformly(rc4.Next(), SpecialCharacters.Length);
                        sb.Append(SpecialCharacters[k]);
                        i++;
                    }
                    break;
                default: throw new Exception("k > 4");
            }
        }
        return sb.ToString();
    }

    private static int Uniformly(int x, int b)
    {
        int _x = 256 / b * b;
        if (x >= _x) return -1;
        return x % b;
    }

    private static byte[] XOR(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) throw new ArgumentException("a.Length != b.Length");
        byte[] c = new byte[a.Length];
        for (int i = 0; i < c.Length; i++)
        {
            c[i] = (byte)(a[i] ^ b[i]);
        }
        return c;
    }
}
