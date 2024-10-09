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
    public int PwdLen { get; set; } = 15;

    [Browsable(false)]
    [DisplayName("Date modified")]
    [NotNull]
    public long DateModified { get; set; }

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
        DateModified = item.DateModified;
    }

    public string[] GetAllChars()
    {
        IEnumerable<string> returnValue = [];
        List<string> UpLetters = new(10);
        List<string> LowLetters = new(10);
        List<string> Numbers = new(10);
        for (int i = 0; i < 25; i++) UpLetters.Add(Encoding.ASCII.GetString([(byte)(i + 65)]));
        for (int i = 0; i < 25; i++) LowLetters.Add(Encoding.ASCII.GetString([(byte)(i + 97)]));
        for (int i = 0; i < 10; i++) Numbers.Add(i.ToString());
        if (UseUpLetter) returnValue = returnValue.Concat(UpLetters);
        if (UseLowLetter) returnValue = returnValue.Concat(LowLetters);
        if (UseNumber) returnValue = returnValue.Concat(Numbers);
        if (UseSpChar) returnValue = returnValue.Concat(SpChars);
        return returnValue.ToArray();
    }

    public string Generate(string MainPassword)
    {
        var chars = GetAllChars();
        if (chars.Length == 0) throw new Exception("Use a character set at least");
        byte[] hashValue1 = SHA512.HashData(Encoding.UTF8.GetBytes($"UserName:{UserName}{MainPassword}"));
        byte[] hashValue2 = SHA512.HashData(Encoding.UTF8.GetBytes($"Platform:{MainPassword}{Platform}"));
        RC4 rc4 = new(Utility.XOR(hashValue1, hashValue2).ToArray());
        StringBuilder sb = new(PwdLen);
        using var rc4Enumerator = rc4.GetEnumerator();
        if (!rc4Enumerator.MoveNext()) throw new Exception();
        for (int sc = 0; sc < SkipCount; sc++)
        {
            if (!rc4Enumerator.MoveNext()) throw new Exception();
        }
        for (int i = 0; i < PwdLen;)
        {
            int k = -1;
            while (k == -1)
            {
                k = Utility.Uniformly256(rc4Enumerator.Current, chars.Length);
                if (!rc4Enumerator.MoveNext()) throw new Exception();
            }
            sb.Append(chars[k]);
            i++;
        }
        return sb.ToString();
    }
}
