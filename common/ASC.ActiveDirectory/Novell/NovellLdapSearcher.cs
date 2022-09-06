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

namespace ASC.ActiveDirectory.Novell;

[Scope]
public class NovellLdapSearcher : IDisposable
{
    protected readonly ILogger<NovellLdapSearcher> _logger;
    private LdapCertificateConfirmRequest _certificateConfirmRequest;
    private static readonly object _rootSync = new object();
    private readonly IConfiguration _configuration;
    private readonly NovellLdapEntryExtension _novellLdapEntryExtension;
    private LdapConnection _ldapConnection;

    public string Login { get; private set; }
    public string Password { get; private set; }
    public string Server { get; private set; }
    public int PortNumber { get; private set; }
    public bool StartTls { get; private set; }
    public bool Ssl { get; private set; }
    public bool AcceptCertificate { get; private set; }
    public string AcceptCertificateHash { get; private set; }

    public string LdapUniqueIdAttribute { get; set; }

    private Dictionary<string, string[]> _capabilities;

    public bool IsConnected
    {
        get { return _ldapConnection != null && _ldapConnection.Connected; }
    }

    public NovellLdapSearcher(
        IConfiguration configuration,
        ILogger<NovellLdapSearcher> logger,
        NovellLdapEntryExtension novellLdapEntryExtension)
    {
        _logger = logger;
        _configuration = configuration;
        _novellLdapEntryExtension = novellLdapEntryExtension;
        LdapUniqueIdAttribute = configuration["ldap:unique:id"];
    }

    public void Init(string login,
        string password,
        string server,
        int portNumber,
        bool startTls,
        bool ssl,
        bool acceptCertificate,
        string acceptCertificateHash = null)
    {
        Login = login;
        Password = password;
        Server = server;
        PortNumber = portNumber;
        StartTls = startTls;
        Ssl = ssl;
        AcceptCertificate = acceptCertificate;
        AcceptCertificateHash = acceptCertificateHash;
    }

    public void Connect()
    {
        if (Server.StartsWith("LDAP://"))
        {
            Server = Server.Substring("LDAP://".Length);
        }

        LdapConnection ldapConnection;

        if (StartTls || Ssl)
        {
            var ldapConnectionOptions = new LdapConnectionOptions();
            ldapConnectionOptions.ConfigureRemoteCertificateValidationCallback(ServerCertValidationHandler);
            ldapConnection = new LdapConnection(ldapConnectionOptions);
        }
        else
        {
            ldapConnection = new LdapConnection();
        }

        if (Ssl)
        {
            ldapConnection.SecureSocketLayer = true;
        }

        try
        {
            ldapConnection.ConnectionTimeout = 30000; // 30 seconds

            _logger.DebugldapConnection(Server, PortNumber);

            ldapConnection.Connect(Server, PortNumber);

            if (StartTls)
            {
                _logger.DebugStartTls();
                ldapConnection.StartTls();
            }
        }
        catch (Exception ex)
        {
            if (_certificateConfirmRequest == null)
            {
                if (ex.Message.StartsWith("Connect Error"))
                {
                    throw new SocketException();
                }

                if (ex.Message.StartsWith("Unavailable"))
                {
                    throw new NotSupportedException(ex.Message);
                }

                throw;
            }

            _logger.DebugLdapCertificateConfirmationRequested();

            ldapConnection.Disconnect();

            var exception = new NovellLdapTlsCertificateRequestedException
            {
                CertificateConfirmRequest = _certificateConfirmRequest
            };

            throw exception;
        }

        if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
        {
            _logger.DebugBindAnonymous();

            ldapConnection.Bind(null, null);
        }
        else
        {
            _logger.DebugBind(Login);

            ldapConnection.Bind(Login, Password);
        }

        if (!ldapConnection.Bound)
        {
            throw new Exception("Bind operation wasn't completed successfully.");
        }

        _ldapConnection = ldapConnection;
    }

    private bool ServerCertValidationHandler(object sender, X509Certificate certificate,
        X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
        {
            return true;
        }

        lock (_rootSync)
        {
            var certHash = certificate.GetCertHashString();

            if (AcceptCertificate)
            {
                if (AcceptCertificateHash == null || AcceptCertificateHash.Equals(certHash))
                {
                    return true;
                }

                AcceptCertificate = false;
                AcceptCertificateHash = null;
            }

            _logger.WarnSslPolicyErrors(sslPolicyErrors);

            _certificateConfirmRequest = LdapCertificateConfirmRequest.FromCert(certificate, chain, sslPolicyErrors, false, true, _logger);
        }

        return false;
    }

