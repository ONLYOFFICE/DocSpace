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

    public void RegisterUser(Guid userId, int tenantId, int telegramId)
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

    public TelegramUser GetUser(Guid userId, int tenantId)
    {
        return TelegramDbContext.Users
            .AsNoTracking()
            .Where(r => r.PortalUserId == userId)
            .Where(r => r.TenantId == tenantId)
            .FirstOrDefault();
    }

    public List<TelegramUser> GetUser(int telegramId)
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

    public void Delete(int telegramId)
    {
        var toRemove = TelegramDbContext.Users
            .Where(r => r.TelegramUserId == telegramId)
            .ToList();

        TelegramDbContext.Users.RemoveRange(toRemove);
        TelegramDbContext.SaveChanges();
    }
}
