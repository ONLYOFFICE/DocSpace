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

    private List<string> _featuresList;

    public string Features
    {
        get
        {
            return string.Join(",", _featuresList);
        }
        set
        {
            if (value != null)
            {
                _featuresList = value.Split(' ', ',', ';').ToList();
            }
            else
            {
                _featuresList = new List<string>();
            }
        }
    }
    public decimal Price { get; set; }
    public string ProductId { get; set; }
    public bool Visible { get; set; }
    public long MaxFileSize { get; set; }
    public long MaxTotalSize
    {
        get => ByteConverter.GetInBytes(GetFeature("total_size", () => Default.MaxTotalSize));
        set => SetFeature("users", ByteConverter.GetInMBytes(value));
    }

    public int ActiveUsers
    {
        get => GetFeature("users", () => Default.ActiveUsers);
        set => SetFeature("users", value);
    }

    public int CountAdmin
    {
        get => GetFeature("admin", () => Default.CountAdmin);
        set => SetFeature("admin", value);
    }

    public int CountRoom
    {
        get => GetFeature("room", () => Default.CountRoom);
        set => SetFeature("room", value);
    }

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

    public TenantQuota()
    {
        _featuresList = new List<string>();
    }

    public TenantQuota(int tenant) : this()
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

    private bool GetFeature(string feature)
    {
        return _featuresList.Contains(feature);
    }

    private int GetFeature(string feature, Func<int> @default)
    {
        var featureValue = GetFeatureValue(feature);

        if (featureValue == null || !int.TryParse(featureValue, out var result))
        {
            return @default();
        }

        return result;
    }

    private long GetFeature(string feature, Func<long> @default)
    {
        var featureValue = GetFeatureValue(feature);

        if (featureValue == null || !long.TryParse(featureValue, out var result))
        {
            return @default();
        }

        return result;
    }

    private string GetFeatureValue(string feature)
    {
        var parsed = _featuresList.FirstOrDefault(f => f.StartsWith($"{feature}:"));

        if (parsed == null)
        {
            return null;
        }

        return parsed.Replace($"{feature}:", "");
    }

    private void SetFeature(string feature, bool set)
    {
        if (set && !_featuresList.Contains(feature))
        {
            _featuresList.Add(feature);
        }
        else if (!set && _featuresList.Contains(feature))
        {
            _featuresList.Remove(feature);
        }
    }

    private void SetFeature(string feature, int @value)
    {
        SetFeature(feature, @value > 0 ? value.ToString() : null);
    }

    private void SetFeature(string feature, long @value)
    {
        SetFeature(feature, @value > 0 ? value.ToString() : null);
    }

    internal void SetFeature(string feature, string @value)
    {
        var featureValue = _featuresList.FirstOrDefault(f => f.StartsWith($"{feature}:"));
        _featuresList.Remove(featureValue);

        if (!string.IsNullOrEmpty(@value))
        {
            _featuresList.Add($"{feature}:{@value}");
        }
    }

    public static TenantQuota operator *(TenantQuota quota, int quantity)
    {
        var newQuota = (TenantQuota)quota.Clone();

        newQuota.Price *= quantity;

        if (newQuota.MaxTotalSize != long.MaxValue)
        {
            newQuota.MaxTotalSize *= quantity;
        }

        if (newQuota.ActiveUsers != int.MaxValue)
        {
            newQuota.ActiveUsers *= quantity;
        }

        if (newQuota.CountAdmin != int.MaxValue)
        {
            newQuota.CountAdmin *= quantity;
        }

        if (newQuota.CountRoom != int.MaxValue)
        {
            newQuota.CountRoom *= quantity;
        }

        return newQuota;
    }

    public static TenantQuota operator +(TenantQuota old, TenantQuota quota)
    {
        if (old == null)
        {
            return quota;
        }

        var newQuota = (TenantQuota)old.Clone();
        newQuota.Name = "";
        newQuota.MaxFileSize = Math.Max(newQuota.MaxFileSize, quota.MaxFileSize);
        newQuota.Price += quota.Price;
        newQuota.Visible &= quota.Visible;
        newQuota.ProductId = "";

        var features = newQuota._featuresList.Concat(quota._featuresList).ToList();

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
                            if (int.TryParse(features[i].Replace(pref1 + ":", ""), out var val1) &&
                                int.TryParse(features[j].Replace(pref1 + ":", ""), out var val2))
                            {
                                features[i] = $"{pref1}:{val1 + val2}";
                                features.RemoveAt(j);
                                j--;
                            }
                            else if (long.TryParse(features[i].Replace(pref1 + ":", ""), out var val3) &&
                                     long.TryParse(features[j].Replace(pref1 + ":", ""), out var val4))
                            {
                                features[i] = $"{pref1}:{val3 + val4}";
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

        newQuota._featuresList = features;

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
