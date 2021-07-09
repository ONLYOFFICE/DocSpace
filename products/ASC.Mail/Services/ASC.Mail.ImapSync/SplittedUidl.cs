/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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


using ASC.Mail.Enums;
using MailKit;

namespace ASC.Mail.ImapSync
{
    public class SplittedUidl
    {
        public UniqueId UniqueId { get; }
        public FolderType FolderType { get; }

        public string Uidl { get; }

        public SplittedUidl(string uidl)
        {
            if (string.IsNullOrEmpty(uidl)) return;

            if (!uint.TryParse(uidl.Split('-')[0], out uint uidlInt))
            {
                return;
            }

            if (!uint.TryParse(uidl.Split('-')[1], out uint folderTypeInt))
            {
                return;
            }

            Uidl = uidl;

            UniqueId = new UniqueId(uidlInt);

            FolderType = (FolderType)folderTypeInt;
        }

        public SplittedUidl(FolderType folder, UniqueId uniqueId)
        {
            if (uniqueId == null) return;

            UniqueId = uniqueId;

            FolderType = folder;

            Uidl = UniqueId.ToString() + "-" + ((int)FolderType).ToString();
        }

        public static string ToUidl(FolderType folder, UniqueId uniqueId)
        {
            return $"{uniqueId.Id}-{(int)folder}";
        }
    }
}