/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

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

    private int _launchFrequency;
    public int LaunchFrequency
    {
        get => _launchFrequency != 0 ? _launchFrequency : 1;
        set => _launchFrequency = value;
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
        get => _formats ?? ".pptx|.pptm|.ppt|.ppsx|.ppsm|.pps|.potx|.potm|.pot|.odp|.fodp|.otp|.gslides|.xlsx|.xlsm|.xls|.xltx|.xltm|.xlt|.ods|.fods|.ots|.gsheet|.csv|.docx|.docxf|.oform|.docm|.doc|.dotx|.dotm|.dot|.odt|.fodt|.ott|.gdoc|.txt|.rtf|.mht|.html|.htm|.fb2|.epub|.pdf|.djvu|.xps|.bmp|.jpeg|.jpg|.png|.gif|.tiff|.tif|.ico";
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
        get => _maxDegreeOfParallelism != 0 ? _maxDegreeOfParallelism : 10;
        set => _maxDegreeOfParallelism = value;
    }

    private long? _availableFileSize;
    public long? AvailableFileSize
    {
        get => _availableFileSize ?? 100L * 1024L * 1024L;
        set => _availableFileSize = value;
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

    private int _thumbnaillHeight;
    public int ThumbnaillHeight
    {
        get => _thumbnaillHeight != 0 ? _thumbnaillHeight : 128;
        set => _thumbnaillHeight = value;
    }

    private int _thumbnaillWidth;
    public int ThumbnaillWidth
    {
        get => _thumbnaillWidth != 0 ? _thumbnaillWidth : 192;
        set => _thumbnaillWidth = value;
    }

    #endregion
}
