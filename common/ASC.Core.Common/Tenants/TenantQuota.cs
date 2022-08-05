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

namespace ASC.Core.Tenants;

[DebuggerDisplay("{Tenant} {Name}")]
public class TenantQuota : ICloneable, IMapFrom<DbQuota>
{
    public static readonly TenantQuota Default = new TenantQuota(Tenants.Tenant.DefaultTenant)
    {
        Name = "Default",
        MaxFileSize = 25 * 1024 * 1024, // 25Mb
        MaxTotalSize = long.MaxValue,
        ActiveUsers = int.MaxValue,
        CountAdmin = int.MaxValue,
        CountRoom = int.MaxValue
    };

    public int Tenant { get; set; }
    public string Name { get; set; }
    public long MaxFileSize { get; set; }
    public long MaxTotalSize
    {
        get
        {
            var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
            var totalSize = features.FirstOrDefault(f => f.StartsWith("total_size:"));
            long maxTotalSize;
            if (totalSize == null || !long.TryParse(totalSize.Replace("total_size:", ""), out maxTotalSize))
            {
                maxTotalSize = Default.MaxTotalSize;
            }

            return ByteConverter.GetInBytes(maxTotalSize);
        }
        set
        {
            var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
            var maxTotalSize = features.FirstOrDefault(f => f.StartsWith("total_size:"));
            features.Remove(maxTotalSize);
            if (value > 0)
            {
                features.Add("total_size:" + ByteConverter.GetInMBytes(value));
            }

            Features = string.Join(",", features.ToArray());
        }
    }

    public int ActiveUsers
    {
        get
        {
            var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
            var users = features.FirstOrDefault(f => f.StartsWith("users:"));
            int activeUsers;
            if (users == null || !int.TryParse(users.Replace("users:", ""), out activeUsers))
            {
                activeUsers = Default.ActiveUsers;
            }

            return activeUsers;
        }
        set
        {
            var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
            var activeUsers = features.FirstOrDefault(f => f.StartsWith("users:"));
            features.Remove(activeUsers);
            if (value > 0)
            {
                features.Add("users:" + value);
            }

            Features = string.Join(",", features.ToArray());
        }
    }

    public string Features { get; set; }
    public decimal Price { get; set; }
    public string ProductId { get; set; }
    public bool Visible { get; set; }

    public bool NonProfit
    {
        get => GetFeature("non-profit");
        set => SetFeature("non-profit", value);
    }

    public bool Trial
    {
        get => GetFeature("trial");
        set => SetFeature("trial", value);
    }

    public bool Free
    {
        get => GetFeature("free");
        set => SetFeature("free", value);
    }

    public bool Open
    {
        get => GetFeature("open");
        set => SetFeature("open", value);
    }

    public bool Update
    {
        get => GetFeature("update");
        set => SetFeature("update", value);
    }

    public bool Support
    {
        get => GetFeature("support");
        set => SetFeature("support", value);
    }

    public bool Audit
    {
        get => GetFeature("audit");
        set => SetFeature("audit", value);
    }

    public bool DocsEdition
    {
        get => GetFeature("docs");
        set => SetFeature("docs", value);
    }

    public bool HasMigration
    {
        get => GetFeature("migration");
        set => SetFeature("migration", value);
    }

    public bool Ldap
    {
        get => GetFeature("ldap");
        set => SetFeature("ldap", value);
    }

    public bool Sso
    {
        get => GetFeature("sso");
        set => SetFeature("sso", value);
    }

    public bool WhiteLabel
    {
        get => GetFeature("whitelabel");
        set => SetFeature("whitelabel", value);
    }

    public bool Customization
    {
        get => GetFeature("customization");
        set => SetFeature("customization", value);
    }

    public bool Custom
    {
        get => GetFeature("custom");
        set => SetFeature("custom", value);
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
                countAdmin = Default.CountAdmin;
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

    public int CountRoom
    {
        get
        {
            var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
            var room = features.FirstOrDefault(f => f.StartsWith("room:"));
            int countRoom;
            if (room == null || !int.TryParse(room.Replace("room:", ""), out countRoom))
            {
                countRoom = Default.CountRoom;
            }

            return countRoom;
        }
        set
        {
            var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
            var room = features.FirstOrDefault(f => f.StartsWith("room:"));
            features.Remove(room);
            if (value > 0)
            {
                features.Add("room:" + value);
            }

            Features = string.Join(",", features.ToArray());
        }
    }

    public bool Restore
    {
        get => GetFeature("restore");
        set => SetFeature("restore", value);
    }

    public bool AutoBackup
    {
        get => GetFeature("autobackup");
        set => SetFeature("autobackup", value);
    }

    public bool Oauth
    {
        get => GetFeature("oauth");
        set => SetFeature("oauth", value);
    }

    public bool ContentSearch
    {
        get => GetFeature("contentsearch");
        set => SetFeature("contentsearch", value);
    }

    public bool ThirdParty
    {
        get => GetFeature("thirdparty");
        set => SetFeature("thirdparty", value);
    }

    public TenantQuota() { }

    public TenantQuota(int tenant)
    {
        Tenant = tenant;
    }

    public override int GetHashCode()
    {
        return Tenant.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is TenantQuota q && q.Tenant == Tenant;
    }

    public bool GetFeature(string feature)
    {
        return !string.IsNullOrEmpty(Features) && Features.Split(' ', ',', ';').Contains(feature);
    }

    internal void SetFeature(string feature, bool set)
    {
        var features = (Features == null
                            ? Array.Empty<string>()
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

    public TenantQuota Concat(TenantQuota quota)
    {
        var newQuota = (TenantQuota)this.Clone();
        newQuota.Name = "";
        newQuota.MaxFileSize = Math.Max(newQuota.MaxFileSize, quota.MaxFileSize);
        newQuota.Price += quota.Price;
        newQuota.Visible &= quota.Visible;
        newQuota.ProductId = "";

        newQuota.Features = ((newQuota.Features ?? "") + "," + quota.Features).Trim(',');
        var features = newQuota.Features.Split(' ', ',', ';').ToList();
        for (var i = 0; i < features.Count - 1; i++)
        {
            for (var j = i + 1; j < features.Count; j++)
            {
                if (features[i].Contains(':'))
                {
                    if (features[j].Contains(':'))
                    {
                        var pref1 = features[i].Split(':')[0];
                        var pref2 = features[j].Split(':')[0];
                        if (pref1 == pref2)
                        {
                            int val1;
                            int val2;
                            if (int.TryParse(features[i].Replace(pref1 + ":", ""), out val1)
                                && int.TryParse(features[j].Replace(pref1 + ":", ""), out val2))
                            {
                                features[i] = pref1 + ":" + (val1 + val2);
                                features.RemoveAt(j);
                                j--;
                            }
                        }
                    }
                }
                else if (features[i] == features[j])
                {
                    features.RemoveAt(j);
                    j--;
                }
            }
        }
        newQuota.Features = string.Join(",", features.ToArray());

        return newQuota;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbQuota, TenantQuota>()
            .ForMember(dest => dest.MaxFileSize, opt => opt.MapFrom(src => ByteConverter.GetInBytes(src.MaxFileSize)));
    }
}