    public enum LdapScope
    {
        Base = LdapConnection.ScopeBase,
        One = LdapConnection.ScopeOne,
        Sub = LdapConnection.ScopeSub
    }

    public List<LdapObject> Search(LdapScope scope, string searchFilter,
        string[] attributes = null, int limit = -1, LdapSearchConstraints searchConstraints = null)
    {
        return Search("", scope, searchFilter, attributes, limit, searchConstraints);
    }

    public List<LdapObject> Search(string searchBase, LdapScope scope, string searchFilter,
        string[] attributes = null, int limit = -1, LdapSearchConstraints searchConstraints = null)
    {
        if (!IsConnected)
        {
            Connect();
        }

        if (searchBase == null)
        {
            searchBase = "";
        }

        var entries = new List<LdapEntry>();

        if (string.IsNullOrEmpty(searchFilter))
        {
            return new List<LdapObject>();
        }

        if (attributes == null)
        {
            if (string.IsNullOrEmpty(LdapUniqueIdAttribute))
            {
                attributes = new[]
                {
                        "*", LdapConstants.RfcLDAPAttributes.ENTRY_DN, LdapConstants.RfcLDAPAttributes.ENTRY_UUID,
                        LdapConstants.RfcLDAPAttributes.NS_UNIQUE_ID, LdapConstants.RfcLDAPAttributes.GUID
                    };
            }
            else
            {
                attributes = new[] { "*", LdapUniqueIdAttribute };
            }
        }

        var ldapSearchConstraints = searchConstraints ?? new LdapSearchConstraints
        {
            // Maximum number of search results to return.
            // The value 0 means no limit. The default is 1000.
            MaxResults = limit == -1 ? 0 : limit,
            // Returns the number of results to block on during receipt of search results. 
            // This should be 0 if intermediate results are not needed, and 1 if results are to be processed as they come in.
            //BatchSize = 0,
            // The maximum number of referrals to follow in a sequence during automatic referral following. 
            // The default value is 10. A value of 0 means no limit.
            HopLimit = 0,
            // Specifies whether referrals are followed automatically
            // Referrals of any type other than to an LDAP server (for example, a referral URL other than ldap://something) are ignored on automatic referral following.
            // The default is false.
            ReferralFollowing = true,
            // The number of seconds to wait for search results.
            // Sets the maximum number of seconds that the server is to wait when returning search results.
            //ServerTimeLimit = 600000, // 10 minutes
            // Sets the maximum number of milliseconds the client waits for any operation under these constraints to complete.
            // If the value is 0, there is no maximum time limit enforced by the API on waiting for the operation results.
            //TimeLimit = 600000 // 10 minutes
        };

        var queue = _ldapConnection.Search(searchBase,
            (int)scope, searchFilter, attributes, false, ldapSearchConstraints);

        while (queue.HasMore())
        {
            LdapEntry nextEntry;
            try
            {
                nextEntry = queue.Next();

                if (nextEntry == null)
                {
                    continue;
                }
            }
            catch (LdapException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.Contains("Sizelimit Exceeded"))
                {
                    if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password) && limit == -1)
                    {
                        _logger.WarnStartTrySearchSimple();

                        List<LdapObject> simpleResults;

                        if (TrySearchSimple(searchBase, scope, searchFilter, out simpleResults, attributes, limit,
                            searchConstraints))
                        {
                            if (entries.Count >= simpleResults.Count)
                            {
                                break;
                            }

                            return simpleResults;
                        }
                    }

                    break;
                }

