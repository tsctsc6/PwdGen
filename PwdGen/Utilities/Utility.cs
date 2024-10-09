namespace PwdGen.Utilities;

public static class Utility
{
    public static IEnumerable<byte> XOR(IEnumerable<byte> bytes1, IEnumerable<byte> bytes2)
    {
        using var byteEnumerator1 = bytes1.GetEnumerator();
        using var byteEnumerator2 = bytes2.GetEnumerator();
        while (byteEnumerator1.MoveNext() && byteEnumerator2.MoveNext())
        {
            yield return (byte)(byteEnumerator1.Current ^ byteEnumerator2.Current);
        }
    }

    /// <summary>
    /// 在 n 中情况随机选择一种时，一般做法是，采样值 % n 。
    /// 但是如果 256 不能整除 n 时，概率的分布不会均匀。
    /// 该函数做的是，如果采样值大于等于 256 - (256 % n) ，不采纳本次采样结果。
    /// </summary>
    /// <param name="x">采样值</param>
    /// <param name="n">情况数</param>
    /// <returns>-1 ，不采纳本次采样结果；大于 0 ，采纳本次采样结果</returns>
    public static int Uniformly256(int x, int n)
    {
        if (x >= 256 - (256 % n)) return -1;
        return x % n;
    }
}
