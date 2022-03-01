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

namespace ASC.ElasticSearch.Service;

[Singletone]
public class Settings
{
    public string Host
    {
        get => _host ?? "localhost";
        set => _host = value;
    }
    public int? Port
    {
        get => _port ?? 9200;
        set => _port = value;
    }
    public string Scheme
    {
        get => _scheme ?? "http";
        set => _scheme = value;
    }
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

    private string _host;
    private int? _port;
    private string _scheme;
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
