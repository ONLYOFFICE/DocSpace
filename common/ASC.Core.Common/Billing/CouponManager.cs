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

using ConfigurationManager = System.Configuration.ConfigurationManager;
using ConfigurationSection = System.Configuration.ConfigurationSection;

namespace ASC.Core.Common.Billing;

[Singletone]
public class CouponManager
{
    private IEnumerable<AvangateProduct> _products;
    private readonly IHttpClientFactory _clientFactory;
    private IEnumerable<string> _groups;
    private readonly int _percent;
    private readonly int _schedule;
    private readonly string _vendorCode;
    private readonly byte[] _secret;
    private readonly Uri _baseAddress;
    private readonly string _apiVersion;
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly ILog _logger;

    public CouponManager(ILog logger, IHttpClientFactory clientFactory)
    {
        _semaphoreSlim = new SemaphoreSlim(1, 1);
        _logger = logger;
        _clientFactory = clientFactory;

        try
        {
            var cfg = (AvangateCfgSectionHandler)ConfigurationManager.GetSection("avangate");
            _secret = Encoding.UTF8.GetBytes(cfg.Secret);
            _vendorCode = cfg.Vendor;
            _percent = cfg.Percent;
            _schedule = cfg.Schedule;
            _baseAddress = new Uri(cfg.BaseAddress);
            _apiVersion = "/rest/" + cfg.ApiVersion.TrimStart('/');
            _groups = (cfg.Groups ?? "").Split(',', '|', ' ');
        }
        catch (Exception e)
        {
            _secret = Encoding.UTF8.GetBytes("");
            _vendorCode = "";
            _percent = AvangateCfgSectionHandler.DefaultPercent;
            _schedule = AvangateCfgSectionHandler.DefaultShedule;
            _baseAddress = new Uri(AvangateCfgSectionHandler.DefaultAdress);
            _apiVersion = AvangateCfgSectionHandler.DefaultApiVersion;
            _groups = new List<string>();
            _logger.Fatal(e);
        }
    }

    public string CreateCoupon(TenantManager tenantManager)
    {
        return CreatePromotionAsync(tenantManager).Result;
    }

    private async Task<string> CreatePromotionAsync(TenantManager tenantManager)
    {
        try
        {
            using var httpClient = PrepaireClient();
            using var content = new StringContent(await Promotion.GeneratePromotion(_logger, this, tenantManager, _percent, _schedule), Encoding.Default, "application/json");
            using var response = await httpClient.PostAsync($"{_apiVersion}/promotions/", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var result = await response.Content.ReadAsStringAsync();
            await Task.Delay(1000 - DateTime.UtcNow.Millisecond); // otherwise authorize exception
            var createdPromotion = JsonConvert.DeserializeObject<Promotion>(result);
            return createdPromotion.Coupon.Code;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
            throw;
        }
    }

    internal Task<IEnumerable<AvangateProduct>> GetProducts()
    {
        return _products != null ? Task.FromResult(_products) : InternalGetProducts();
    }

    private async Task<IEnumerable<AvangateProduct>> InternalGetProducts()
    {
        await _semaphoreSlim.WaitAsync();

        if (_products != null)
        {
            _semaphoreSlim.Release();

            return _products;
        }

        try
        {
            using var httpClient = PrepaireClient();
            using var response = await httpClient.GetAsync($"{_apiVersion}/products/?Limit=1000&Enabled=true");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var result = await response.Content.ReadAsStringAsync();
            _logger.Debug(result);

            var products = JsonConvert.DeserializeObject<List<AvangateProduct>>(result);
            products = products.Where(r => r.ProductGroup != null && _groups.Contains(r.ProductGroup.Code)).ToList();

            return _products = products;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);

            throw;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private HttpClient PrepaireClient()
    {
        const string applicationJson = "application/json";

        var httpClient = _clientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        httpClient.Timeout = TimeSpan.FromMinutes(3);

        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", applicationJson);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", applicationJson);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Avangate-Authentication", CreateAuthHeader());

        return httpClient;
    }

    private string CreateAuthHeader()
    {
        using var hmac = new HMACMD5(_secret);
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        var hash = _vendorCode.Length + _vendorCode + date.Length + date;
        var data = hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));

