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
using ASC.Core;
using ASC.Web.Core.Calendars;
using ASC.Common;
using System.Text.Json.Serialization;

namespace ASC.Calendar.Models
{
    [DataContract(Name = "sharing", Namespace = "")]
    [Scope]
    public class PublicItemCollection
    {
        [JsonPropertyName("actions")]
        public List<AccessOption> AvailableOptions { get; set; }

        [DataMember(Name = "items", Order = 20)]
        public List<PublicItemWrapper> Items { get; set; }


        public static PublicItemCollection GetSample()
        {
            return new PublicItemCollection { AvailableOptions = new List<AccessOption>() { AccessOption.GetSample() }, Items = new List<PublicItemWrapper>() { PublicItemWrapper.GetSample() } };
        }

    }

    [Scope]
    public class PublicItemCollectionHelper
    {
        public AuthContext AuthContext { get; }
        public PublicItemWrapperHelper PublicItemWrapperHelper { get; }

        public PublicItemCollectionHelper(
            AuthContext authContext,
            PublicItemWrapperHelper publicItemWrapperHelper)
        {
            AuthContext = authContext;
            PublicItemWrapperHelper = publicItemWrapperHelper;
        }

        public PublicItemCollection GetDefault()
        {
            var sharingOptions = GetPublicItemCollection();
            sharingOptions.Items.Add(PublicItemWrapperHelper.Get(
                new ASC.Web.Core.Calendars.SharingOptions.PublicItem()
                {
                    Id = AuthContext.CurrentAccount.ID,
                    IsGroup = false
                },
            "0", AuthContext.CurrentAccount.ID));
            return sharingOptions;
        }

        public  PublicItemCollection GetForCalendar(ICalendar calendar)
        {
            var sharingOptions = GetPublicItemCollection();
            sharingOptions.Items.Add(PublicItemWrapperHelper.Get(new ASC.Web.Core.Calendars.SharingOptions.PublicItem()
            {
                Id = calendar.OwnerId,
                IsGroup = false
            },
                  calendar.Id, calendar.OwnerId));

            foreach (var item in calendar.SharingOptions.PublicItems)
                sharingOptions.Items.Add(PublicItemWrapperHelper.Get(item, calendar.Id, calendar.OwnerId));

            return sharingOptions;
        }
        public PublicItemCollection GetForEvent(IEvent calendarEvent)
        {
            var sharingOptions = GetPublicItemCollection();
            sharingOptions.Items.Add(PublicItemWrapperHelper.Get(new ASC.Web.Core.Calendars.SharingOptions.PublicItem()
            {
                Id = calendarEvent.OwnerId,
                IsGroup = false
            },

            calendarEvent.CalendarId, calendarEvent.Id, calendarEvent.OwnerId));

            foreach (var item in calendarEvent.SharingOptions.PublicItems)
                sharingOptions.Items.Add(PublicItemWrapperHelper.Get(item, calendarEvent.CalendarId, calendarEvent.Id, calendarEvent.OwnerId));

            return sharingOptions;
        }

        public PublicItemCollection GetPublicItemCollection()
        {
            var publicItemCollection = new PublicItemCollection();

            publicItemCollection.Items = new List<PublicItemWrapper>();

            publicItemCollection.AvailableOptions = AccessOption.CalendarStandartOptions;

            return publicItemCollection;
        }
    }
   
}
