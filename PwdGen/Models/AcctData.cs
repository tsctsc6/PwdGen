using PwdGen.Utilities;
using SQLite;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace PwdGen.Models;

public class AcctData
{
    [PrimaryKey]
    [AutoIncrement]
    [Browsable(false)]
    [Unique]
    public int Id { get; set; }

    [DisplayName("User Name")]
    [System.ComponentModel.DataAnnotations.MaxLength(50)]
    [SQLite.MaxLength(50)]
    [Indexed]
    [NotNull]
    public string UserName { get; set; } = string.Empty;

    [DisplayName("Platform")]
    [System.ComponentModel.DataAnnotations.MaxLength(50)]
    [SQLite.MaxLength(50)]
    [Indexed]
    [NotNull]
    public string Platform { get; set; } = string.Empty;

    [DisplayName("Remark")]
    [System.ComponentModel.DataAnnotations.MaxLength(100)]
    [SQLite.MaxLength(100)]
    [NotNull]
    public string Remark { get; set; } = string.Empty;

    [DisplayName("Skip Count")]
    [Range(0, 255)]
    [NotNull]
    public int SkipCount { get; set; } = 0;

    [DisplayName("Use Up Letter")]
    [NotNull]
    public bool UseUpLetter { get; set; } = true;

    [DisplayName("Use Low Letter")]
    [NotNull]
    public bool UseLowLetter { get; set; } = true;

    [DisplayName("Use Number")]
    [NotNull]
    public bool UseNumber { get; set; } = true;

    [DisplayName("Use Special Character")]
    [NotNull]
    public bool UseSpChar { get; set; } = true;

    [Required]
    [Range(1, 255)]
    [DisplayName("Password Length")]
    [NotNull]
    public int PwdLen { get; set; } = 10;

    [Browsable(false)]
    [DisplayName("Create Time")]
    [NotNull]
    public long CreateTime { get; set; }

    public readonly static string[] SpChars =
        ["~", "!", "@", "#", "$", "%", "^", "&", "*", "_",
        "|", ".", ",", "?", ";", ":"];

    public AcctData Clone()
    {
        return (MemberwiseClone() as AcctData)!;
    }

    public void Clone(AcctData item)
    {
        Id = item.Id;
        UserName = item.UserName;
        Platform = item.Platform;
        Remark = item.Remark;
        SkipCount = item.SkipCount;
        UseUpLetter = item.UseUpLetter;
        UseLowLetter = item.UseLowLetter;
        UseNumber = item.UseNumber;
        UseSpChar = item.UseSpChar;
        PwdLen = item.PwdLen;
        CreateTime = item.CreateTime;
    }

    public string Generate(string MainPassword)
    {
        if (!(UseNumber || UseUpLetter || UseLowLetter || UseSpChar))
            throw new Exception("Use a character set at least");
        byte[] hashValue1 = SHA512.HashData(Encoding.UTF8.GetBytes(UserName + MainPassword));
        byte[] hashValue2 = SHA512.HashData(Encoding.UTF8.GetBytes(MainPassword + Platform));
        RC4 rc4 = new(Utilities.Utilities.XOR(hashValue1, hashValue2).ToArray());
        StringBuilder sb = new(PwdLen);
        using var rc4Enumerator = rc4.GetEnumerator();
        for (int i = 0; i < PwdLen;)
        {
            string c = string.Empty;
            int k = -1;
            if (!rc4Enumerator.MoveNext()) throw new Exception();
            for(int sc = 0; sc < SkipCount; sc++)
            {
                if (!rc4Enumerator.MoveNext()) throw new Exception();
            }
            switch (rc4Enumerator.Current % 4)
            {
                case 0:
                    if (UseUpLetter)
                    {
                        while (k == -1)
                        {
                            k = Utilities.Utilities.Uniformly256(rc4Enumerator.Current, 26);
                            if (!rc4Enumerator.MoveNext()) throw new Exception();
                        }
                        byte[] ks = [(byte)(k + 65)];
                        sb.Append(Encoding.ASCII.GetString(ks));
                        break;
                    }
                    else continue;
                case 1:
                    if (UseLowLetter)
                    {
                        while (k == -1)
                        {
                            k = Utilities.Utilities.Uniformly256(rc4Enumerator.Current, 26);
                            if (!rc4Enumerator.MoveNext()) throw new Exception();
                        }
                        byte[] ks = [(byte)(k + 97)];
                        sb.Append(Encoding.ASCII.GetString(ks));
                        break;
                    }
                    else continue;
                case 2:
                    if (UseNumber)
                    {
                        while (k == -1)
                        {
                            k = Utilities.Utilities.Uniformly256(rc4Enumerator.Current, 10);
                            if (!rc4Enumerator.MoveNext()) throw new Exception();
                        }
                        sb.Append(k);
                        break;
                    }
                    else continue;
                case 3:
                    if (UseSpChar)
                    {
                        while (k == -1)
                        {
                            k = Utilities.Utilities.Uniformly256(rc4Enumerator.Current, SpChars.Length);
                            if (!rc4Enumerator.MoveNext()) throw new Exception();
                        }
                        sb.Append(SpChars[k]);
                        break;
                    }
                    else continue;
            }
            sb.Append(c);
            i++;
        }
        return sb.ToString();
    }
}
