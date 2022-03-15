// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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