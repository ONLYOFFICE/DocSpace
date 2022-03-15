namespace ASC.Common.Security;

public class AscRandom : Random
{
    private const int Mbig = int.MaxValue;
    private const int Mseed = 161803398;
    private const int Mz = 0;

    private int _inext;
    private int _inextp;
    private readonly int[] _seeds;

    public AscRandom() : this(Environment.TickCount) { }

    public AscRandom(int seed)
    {
        _seeds = new int[56];
        var num4 = (seed == int.MinValue) ? int.MaxValue : Math.Abs(seed);
        var num2 = 161803398 - num4;
        _seeds[^1] = num2;
        var num3 = 1;

        for (var i = 1; i < _seeds.Length - 1; i++)
        {
            var index = 21 * i % (_seeds.Length - 1);
            _seeds[index] = num3;
            num3 = num2 - num3;

            if (num3 < 0)
            {
                num3 += int.MaxValue;
            }

            num2 = _seeds[index];
        }

        for (var j = 1; j < 5; j++)
        {
            for (var k = 1; k < _seeds.Length; k++)
            {
                _seeds[k] -= _seeds[1 + ((k + 30) % (_seeds.Length - 1))];

                if (_seeds[k] < 0)
                {
                    _seeds[k] += int.MaxValue;
                }
            }
        }

        _inext = 0;
        _inextp = 21;
    }

    public override int Next(int maxValue)
    {
        if (maxValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue));
        }

        return (int)(InternalSample() * 4.6566128752457969E-10 * maxValue);
    }

    public override void NextBytes(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (byte)(InternalSample() % (byte.MaxValue + 1));
        }
    }

    private int InternalSample()
    {
        var inext = _inext;
        var inextp = _inextp;

        if (++inext >= _seeds.Length - 1)
        {
            inext = 1;
        }

        if (++inextp >= _seeds.Length - 1)
        {
            inextp = 1;
        }

        var num = _seeds[inext] - _seeds[inextp];

        if (num == int.MaxValue)
        {
            num--;
        }

        if (num < 0)
        {
            num += int.MaxValue;
        }

        _seeds[inext] = num;
        _inext = inext;
        _inextp = inextp;

        return num;
    }
}