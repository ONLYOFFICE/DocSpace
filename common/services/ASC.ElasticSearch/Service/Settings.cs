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

namespace ASC.ElasticSearch.Service;

[Singletone]
public class Settings
{
    public string Host { get; set; }
    public int? Port { get; set; }
    public string Scheme { get; set; }

    public int? Period
    {
        get => _period ?? 1;
        set => _period = value;
    }
    public long? MaxContentLength
    {
        get => _maxContentLength ?? 100 * 1024 * 1024L;
        set => _maxContentLength = value;
    }
    public long? MaxFileSize
    {
        get => _maxFileSize ?? 10 * 1024 * 1024L;
        set => _maxFileSize = value;
    }
    public int? Threads
    {
        get => _threads ?? 1;
        set => _threads = value;
    }
    public bool? HttpCompression
    {
        get => _httpCompression ?? true;
        set => _httpCompression = value;
    }

    public Authentication Authentication { get; set; }

    public ApiKey ApiKey { get; set; }

    private int? _period;
    private long? _maxContentLength;
    private long? _maxFileSize;
    private int? _threads;
    private bool? _httpCompression;

    public Settings(ConfigurationExtension configuration)
    {
        configuration.GetSetting("elastic", this);
    }
}

public class Authentication
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class ApiKey
{
    public string Id { get; set; }
    public string Value { get; set; }
}

