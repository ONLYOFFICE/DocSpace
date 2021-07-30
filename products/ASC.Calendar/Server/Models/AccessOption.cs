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


using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ASC.Calendar.Models
{
    [DataContract(Name = "sharingOption", Namespace = "")]
    public class AccessOption
    {
        [DataMember(Name = "id", Order = 10)]
        public string Id { get; set; }

        [DataMember(Name = "name", Order = 20)]
        public string Name { get; set; }

        [DataMember(Name = "defaultAction", Order = 30)]
        public bool Default { get; set; }

        [DataMember(Name = "defaultStyle", Order = 40)]
        public string DefaultStyle { get; set; }

        public static AccessOption ReadOption
        {
            get { return new AccessOption() { Id = "read", Default = true, DefaultStyle = "read", Name= Resources.CalendarApiResource.ReadOption }; }
        }

        public static AccessOption FullAccessOption
        {
            get { return new AccessOption() { Id = "full_access", DefaultStyle = "full", Name = Resources.CalendarApiResource.FullAccessOption }; }
        }

        public static AccessOption OwnerOption
        {
            get { return new AccessOption() { Id = "owner", Name = Resources.CalendarApiResource.OwnerOption }; }
        }


        public static List<AccessOption> CalendarStandartOptions {
            get {
                 return new List<AccessOption>(){ReadOption, FullAccessOption};
            }
        }

        public static AccessOption GetSample()
        {
            return new AccessOption { Id = "read", Name = "Read only", Default = true };
        }
    }
}
