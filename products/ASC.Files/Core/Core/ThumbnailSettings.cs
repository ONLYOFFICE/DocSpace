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

using SixLabors.ImageSharp.Processing;

namespace ASC.Files.ThumbnailBuilder;

[Singletone]
public class ThumbnailSettings
{
    public ThumbnailSettings(ConfigurationExtension configuration)
    {
        configuration.GetSetting("thumbnail", this);
    }

    #region worker settings

    private string _serverRoot;
    public string ServerRoot
    {
        get => _serverRoot ?? "http://localhost/";
        set => _serverRoot = value;
    }

    #endregion


    #region data privider settings

    private string _connectionStringName;
    public string ConnectionStringName
    {
        get => _connectionStringName ?? "default";
        set => _connectionStringName = value;
    }

    private string _formats;
    public string Formats
    {
        get => _formats ?? ".pptx|.pptm|.ppt|.ppsx|.ppsm|.pps|.potx|.potm|.pot|.odp|.fodp|.otp|.gslides|.xlsx|.xlsm|.xls|.xltx|.xltm|.xlt|.ods|.fods|.ots|.gsheet|.csv|.docx|.docxf|.oform|.docm|.doc|.dotx|.dotm|.dot|.odt|.fodt|.ott|.gdoc|.txt|.rtf|.mht|.html|.htm|.fb2|.epub|.pdf|.djvu|.xps|.oxps";
        set => _formats = value;
    }

    private string[] _formatsArray;
    public string[] FormatsArray
    {
        get
        {
            if (_formatsArray != null)
            {
                return _formatsArray;
            }

            _formatsArray = (Formats ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);

            return _formatsArray;
        }
    }

    private int _sqlMaxResults;
    public int SqlMaxResults
    {
        get => _sqlMaxResults != 0 ? _sqlMaxResults : 1000;
        set => _sqlMaxResults = value;
    }

    #endregion


    #region thumbnails generator settings

    private int _maxDegreeOfParallelism;
    public int MaxDegreeOfParallelism
    {
        get => _maxDegreeOfParallelism != 0 ? _maxDegreeOfParallelism : 1;
        set => _maxDegreeOfParallelism = value;
    }

    private long? _maxImageFileSize;
    public long? MaxImageFileSize
    {
        get => _maxImageFileSize ?? 30L * 1024L * 1024L;
        set => _maxImageFileSize = value;
    }

    private long? _maxVideoFileSize;
    public long? MaxVideoFileSize
    {
        get => _maxVideoFileSize ?? 1000L * 1024L * 1024L;
        set => _maxVideoFileSize = value;
    }


    private int? _attemptsLimit;
    public int? AttemptsLimit
    {
        get => _attemptsLimit ?? 3;
        set => _attemptsLimit = value;
    }

    private int _attemptWaitInterval;
    public int AttemptWaitInterval
    {
        get => _attemptWaitInterval != 0 ? _attemptWaitInterval : 1000;
        set => _attemptWaitInterval = value;
    }

    public IEnumerable<ThumbnailSize> Sizes { get; set; }

    #endregion
}

public class ThumbnailSize
{
    public int Height { get; set; }
    public int Width { get; set; }
    public ResizeMode ResizeMode { get; set; } = ResizeMode.Crop;
}