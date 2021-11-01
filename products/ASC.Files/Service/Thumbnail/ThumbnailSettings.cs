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
        public static ThumbnailSettings GetInstance(ConfigurationExtension configuration)
        {
            var result = new ThumbnailSettings();
            var cfg = configuration.GetSetting<ThumbnailSettings>("thumbnail");
            result.ServerRoot = cfg.ServerRoot ?? "http://localhost/";
            result.LaunchFrequency = cfg.LaunchFrequency != 0 ? cfg.LaunchFrequency : 1;
            result.ConnectionStringName = cfg.ConnectionStringName ?? "default";
            result.Formats = cfg.Formats ?? ".pptx|.pptm|.ppt|.ppsx|.ppsm|.pps|.potx|.potm|.pot|.odp|.fodp|.otp|.gslides|.xlsx|.xlsm|.xls|.xltx|.xltm|.xlt|.ods|.fods|.ots|.gsheet|.csv|.docx|.docm|.doc|.dotx|.dotm|.dot|.odt|.fodt|.ott|.gdoc|.txt|.rtf|.mht|.html|.htm|.fb2|.epub|.pdf|.djvu|.xps|.bmp|.jpeg|.jpg|.png|.gif|.tiff|.tif|.ico";
            result.SqlMaxResults = cfg.SqlMaxResults != 0 ? cfg.SqlMaxResults : 1000;
            result.MaxDegreeOfParallelism = cfg.MaxDegreeOfParallelism != 0 ? cfg.MaxDegreeOfParallelism : 10;
            result.AvailableFileSize = cfg.AvailableFileSize ?? 100L * 1024L * 1024L;
            result.AttemptsLimit = cfg.AttemptsLimit ?? 3;
            result.AttemptWaitInterval = cfg.AttemptWaitInterval != 0 ? cfg.AttemptWaitInterval : 1000;
            result.ThumbnaillHeight = cfg.ThumbnaillHeight != 0 ? cfg.ThumbnaillHeight : 128;
            result.ThumbnaillWidth = cfg.ThumbnaillWidth != 0 ? cfg.ThumbnaillWidth : 192;
            return result;
        }

        #region worker settings

        public string ServerRoot { get; set; }

        public int LaunchFrequency { get; set; }

        #endregion


        #region data privider settings

        public string ConnectionStringName { get; set; }
        public string Formats { get; set; }

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

        public int SqlMaxResults { get; set; }

        #endregion


        #region thumbnails generator settings

        public int MaxDegreeOfParallelism { get; set; }

        public long? AvailableFileSize { get; set; }

        public int? AttemptsLimit { get; set; }

        public int AttemptWaitInterval { get; set; }

        public int ThumbnaillHeight { get; set; }

        public int ThumbnaillWidth { get; set; }

        #endregion
    }
}