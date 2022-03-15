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
}