        var sBuilder = new StringBuilder();
        foreach (var t in data)
        {
            sBuilder.Append(t.ToString("x2"));
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"code='{_vendorCode}' ");
        stringBuilder.Append($"date='{date}' ");
        stringBuilder.Append($"hash='{sBuilder}'");

        return stringBuilder.ToString();
    }
}

class Promotion
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public bool Enabled { get; set; }
    public int MaximumOrdersNumber { get; set; }
    public bool InstantDiscount { get; set; }
    public string ChannelType { get; set; }
    public string ApplyRecurring { get; set; }
    public Coupon Coupon { get; set; }
    public Discount Discount { get; set; }
    public IEnumerable<CouponProduct> Products { get; set; }
    public int PublishToAffiliatesNetwork { get; set; }
    public int AutoApply { get; set; }

    public static async Task<string> GeneratePromotion(ILog log, CouponManager couponManager, TenantManager tenantManager, int percent, int schedule)
    {
        try
        {
            var tenant = tenantManager.GetCurrentTenant();
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(schedule);
            var code = tenant.Alias;

            var promotion = new Promotion
            {
                Type = "REGULAR",
                Enabled = true,
                MaximumOrdersNumber = 1,
                InstantDiscount = false,
                ChannelType = "ECOMMERCE",
                ApplyRecurring = "NONE",
                PublishToAffiliatesNetwork = 0,
                AutoApply = 0,

                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
                Name = string.Format("{0} {1}% off", code, percent),
                Coupon = new Coupon { Type = "SINGLE", Code = code },
                Discount = new Discount { Type = "PERCENT", Value = percent },
                Products = (await couponManager.GetProducts()).Select(r => new CouponProduct { Code = r.ProductCode })

            };

            return JsonConvert.SerializeObject(promotion);
        }
        catch (Exception ex)
        {
            log.Error(ex.Message, ex);

            throw;
        }
    }
}

class Coupon
{
    public string Type { get; set; }
    public string Code { get; set; }
}

class Discount
{
    public string Type { get; set; }
    public int Value { get; set; }
}

class AvangateProduct
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public AvangateProductGroup ProductGroup { get; set; }
}

class AvangateProductGroup
{
    public string Name { get; set; }
    public string Code { get; set; }
}

class CouponProduct
{
    public string Code { get; set; }
}

class AvangateCfgSectionHandler : ConfigurationSection
{
    public const string DefaultAdress = "https://api.avangate.com/";
    public const string DefaultApiVersion = "4.0";
    public const int DefaultPercent = 5;
    public const int DefaultShedule = 10;

    [ConfigurationProperty("secret")]
    public string Secret => (string)this["secret"];

    [ConfigurationProperty("vendor")]
    public string Vendor
    {
        get => (string)this["vendor"];
        set => this["vendor"] = value;
    }

    [ConfigurationProperty("percent", DefaultValue = DefaultPercent)]
    public int Percent
    {
        get => Convert.ToInt32(this["percent"]);
        set => this["percent"] = value;
    }

    [ConfigurationProperty("schedule", DefaultValue = DefaultShedule)]
    public int Schedule
    {
        get => Convert.ToInt32(this["schedule"]);
        set => this["schedule"] = value;
    }

    [ConfigurationProperty("groups")]
    public string Groups => (string)this["groups"];

    [ConfigurationProperty("address", DefaultValue = DefaultAdress)]
    public string BaseAddress
    {
        get => (string)this["address"];
        set => this["address"] = value;
    }

    [ConfigurationProperty("apiVersion", DefaultValue = DefaultApiVersion)]
    public string ApiVersion
    {
        get => (string)this["apiVersion"];
        set => this["apiVersion"] = value;
    }
}
