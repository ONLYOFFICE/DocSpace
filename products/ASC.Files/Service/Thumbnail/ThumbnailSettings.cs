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


using System;

using ASC.Common;
using ASC.Common.Utils;

namespace ASC.Files.ThumbnailBuilder
{
    [Singletone]
    public class ThumbnailSettings
    {
        public ThumbnailSettings(ConfigurationExtension configuration)
        {
            configuration.GetSetting("thumbnail", this);
        }

        #region worker settings

        private string serverRoot;
        public string ServerRoot { get => serverRoot ?? "http://localhost/"; set { serverRoot = value; } }

        private int launchFrequency;
        public int LaunchFrequency { get => launchFrequency != 0 ? launchFrequency : 1; set { launchFrequency = value; } }

        #endregion


        #region data privider settings

        private string connectionStringName;
        public string ConnectionStringName { get => connectionStringName ?? "default"; set { connectionStringName = value; } }

        private string formats;
        public string Formats { get => formats ?? ".pptx|.pptm|.ppt|.ppsx|.ppsm|.pps|.potx|.potm|.pot|.odp|.fodp|.otp|.gslides|.xlsx|.xlsm|.xls|.xltx|.xltm|.xlt|.ods|.fods|.ots|.gsheet|.csv|.docx|.docxf|.oform|.docm|.doc|.dotx|.dotm|.dot|.odt|.fodt|.ott|.gdoc|.txt|.rtf|.mht|.html|.htm|.fb2|.epub|.pdf|.djvu|.xps|.bmp|.jpeg|.jpg|.png|.gif|.tiff|.tif|.ico"; set { formats = value; } }

        private string[] formatsArray;

        public string[] FormatsArray
        {
            get
            {
                if (formatsArray != null)
                {
                    return formatsArray;
                }
                formatsArray = (Formats ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);
                return formatsArray;
            }
        }

        private int sqlMaxResults;
        public int SqlMaxResults { get => sqlMaxResults != 0 ? sqlMaxResults : 1000; set { sqlMaxResults = value; } }

        #endregion


        #region thumbnails generator settings

        private int maxDegreeOfParallelism;
        public int MaxDegreeOfParallelism { get => maxDegreeOfParallelism != 0 ? maxDegreeOfParallelism : 10; set { maxDegreeOfParallelism = value; } }

        private long? availableFileSize;
        public long? AvailableFileSize { get => availableFileSize ?? 100L * 1024L * 1024L; set { availableFileSize = value; } }

        private int? attemptsLimit;
        public int? AttemptsLimit { get => attemptsLimit ?? 3; set { attemptsLimit = value; } }

        private int attemptWaitInterval;
        public int AttemptWaitInterval { get => attemptWaitInterval != 0 ? attemptWaitInterval : 1000; set { attemptWaitInterval = value; } }

        private int thumbnaillHeight;
        public int ThumbnaillHeight { get => thumbnaillHeight != 0 ? thumbnaillHeight : 128; set { thumbnaillHeight = value; } }

        private int thumbnaillWidth;
        public int ThumbnaillWidth { get => thumbnaillWidth != 0 ? thumbnaillWidth : 192; set { thumbnaillWidth = value; } }

        #endregion
    }
}