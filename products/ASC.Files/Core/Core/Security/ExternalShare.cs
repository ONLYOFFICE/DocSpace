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

namespace ASC.Files.Core.Security;

[Scope]
public class ExternalShare
{
    private readonly Global _global;
    private readonly IDaoFactory _daoFactory;
    private readonly CookiesManager _cookiesManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CommonLinkUtility _commonLinkUtility;
    private Guid _linkId;
    private Guid _sessionId;
    private string _passwordKey;

    public ExternalShare(
        Global global, 
        IDaoFactory daoFactory, 
        CookiesManager cookiesManager,
        IHttpContextAccessor httpContextAccessor,
        CommonLinkUtility commonLinkUtility)
    {
        _global = global;
        _daoFactory = daoFactory;
        _cookiesManager = cookiesManager;
        _httpContextAccessor = httpContextAccessor;
        _commonLinkUtility = commonLinkUtility;
    }
    
    public string GetLink(Guid linkId)
    {
        var key = CreateShareKey(linkId);

        return _commonLinkUtility.GetFullAbsolutePath($"rooms/share?key={key}");
    }
    
    public async Task<Status> ValidateAsync(Guid linkId)
    {
        var record = await _daoFactory.GetSecurityDao<int>().GetSharesAsync(new [] { linkId }).FirstOrDefaultAsync();

        return record == null ? Status.Invalid : ValidateRecord(record, null);
    }
    
    public Status ValidateRecord(FileShareRecord record, string password)
    {
        if (record.SubjectType != SubjectType.ExternalLink ||
            record.Subject == FileConstant.ShareLinkId ||
            record.Options == null)
        {
            return Status.Ok;
        }
        
        if (record.Options.IsExpired())
        {
            return Status.Expired;
        }

        if (record.Options.Disabled)
        {
            return Status.Invalid;
        }

        if (string.IsNullOrEmpty(record.Options.Password))
        {
            return Status.Ok;
        }
        
        if (string.IsNullOrEmpty(_passwordKey))
        {
            _passwordKey = _cookiesManager.GetCookies(CookiesType.ShareLink, record.Subject.ToString(), true);
        }

        if (_passwordKey == record.Options.Password)
        {
            return Status.Ok;
        }

        if (string.IsNullOrEmpty(password))
        {
            return Status.RequiredPassword;
        }

        if (CreatePasswordKey(password) == record.Options.Password)
        {
            _cookiesManager.SetCookies(CookiesType.ShareLink, record.Options.Password, true, record.Subject.ToString());
            return Status.Ok;
        }

        _cookiesManager.ClearCookies(CookiesType.ShareLink, record.Subject.ToString());
        
        return Status.InvalidPassword;
    }
    
    public string CreatePasswordKey(string password)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(password);

        return Signature.Create(password, _global.GetDocDbKey());
    }

    public string GetPassword(string passwordKey)
    {
        return string.IsNullOrEmpty(passwordKey) ? null : Signature.Read<string>(passwordKey, _global.GetDocDbKey());
    }

    public string GetKey()
    {
        var key = _httpContextAccessor.HttpContext?.Request.Headers[HttpRequestExtensions.RequestTokenHeader].FirstOrDefault();

        if (string.IsNullOrEmpty(key))
        {
            key = _httpContextAccessor.HttpContext?.Request.Query.GetRequestValue(FilesLinkUtility.FolderShareKey);
        }

        return string.IsNullOrEmpty(key) ? null : key;
    }
    
    public Guid ParseShareKey(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        
        return Signature.Read<Guid>(key, _global.GetDocDbKey());
    }

    public bool TryGetLinkId(out Guid linkId)
    {
        linkId = _linkId;

        if (linkId != default)
        {
            return true;
        }

        var key = GetKey();

        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        var keyLinkId = ParseShareKey(key);

        if (keyLinkId == default)
        {
            return false;
        }

        linkId = _linkId = keyLinkId;

        return true;
    }

    public bool TryGetSessionId(out Guid sessionId)
    {
        sessionId = _sessionId;

        if (sessionId != default)
        {
            return true;
        }

        var sessionKey = _cookiesManager.GetCookies(CookiesType.AnonymousSessionKey);

        if (string.IsNullOrEmpty(sessionKey))
        {
            return false;
        }

        var id = Signature.Read<Guid>(sessionKey, _global.GetDocDbKey());

        if (id == default)
        {
            return false;
        }

        sessionId = _sessionId = id;

        return true;
    }

    public ExternalShareData GetCurrentShareData()
    {
        _ = TryGetLinkId(out var linkId);
        _ = TryGetSessionId(out var sessionId);
        var password = string.IsNullOrEmpty(_passwordKey) ? _cookiesManager.GetCookies(CookiesType.ShareLink, _linkId.ToString(), true) : _passwordKey;

        return new ExternalShareData(linkId, sessionId, password);
    }

    public void SetCurrentShareData(ExternalShareData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        
        if (_linkId == default)
        {
            _linkId = data.LinkId;
        }

        if (_sessionId == default)
        {
            _sessionId = data.SessionId;
        }

        if (string.IsNullOrEmpty(_passwordKey))
        {
            _passwordKey = data.PasswordKey;
        }
    }

    public string CreateDownloadSessionKey()
    {
        _ = TryGetLinkId(out var linkId);
        _ = TryGetSessionId(out var sessionId);
        
        var session = new DownloadSession
        {
            Id = sessionId,
            LinkId = linkId
        };

        return Signature.Create(session, _global.GetDocDbKey());
    }

    public DownloadSession ParseDownloadSessionKey(string sessionKey)
    {
        return Signature.Read<DownloadSession>(sessionKey, _global.GetDocDbKey());
    }
    
    public string GetAnonymousSessionKey()
    {
        return _cookiesManager.GetCookies(CookiesType.AnonymousSessionKey);
    }

    public void SetAnonymousSessionKey()
    {
        _cookiesManager.SetCookies(CookiesType.AnonymousSessionKey, Signature.Create(Guid.NewGuid(), _global.GetDocDbKey()), true);
    }
    
    private string CreateShareKey(Guid linkId)
    {
        return Signature.Create(linkId, _global.GetDocDbKey());
    }
}

public class ValidationInfo
{
    public Status Status { get; set; }
    public string Id { get; set; }
    public string Title { get; set; }
    public FileShare Access { get; set; }
    public FolderType FolderType { get; set; }
    public Logo Logo { get; set; }
    public int TenantId { get; set; }
}

public class ExternalShareData
{
    public Guid LinkId { get; }
    public Guid SessionId { get; }
    public string PasswordKey { get; }
    
    public ExternalShareData(Guid linkId, Guid sessionId, string passwordKey)
    {
        LinkId = linkId;
        SessionId = sessionId;
        PasswordKey = passwordKey;
    }
}

public class DownloadSession
{
    public Guid Id { get; set; }
    public Guid LinkId { get; set; }
}

public enum Status
{
    Ok,
    Invalid,
    Expired,
    RequiredPassword,
    InvalidPassword
}