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

namespace ASC.Web.Core;
[Scope]
public class BruteForceLoginManager
{
    private readonly LoginSettings _settings;
    private readonly ICache _cache;
    private string _login;
    private string _requestIp;

    public BruteForceLoginManager(SettingsManager settingsManager, ICache cache)
    {
        _settings = settingsManager.Load<LoginSettings>();
        _cache = cache;
    }

    public void Init(string login, string requestIp)
    {
        _login = login;
        _requestIp = requestIp;
    }

    private string GetBlockCacheKey()
    {
        return "loginblock/" + _login + _requestIp;
    }

    private string GetHistoryCacheKey()
    {
        return "loginsec/" + _login + _requestIp;
    }

    public bool Increment(out bool showRecaptcha)
    {
        showRecaptcha = true;

        var blockCacheKey = GetBlockCacheKey();

        if (_cache.Get<string>(blockCacheKey) != null)
        {
            return false;
        }

        var historyCacheKey = GetHistoryCacheKey();

        var history = _cache.Get<List<DateTime>>(historyCacheKey) ?? new List<DateTime>();

        var now = DateTime.UtcNow;

        var checkTime = now.Subtract(TimeSpan.FromSeconds(_settings.CheckPeriod));

        history = history.Where(item => item > checkTime).ToList();

        history.Add(now);

        showRecaptcha = history.Count > _settings.AttemptCount - 1;

        if (history.Count > _settings.AttemptCount)
        {
            _cache.Insert(blockCacheKey, "block", now.Add(TimeSpan.FromSeconds(_settings.BlockTime)));
            _cache.Remove(historyCacheKey);
            return false;
        }

        _cache.Insert(historyCacheKey, history, now.Add(TimeSpan.FromSeconds(_settings.CheckPeriod)));
        return true;
    }

    public void Decrement()
    {
        var historyCacheKey = GetHistoryCacheKey();

        var history = _cache.Get<List<DateTime>>(historyCacheKey) ?? new List<DateTime>();

        if (history.Count > 0)
        {
            history.RemoveAt(history.Count - 1);
        }

        _cache.Insert(historyCacheKey, history, DateTime.UtcNow.Add(TimeSpan.FromSeconds(_settings.CheckPeriod)));
    }
}
