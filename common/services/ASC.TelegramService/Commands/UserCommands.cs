/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.Runtime.Caching;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core.Common.Notify.Telegram;
using ASC.TelegramService.Core;

using Microsoft.Extensions.Options;

namespace ASC.TelegramService.Commands
{
    [Scope]
    public class UserCommands : CommandContext
    {
        private CachedTelegramDao CachedTelegramDao { get; }

        public UserCommands(IOptionsSnapshot<CachedTelegramDao> cachedTelegramDao)
        {
            CachedTelegramDao = cachedTelegramDao.Value;
        }

        [Command("start")]
        public Task StartCommand(string token)
        {
            if (string.IsNullOrEmpty(token)) return Task.CompletedTask;

            return InternalStartCommand(token);
        }

        private async Task InternalStartCommand(string token)
        {
            var user = MemoryCache.Default.Get(token);
            if (user != null)
            {
                MemoryCache.Default.Remove(token);
                MemoryCache.Default.Remove((string)user);
                var split = ((string)user).Split(':');

                var guid = Guid.Parse(split[0]);
                var tenant = int.Parse(split[1]);

                if (tenant == TenantId)
                {
                    CachedTelegramDao.RegisterUser(guid, tenant, Context.User.Id);
                    await ReplyAsync("Ok!");
                    return;
                }
            }

            await ReplyAsync("Error");
        }
    }
}