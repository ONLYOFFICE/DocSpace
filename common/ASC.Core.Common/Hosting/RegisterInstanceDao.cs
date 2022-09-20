﻿// (c) Copyright Ascensio System SIA 2010-2022
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

using ASC.Notify;

namespace ASC.Core.Common.Hosting;

[Scope]
public class RegisterInstanceDao<T> : IRegisterInstanceDao<T> where T : IHostedService
{
    private readonly ILogger _logger;
    private readonly IDbContextFactory<InstanceRegistrationContext> _dbContextFactory;

    public RegisterInstanceDao(
        ILogger<RegisterInstanceDao<T>> logger,
        IDbContextFactory<InstanceRegistrationContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddOrUpdate(InstanceRegistration obj)
    {
        using var _instanceRegistrationContext = _dbContextFactory.CreateDbContext();
        var inst = _instanceRegistrationContext.InstanceRegistrations.Find(obj.InstanceRegistrationId);

        if (inst == null)
        {
            await _instanceRegistrationContext.AddAsync(obj);
        }
        else
        {
            _instanceRegistrationContext.Entry(inst).CurrentValues.SetValues(obj);
        }

        bool saveFailed;

        do
        {
            saveFailed = false;

            try
            {
                await _instanceRegistrationContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.TraceDbUpdateConcurrencyException(obj.InstanceRegistrationId, DateTimeOffset.Now);

                saveFailed = true;

                var entry = ex.Entries.Single();

                if (entry.State == EntityState.Modified)
                {
                    entry.State = EntityState.Added;
                }
            }
        }
        while (saveFailed);
    }

    public async Task<IEnumerable<InstanceRegistration>> GetAll()
    {
        using var _instanceRegistrationContext = _dbContextFactory.CreateDbContext();
        return await _instanceRegistrationContext.InstanceRegistrations
                                                .Where(x => x.WorkerTypeName == typeof(T).GetFormattedName())
                                                .ToListAsync();
    }

    public async Task Delete(string instanceId)
    {
        using var _instanceRegistrationContext = _dbContextFactory.CreateDbContext();
        var item = _instanceRegistrationContext.InstanceRegistrations.Find(instanceId);

        if (item == null)
        {
            return;
        }

        _instanceRegistrationContext.InstanceRegistrations.Remove(item);

        try
        {
            await _instanceRegistrationContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Detached;
            }
        }
    }

}
