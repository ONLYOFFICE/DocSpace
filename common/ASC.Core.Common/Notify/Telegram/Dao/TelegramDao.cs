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

public class ConfigureTelegramDaoService : IConfigureNamedOptions<TelegramDao>
{
    private readonly DbContextManager<TelegramDbContext> _dbContextManager;

    public ConfigureTelegramDaoService(DbContextManager<TelegramDbContext> dbContextManager)
    {
        _dbContextManager = dbContextManager;
    }

    public void Configure(string name, TelegramDao options)
    {
        Configure(options);
        options.TelegramDbContext = _dbContextManager.Get(name);
    }

    public void Configure(TelegramDao options)
    {
        options.TelegramDbContext = _dbContextManager.Value;
    }
}

[Scope(typeof(ConfigureTelegramDaoService))]
public class TelegramDao
{
    public TelegramDbContext TelegramDbContext { get; set; }
    public TelegramDao() { }

    public TelegramDao(DbContextManager<TelegramDbContext> dbContextManager)
    {
        TelegramDbContext = dbContextManager.Value;
    }

    public void RegisterUser(Guid userId, int tenantId, long telegramId)
    {
        var user = new TelegramUser
        {
            PortalUserId = userId,
            TenantId = tenantId,
            TelegramUserId = telegramId
        };

        TelegramDbContext.AddOrUpdate(r => r.Users, user);
        TelegramDbContext.SaveChanges();
    }

    public TelegramUser GetUser(Guid userId, long tenantId)
    {
        return TelegramDbContext.Users
            .AsNoTracking()
            .Where(r => r.PortalUserId == userId)
            .Where(r => r.TenantId == tenantId)
            .FirstOrDefault();
    }

    public List<TelegramUser> GetUser(long telegramId)
    {
        return TelegramDbContext.Users
            .AsNoTracking()
            .Where(r => r.TelegramUserId == telegramId)
            .ToList();
    }

    public void Delete(Guid userId, int tenantId)
    {
        var toRemove = TelegramDbContext.Users
            .Where(r => r.PortalUserId == userId)
            .Where(r => r.TenantId == tenantId)
            .ToList();

        TelegramDbContext.Users.RemoveRange(toRemove);
        TelegramDbContext.SaveChanges();
    }

    public void Delete(long telegramId)
    {
        var toRemove = TelegramDbContext.Users
            .Where(r => r.TelegramUserId == telegramId)
            .ToList();

        TelegramDbContext.Users.RemoveRange(toRemove);
        TelegramDbContext.SaveChanges();
    }
}
