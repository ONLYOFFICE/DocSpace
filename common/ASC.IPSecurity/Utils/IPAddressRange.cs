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

namespace ASC.IPSecurity;

internal class IPAddressRange
{
    private readonly AddressFamily _addressFamily;
    private readonly byte[] _lowerBytes;
    private readonly byte[] _upperBytes;

    public IPAddressRange(IPAddress lower, IPAddress upper)
    {
        _addressFamily = lower.AddressFamily;
        _lowerBytes = lower.GetAddressBytes();
        _upperBytes = upper.GetAddressBytes();
    }

    public bool IsInRange(IPAddress address)
    {
        if (address.AddressFamily != _addressFamily)
        {
            return false;
        }

        var addressBytes = address.GetAddressBytes();

        bool lowerBoundary = true, upperBoundary = true;

        for (var i = 0; i < _lowerBytes.Length &&
                        (lowerBoundary || upperBoundary); i++)
        {
            var addressByte = addressBytes[i];
            var upperByte = _upperBytes[i];
            var lowerByte = _lowerBytes[i];

            if ((lowerBoundary && addressByte < lowerByte) || (upperBoundary && addressByte > upperByte))
            {
                return false;
            }

            lowerBoundary &= addressByte == lowerByte;
            upperBoundary &= addressByte == upperByte;
        }

        return true;
    }

    public static bool IsInRange(string ipAddress, string CIDRmask)
    {
        var parts = CIDRmask.Split('/');

        var requestIP = IPAddress.Parse(ipAddress);
        var restrictionIP = IPAddress.Parse(parts[0]);

        if (requestIP.AddressFamily != restrictionIP.AddressFamily)
        {
            return false;
        }

        var IP_addr = BitConverter.ToInt32(requestIP.GetAddressBytes(), 0);
        var CIDR_addr = BitConverter.ToInt32(restrictionIP.GetAddressBytes(), 0);
        var CIDR_mask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

        return (IP_addr & CIDR_mask) == (CIDR_addr & CIDR_mask);
    }
}