                _logger.ErrorSearch( searchFilter, ex);
                continue;
            }

            entries.Add(nextEntry);

            if (string.IsNullOrEmpty(LdapUniqueIdAttribute))
            {
                LdapUniqueIdAttribute = GetLdapUniqueId(nextEntry);
            }
        }

        var result = _novellLdapEntryExtension.ToLdapObjects(entries, LdapUniqueIdAttribute);

        return result;
    }

    private bool TrySearchSimple(string searchBase, LdapScope scope, string searchFilter, out List<LdapObject> results,
        string[] attributes = null, int limit = -1, LdapSearchConstraints searchConstraints = null)
    {

        try
        {
            results = SearchSimple(searchBase, scope, searchFilter, attributes, limit, searchConstraints);

            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorTrySearchSimple(ex);
        }

        results = null;
        return false;
    }

    public List<LdapObject> SearchSimple(string searchBase, LdapScope scope, string searchFilter,
        string[] attributes = null, int limit = -1, LdapSearchConstraints searchConstraints = null)
    {
        if (!IsConnected)
        {
            Connect();
        }

        if (searchBase == null)
        {
            searchBase = "";
        }

        var entries = new List<LdapEntry>();

        if (string.IsNullOrEmpty(searchFilter))
        {
            return new List<LdapObject>();
        }

        if (attributes == null)
        {
            if (string.IsNullOrEmpty(LdapUniqueIdAttribute))
            {
                attributes = new[]
                {
                        "*", LdapConstants.RfcLDAPAttributes.ENTRY_DN, LdapConstants.RfcLDAPAttributes.ENTRY_UUID,
                        LdapConstants.RfcLDAPAttributes.NS_UNIQUE_ID, LdapConstants.RfcLDAPAttributes.GUID
                    };
            }
            else
            {
                attributes = new[] { "*", LdapUniqueIdAttribute };
            }
        }

        var ldapSearchConstraints = searchConstraints ?? new LdapSearchConstraints
        {
            // Maximum number of search results to return.
            // The value 0 means no limit. The default is 1000.
            MaxResults = limit == -1 ? 0 : limit,
            // Returns the number of results to block on during receipt of search results. 
            // This should be 0 if intermediate results are not needed, and 1 if results are to be processed as they come in.
            //BatchSize = 0,
            // The maximum number of referrals to follow in a sequence during automatic referral following. 
            // The default value is 10. A value of 0 means no limit.
            HopLimit = 0,
            // Specifies whether referrals are followed automatically
            // Referrals of any type other than to an LDAP server (for example, a referral URL other than ldap://something) are ignored on automatic referral following.
            // The default is false.
            ReferralFollowing = true,
            // The number of seconds to wait for search results.
            // Sets the maximum number of seconds that the server is to wait when returning search results.
            //ServerTimeLimit = 600000, // 10 minutes
            // Sets the maximum number of milliseconds the client waits for any operation under these constraints to complete.
            // If the value is 0, there is no maximum time limit enforced by the API on waiting for the operation results.
            //TimeLimit = 600000 // 10 minutes
        };

        // initially, cookie must be set to an empty string
        var pageSize = 2;
        var cookie = Array.ConvertAll(Encoding.ASCII.GetBytes(""), b => unchecked(b));
        var i = 0;

        do
        {
            var requestControls = new LdapControl[1];
            requestControls[0] = new SimplePagedResultsControl(pageSize, cookie);
            ldapSearchConstraints.SetControls(requestControls);
            _ldapConnection.Constraints = ldapSearchConstraints;

            var res = _ldapConnection.Search(searchBase,
                (int)scope, searchFilter, attributes, false, (LdapSearchConstraints)null);

            while (res.HasMore())
            {
                LdapEntry nextEntry;
                try
                {
                    nextEntry = res.Next();

                    if (nextEntry == null)
                    {
                        continue;
                    }
                }
                catch (LdapException ex)
                {
                    if (ex is LdapReferralException)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(ex.Message) && ex.Message.Contains("Sizelimit Exceeded"))
                    {
                        break;
                    }

                    _logger.ErrorSearchSimple(searchFilter, ex);
                    continue;
                }

                _logger.DebugDnEnumeration(++i, nextEntry.Dn);

                entries.Add(nextEntry);

                if (string.IsNullOrEmpty(LdapUniqueIdAttribute))
                {
                    LdapUniqueIdAttribute = GetLdapUniqueId(nextEntry);
                }
            }

            // Server should send back a control irrespective of the 
            // status of the search request
            var controls = res.ResponseControls;
            if (controls == null)
            {
                _logger.DebugNoControlsReturned();
                cookie = null;
            }
            else
            {
                // Multiple controls could have been returned
                foreach (var control in controls)
                {
                    /* Is this the LdapPagedResultsResponse control? */
                    if (!(control is SimplePagedResultsControl))
                    {
                        continue;
                    }

                    var response = new SimplePagedResultsControl(control.Id,
                        control.Critical, control.GetValue());

                    cookie = response.Cookie;
                }
            }
            // if cookie is empty, we are done.
        } while (cookie != null && cookie.Length > 0);

        var result = _novellLdapEntryExtension.ToLdapObjects(entries, LdapUniqueIdAttribute);

        return result;
    }

    public Dictionary<string, string[]> GetCapabilities()
    {
        if (_capabilities != null)
        {
            return _capabilities;
        }

        _capabilities = new Dictionary<string, string[]>();

        try
        {
            var ldapSearchConstraints = new LdapSearchConstraints
            {
                MaxResults = int.MaxValue,
                HopLimit = 0,
                ReferralFollowing = true
            };

            var ldapSearchResults = _ldapConnection.Search("", LdapConnection.ScopeBase, LdapConstants.OBJECT_FILTER,
                new[] { "*", "supportedControls", "supportedCapabilities" }, false, ldapSearchConstraints);

            while (ldapSearchResults.HasMore())
            {
                LdapEntry nextEntry;
                try
                {
                    nextEntry = ldapSearchResults.Next();

                    if (nextEntry == null)
                    {
                        continue;
                    }
                }
                catch (LdapException ex)
                {
                    _logger.ErrorGetCapabilitiesLoopResultsFailed(ex);
                    continue;
                }

                var attributeSet = nextEntry.GetAttributeSet();

                var ienum = attributeSet.GetEnumerator();

                while (ienum.MoveNext())
                {
                    var attribute = (LdapAttribute)ienum.Current;
                    if (attribute == null)
                    {
                        continue;
                    }

                    var attributeName = attribute.Name;
                    var attributeVals = attribute.StringValueArray
                        .ToList()
                        .Select(s =>
                        {
                            if (Base64.IsLdifSafe(s))
                            {
                                return s;
                            }

                            s = Base64.Encode(s);
                            return s;
                        }).ToArray();

                    _capabilities.Add(attributeName, attributeVals);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.ErrorGetCapabilitiesFailed(ex);
        }

        return _capabilities;
    }

    private string GetLdapUniqueId(LdapEntry ldapEntry)
    {
        try
        {
            var ldapUniqueIdAttribute = _configuration["ldap:unique:id"];

            if (ldapUniqueIdAttribute != null)
            {
                return ldapUniqueIdAttribute;
            }

            if (!string.IsNullOrEmpty(
                _novellLdapEntryExtension.GetAttributeValue(ldapEntry, LdapConstants.ADSchemaAttributes.OBJECT_SID) as string))
            {
                ldapUniqueIdAttribute = LdapConstants.ADSchemaAttributes.OBJECT_SID;
            }
            else if (!string.IsNullOrEmpty(
                _novellLdapEntryExtension.GetAttributeValue(ldapEntry, LdapConstants.RfcLDAPAttributes.ENTRY_UUID) as string))
            {
                ldapUniqueIdAttribute = LdapConstants.RfcLDAPAttributes.ENTRY_UUID;
            }
            else if (!string.IsNullOrEmpty(
                _novellLdapEntryExtension.GetAttributeValue(ldapEntry, LdapConstants.RfcLDAPAttributes.NS_UNIQUE_ID) as string))
            {
                ldapUniqueIdAttribute = LdapConstants.RfcLDAPAttributes.NS_UNIQUE_ID;
            }
            else if (!string.IsNullOrEmpty(
                _novellLdapEntryExtension.GetAttributeValue(ldapEntry, LdapConstants.RfcLDAPAttributes.GUID) as string))
            {
                ldapUniqueIdAttribute = LdapConstants.RfcLDAPAttributes.GUID;
            }

            return ldapUniqueIdAttribute;
        }
        catch (Exception ex)
        {
            _logger.ErrorGetLdapUniqueId(ex);
        }

        return null;
    }

    public void Dispose()
    {
        if (!IsConnected)
        {
            return;
        }

        try
        {
            _ldapConnection.Constraints.TimeLimit = 10000;
            _ldapConnection.SearchConstraints.ServerTimeLimit = 10000;
            _ldapConnection.SearchConstraints.TimeLimit = 10000;
            _ldapConnection.ConnectionTimeout = 10000;

            if (_ldapConnection.Tls)
            {
                _logger.DebugLdapConnectionStopTls();
                _ldapConnection.StopTls();
            }

            _logger.DebugLdapConnectionDisconnect();
            _ldapConnection.Disconnect();

            _logger.DebugLdapConnectionDispose();
            _ldapConnection.Dispose();

            _ldapConnection = null;
        }
        catch (Exception ex)
        {
            _logger.ErrorLdapDisposeFailed(ex);
        }
    }
}
