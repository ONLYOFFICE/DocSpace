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


using System;
using System.Diagnostics;
using System.Linq;

namespace ASC.Core.Tenants
{
    [DebuggerDisplay("{Name}")]
    public class TenantQuota : ICloneable
    {
        public static readonly TenantQuota Default = new TenantQuota(Tenant.DEFAULT_TENANT)
        {
            Name = "Default",
            MaxFileSize = 25 * 1024 * 1024, // 25Mb
            MaxTotalSize = long.MaxValue,
            ActiveUsers = int.MaxValue,
        };

        public int Id { get; set; }

        public string Name { get; set; }

        public long MaxFileSize { get; set; }

        public long MaxTotalSize { get; set; }

        public int ActiveUsers { get; set; }

        public string Features { get; set; }

        public decimal Price { get; set; }

        public string AvangateId { get; set; }

        public bool Visible { get; set; }

        public bool Year
        {
            get { return GetFeature("year"); }
            set { SetFeature("year", value); }
        }

        public bool Year3
        {
            get { return GetFeature("year3"); }
            set { SetFeature("year3", value); }
        }

        public bool NonProfit
        {
            get { return GetFeature("non-profit"); }
            set { SetFeature("non-profit", value); }
        }

        public bool Trial
        {
            get { return GetFeature("trial"); }
            set { SetFeature("trial", value); }
        }

        public bool Free
        {
            get { return GetFeature("free"); }
            set { SetFeature("free", value); }
        }

        public bool Open
        {
            get { return GetFeature("open"); }
            set { SetFeature("open", value); }
        }

        public bool ControlPanel
        {
            get { return GetFeature("controlpanel"); }
            set { SetFeature("controlpanel", value); }
        }

        public bool Update
        {
            get { return GetFeature("update"); }
            set { SetFeature("update", value); }
        }

        public bool Support
        {
            get { return GetFeature("support"); }
            set { SetFeature("support", value); }
        }

        public bool Audit
        {
            get { return GetFeature("audit"); }
            set { SetFeature("audit", value); }
        }

        public bool DocsEdition
        {
            get { return GetFeature("docs"); }
            set { SetFeature("docs", value); }
        }

        public bool HasDomain
        {
            get { return GetFeature("domain"); }
            set { SetFeature("domain", value); }
        }

        public bool HealthCheck
        {
            get { return GetFeature("healthcheck"); }
            set { SetFeature("healthcheck", value); }
        }

        public bool HasMigration
        {
            get { return GetFeature("migration"); }
            set { SetFeature("migration", value); }
        }

        public bool Ldap
        {
            get { return GetFeature("ldap"); }
            set { SetFeature("ldap", value); }
        }

        public bool Sso
        {
            get { return GetFeature("sso"); }
            set { SetFeature("sso", value); }
        }

        public bool Branding
        {
            get { return GetFeature("branding"); }
            set { SetFeature("branding", value); }
        }

        public bool SSBranding
        {
            get { return GetFeature("ssbranding"); }
            set { SetFeature("ssbranding", value); }
        }

        public bool WhiteLabel
        {
            get { return GetFeature("whitelabel"); }
            set { SetFeature("whitelabel", value); }
        }

        public bool Customization
        {
            get { return GetFeature("customization"); }
            set { SetFeature("customization", value); }
        }

        public bool DiscEncryption
        {
            get { return GetFeature("discencryption"); }
            set { SetFeature("discencryption", value); }
        }

        public bool PrivacyRoom
        {
            get { return GetFeature("privacyroom"); }
            set { SetFeature("privacyroom", value); }
        }

        public bool EnableMailServer
        {
            get { return GetFeature("mailserver"); }
            set { SetFeature("mailserver", value); }
        }

        public int CountAdmin
        {
            get
            {
                var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
                var admin = features.FirstOrDefault(f => f.StartsWith("admin:"));
                int countAdmin;
                if (admin == null || !int.TryParse(admin.Replace("admin:", ""), out countAdmin))
                {
                    countAdmin = int.MaxValue;
                }
                return countAdmin;
            }
            set
            {
                var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
                var admin = features.FirstOrDefault(f => f.StartsWith("admin:"));
                features.Remove(admin);
                if (value > 0)
                {
                    features.Add("admin:" + value);
                }
                Features = string.Join(",", features.ToArray());
            }
        }

        public bool Restore
        {
            get { return GetFeature("restore"); }
            set { SetFeature("restore", value); }
        }

        public bool AutoBackup
        {
            get { return GetFeature("autobackup"); }
            set { SetFeature("autobackup", value); }
        }

        public bool Oauth
        {
            get { return GetFeature("oauth"); }
            set { SetFeature("oauth", value); }
        }

        public bool ContentSearch
        {
            get { return GetFeature("contentsearch"); }
            set { SetFeature("contentsearch", value); }
        }


        public int CountPortals
        {
            get
            {
                var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
                var portals = features.FirstOrDefault(f => f.StartsWith("portals:"));
                if (portals == null || !int.TryParse(portals.Replace("portals:", ""), out var countPortals) || countPortals <= 0)
                {
                    countPortals = 0;
                }
                return countPortals;
            }
            set
            {
                var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
                var portals = features.FirstOrDefault(f => f.StartsWith("portals:"));
                features.Remove(portals);
                if (value > 0)
                {
                    features.Add("portals:" + value);
                }
                Features = string.Join(",", features.ToArray());
            }
        }

        public bool ThirdParty
        {
            get { return GetFeature("thirdparty"); }
            set { SetFeature("thirdparty", value); }
        }

        public TenantQuota()
        {
        }
        public TenantQuota(int tenant)
        {
            Id = tenant;
        }


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is TenantQuota q && q.Id == Id;
        }


        public bool GetFeature(string feature)
        {
            return !string.IsNullOrEmpty(Features) && Features.Split(' ', ',', ';').Contains(feature);
        }

        internal void SetFeature(string feature, bool set)
        {
            var features = (Features == null
                                ? new string[] { }
                                : Features.Split(' ', ',', ';')).ToList();
            if (set && !features.Contains(feature))
            {
                features.Add(feature);
            }
            else if (!set && features.Contains(feature))
            {
                features.Remove(feature);
            }
            Features = string.Join(",", features.ToArray());
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}