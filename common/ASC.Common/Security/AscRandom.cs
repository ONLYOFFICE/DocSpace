/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

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
            var index = (21 * i) % (_seeds.Length - 1);
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
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

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