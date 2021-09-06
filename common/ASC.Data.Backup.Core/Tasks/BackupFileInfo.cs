/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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


using System;
using System.Xml.Linq;

using ASC.Data.Backup.Extensions;

namespace ASC.Data.Backup.Tasks
{
    public class BackupFileInfo
    {
        public string Domain { get; set; }
        public string Module { get; set; }
        public string Path { get; set; }
        public int Tenant { get; set; }

        public BackupFileInfo()
        {
        }

        public BackupFileInfo(string domain, string module, string path, int tenant = -1)
        {
            Domain = domain;
            Module = module;
            Path = path;
            Tenant = tenant;
        }

        public XElement ToXElement()
        {
            var xElement = new XElement("file",
                                new XElement("domain", Domain),
                                new XElement("module", Module),
                                new XElement("path", Path));

            if (Tenant != -1)
            {
                xElement.Add(new XElement("tenant", Tenant));
            }

            return xElement;
        }

        public static BackupFileInfo FromXElement(XElement el)
        {
            return new BackupFileInfo
            {
                Domain = el.Element("domain").ValueOrDefault(),
                Module = el.Element("module").ValueOrDefault(),
                Path = el.Element("path").ValueOrDefault(),
                Tenant = Convert.ToInt32(el.Element("tenant").ValueOrDefault() ?? "-1")
            };
        }
    }
}
