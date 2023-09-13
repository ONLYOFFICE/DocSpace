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

namespace ASC.ActiveDirectory.Base.Data;
public class LdapCertificateConfirmRequest
{
    private volatile bool _approved;
    private volatile bool _requested;
    private volatile string _serialNumber;
    private volatile string _issuerName;
    private volatile string _subjectName;
    private volatile string _hash;
    private volatile int[] _certificateErrors;

    public bool Approved { get { return _approved; } set { _approved = value; } }

    public bool Requested { get { return _requested; } set { _requested = value; } }

    public string SerialNumber { get { return _serialNumber; } set { _serialNumber = value; } }

    public string IssuerName { get { return _issuerName; } set { _issuerName = value; } }

    public string SubjectName { get { return _subjectName; } set { _subjectName = value; } }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidUntil { get; set; }

    public string Hash { get { return _hash; } set { _hash = value; } }

    public int[] CertificateErrors { get { return _certificateErrors; } set { _certificateErrors = value; } }

    private enum LdapCertificateProblem
    {
        CertExpired = -2146762495,
        CertCnNoMatch = -2146762481,
        // ReSharper disable once UnusedMember.Local
        CertIssuerChaining = -2146762489,
        CertUntrustedCa = -2146762478,
        // ReSharper disable once UnusedMember.Local
        CertUntrustedRoot = -2146762487,
        CertMalformed = -2146762488,
        CertUnrecognizedError = -2146762477
    }

    public static int[] GetLdapCertProblems(X509Certificate certificate, X509Chain chain,
        SslPolicyErrors sslPolicyErrors, ILogger log = null)
    {
        var certificateErrors = new List<int>();
        try
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return certificateErrors.ToArray();
            }

            var expDate = DateTime.Parse(certificate.GetExpirationDateString()).ToUniversalTime();
            var utcNow = DateTime.UtcNow;
            if (expDate < utcNow && expDate.AddDays(1) >= utcNow)
            {
                certificateErrors.Add((int)LdapCertificateProblem.CertExpired);
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
            {
                certificateErrors.Add((int)LdapCertificateProblem.CertMalformed);
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
            {
                if (log != null)
                {
                    log.WarnGetLdapCertProblems(Enum.GetName(typeof(SslPolicyErrors), LdapCertificateProblem.CertCnNoMatch));
                }

                certificateErrors.Add((int)LdapCertificateProblem.CertCnNoMatch);
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
            {
                if (log != null)
                {
                    log.WarnGetLdapCertProblems(Enum.GetName(typeof(SslPolicyErrors), LdapCertificateProblem.CertCnNoMatch));
                }

                certificateErrors.Add((int)LdapCertificateProblem.CertUntrustedCa);
            }
        }
        catch (Exception ex)
        {
            if (log != null)
            {
                log.ErrorGetLdapCertProblems(ex);
            }

            certificateErrors.Add((int)LdapCertificateProblem.CertUnrecognizedError);
        }

        return certificateErrors.ToArray();
    }

    public static LdapCertificateConfirmRequest FromCert(X509Certificate certificate, X509Chain chain,
        SslPolicyErrors sslPolicyErrors, bool approved = false, bool requested = false, ILogger log = null)
    {
        var certificateErrors = GetLdapCertProblems(certificate, chain, sslPolicyErrors, log);

        try
        {
            string serialNumber = "", issuerName = "", subjectName = "", hash = "";
            DateTime validFrom = DateTime.UtcNow, validUntil = DateTime.UtcNow;

            LdapUtils.SkipErrors(() => serialNumber = certificate.GetSerialNumberString(), log);
            LdapUtils.SkipErrors(() => issuerName = certificate.Issuer, log);
            LdapUtils.SkipErrors(() => subjectName = certificate.Subject, log);
            LdapUtils.SkipErrors(() => validFrom = DateTime.Parse(certificate.GetEffectiveDateString()), log);
            LdapUtils.SkipErrors(() => validUntil = DateTime.Parse(certificate.GetExpirationDateString()), log);
            LdapUtils.SkipErrors(() => hash = certificate.GetCertHashString(), log);

            var certificateConfirmRequest = new LdapCertificateConfirmRequest
            {
                SerialNumber = serialNumber,
                IssuerName = issuerName,
                SubjectName = subjectName,
                ValidFrom = validFrom,
                ValidUntil = validUntil,
                Hash = hash,
                CertificateErrors = certificateErrors,
                Approved = approved,
                Requested = requested
            };

            return certificateConfirmRequest;
        }
        catch (Exception ex)
        {
            if (log != null)
            {
                log.ErrorLdapCertificateConfirmRequest(ex);
            }

            return null;
        }
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
