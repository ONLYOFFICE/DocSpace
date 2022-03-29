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

namespace ASC.Common.Utils;

public class DnsLookup
{
    private readonly IDnsResolver _sDnsResolver;
    private readonly DnsClient _dnsClient;

    public DnsLookup()
    {
        _dnsClient = DnsClient.Default;
        _sDnsResolver = new DnsStubResolver(_dnsClient);
    }

    /// <summary>
    /// Get domain MX records
    /// </summary>
    /// <param name="domainName">domain name</param>
    /// <exception cref="ArgumentNullException">if domainName is empty</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <returns>list of MxRecord</returns>
    public List<MxRecord> GetDomainMxRecords(string domainName)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domainName);

        var mxRecords = DnsResolve<MxRecord>(domainName, RecordType.Mx);

        return mxRecords;
    }

    /// <summary>
    /// Check existance of MX record in domain DNS
    /// </summary>
    /// <param name="domainName">domain name</param>
    /// <param name="mxRecord">MX record value</param>
    /// <exception cref="ArgumentNullException">if domainName is empty</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <returns>true if exists and vice versa</returns>
    public bool IsDomainMxRecordExists(string domainName, string mxRecord)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domainName);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(mxRecord);

        var mxDomain = DomainName.Parse(mxRecord);

        var records = GetDomainMxRecords(domainName);

        return records.Any(
                mx => mx.ExchangeDomainName.Equals(mxDomain));
    }

    /// <summary>
    /// Check domain existance
    /// </summary>
    /// <param name="domainName"></param>
    /// <exception cref="ArgumentNullException">if domainName is empty</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <exception cref="SystemException">if DNS request failed</exception>
    /// <returns>true if any DNS record exists and vice versa</returns>
    public bool IsDomainExists(string domainName)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domainName);

        var dnsMessage = GetDnsMessage(domainName);

        return dnsMessage.AnswerRecords.Count != 0;
    }

    /// <summary>
    /// Get domain A records
    /// </summary>
    /// <param name="domainName">domain name</param>
    /// <exception cref="ArgumentNullException">if domainName is empty</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <returns>list of ARecord</returns>
    public List<ARecord> GetDomainARecords(string domainName)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domainName);

        var aRecords = DnsResolve<ARecord>(domainName, RecordType.A);

        return aRecords;
    }

    /// <summary>
    /// Get domain IP addresses list
    /// </summary>
    /// <param name="domainName">domain name</param>
    /// <exception cref="ArgumentNullException">if domainName is empty</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <returns>list of IPAddress</returns>
    public List<IPAddress> GetDomainIPs(string domainName)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domainName);

        var addresses = _sDnsResolver.ResolveHost(domainName);

        return addresses;
    }

    /// <summary>
    /// Get domain TXT records
    /// </summary>
    /// <param name="domainName">domain name</param>
    /// <exception cref="ArgumentNullException">if domainName is empty</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <returns>list of TxtRecord</returns>
    public List<TxtRecord> GetDomainTxtRecords(string domainName)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domainName);

        var txtRecords = DnsResolve<TxtRecord>(domainName, RecordType.Txt);

        return txtRecords;
    }

    /// <summary>
    /// Check existance of TXT record in domain DNS
    /// </summary>
    /// <param name="domainName">domain name</param>
    /// <param name="recordValue">TXT record value</param>
    /// <exception cref="ArgumentNullException">if domainName is empty</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <returns>true if exists and vice versa</returns>
    public bool IsDomainTxtRecordExists(string domainName, string recordValue)
    {
        var txtRecords = GetDomainTxtRecords(domainName);

        return
            txtRecords.Any(
                txtRecord =>
                    txtRecord.TextData.Trim('\"').Equals(recordValue, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Check existance of DKIM record in domain DNS
    /// </summary>
    /// <param name="domainName">domain name</param>
    /// <param name="dkimSelector">DKIM selector (example is "dkim")</param>
    /// <param name="dkimValue">DKIM record value</param>
    /// <exception cref="ArgumentNullException">if domainName is empty</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <returns>true if exists and vice versa</returns>
    public bool IsDomainDkimRecordExists(string domainName, string dkimSelector, string dkimValue)
    {
        var dkimRecordName = dkimSelector + "._domainkey." + domainName;

        var txtRecords = GetDomainTxtRecords(dkimRecordName);

        return txtRecords.Any(txtRecord => txtRecord.TextData.Trim('\"').Equals(dkimValue));
    }

    /// <summary>
    /// Check existance Domain in PTR record
    /// </summary>
    /// <param name="ipAddress">IP address for PTR check</param>
    /// <param name="domainName">PTR domain name</param>
    /// <exception cref="ArgumentNullException">if domainName or ipAddress is empty/null</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <returns>true if exists and vice versa</returns>
    public bool IsDomainPtrRecordExists(IPAddress ipAddress, string domainName)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domainName);

        ArgumentNullException.ThrowIfNull(ipAddress);

        var domain = DomainName.Parse(domainName);

        var ptrDomain = _sDnsResolver.ResolvePtr(ipAddress);

        return ptrDomain.Equals(domain);
    }

    /// <summary>
    /// Check existance Domain in PTR record
    /// </summary>
    /// <param name="ipAddress">IP address for PTR check</param>
    /// <param name="domainName">PTR domain name</param>
    /// <exception cref="ArgumentNullException">if domainName or ipAddress is empty/null</exception>
    /// <exception cref="ArgumentException">if domainName is invalid</exception>
    /// <exception cref="FormatException">if ipAddress is invalid</exception>
    /// <returns>true if exists and vice versa</returns>
    public bool IsDomainPtrRecordExists(string ipAddress, string domainName)
    {
        return IsDomainPtrRecordExists(IPAddress.Parse(ipAddress), domainName);
    }

    private DnsMessage GetDnsMessage(string domainName, RecordType? type = null)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domainName);

        var domain = DomainName.Parse(domainName);

        var dnsMessage = type.HasValue ? _dnsClient.Resolve(domain, type.Value) : _dnsClient.Resolve(domain);
        if ((dnsMessage == null) ||
            ((dnsMessage.ReturnCode != ReturnCode.NoError) && (dnsMessage.ReturnCode != ReturnCode.NxDomain)))
        {
            throw new SystemException(); // DNS request failed
        }

        return dnsMessage;
    }

    private List<T> DnsResolve<T>(string domainName, RecordType type)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domainName);

        var dnsMessage = GetDnsMessage(domainName, type);

        return dnsMessage.AnswerRecords.Where(r => r.RecordType == type).Cast<T>().ToList();
    }
}