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

namespace ASC.Core.Common.Notify.Telegram;

class ConfigureCachedTelegramDao : IConfigureNamedOptions<CachedTelegramDao>
{
    private readonly IOptionsSnapshot<TelegramDao> _service;
    private readonly ICache _cache;

    public ConfigureCachedTelegramDao(IOptionsSnapshot<TelegramDao> service, ICache cache)
    {
        _service = service;
        _cache = cache;
    }

    public void Configure(string name, CachedTelegramDao options)
    {
        Configure(options);
        options.TgDao = _service.Get(name);
    }

    public void Configure(CachedTelegramDao options)
    {
        options.TgDao = _service.Value;
        options.Cache = _cache;
        options.Expiration = TimeSpan.FromMinutes(20);

        options.PairKeyFormat = "tgUser:{0}:{1}";
        options.SingleKeyFormat = "tgUser:{0}";
    }
}

[Scope(typeof(ConfigureCachedTelegramDao))]
public class CachedTelegramDao
{
    public TelegramDao TgDao { get; set; }
    public ICache Cache { get; set; }
    public TimeSpan Expiration { get; set; }
    public string PairKeyFormat { get; set; }
    public string SingleKeyFormat { get; set; }


    public void Delete(Guid userId, int tenantId)
    {
        Cache.Remove(string.Format(PairKeyFormat, userId, tenantId));
        TgDao.Delete(userId, tenantId);
    }

    public void Delete(int telegramId)
    {
        Cache.Remove(string.Format(SingleKeyFormat, telegramId));
        TgDao.Delete(telegramId);
    }

    public TelegramUser GetUser(Guid userId, int tenantId)
    {
        var key = string.Format(PairKeyFormat, userId, tenantId);

        var user = Cache.Get<TelegramUser>(key);
        if (user != null)
        {
            return user;
        }

        user = TgDao.GetUser(userId, tenantId);
        if (user != null)
        {
            Cache.Insert(key, user, Expiration);
        }

        return user;
    }

    public List<TelegramUser> GetUser(int telegramId)
    {
        var key = string.Format(SingleKeyFormat, telegramId);

        var users = Cache.Get<List<TelegramUser>>(key);
        if (users != null)
        {
            return users;
        }

        users = TgDao.GetUser(telegramId);
        if (users.Count > 0)
        {
            Cache.Insert(key, users, Expiration);
        }

        return users;
    }

    public void RegisterUser(Guid userId, int tenantId, long telegramId)
    {
        TgDao.RegisterUser(userId, tenantId, telegramId);

        var key = string.Format(PairKeyFormat, userId, tenantId);
        Cache.Insert(key, new TelegramUser { PortalUserId = userId, TenantId = tenantId, TelegramUserId = telegramId }, Expiration);
    }
}
