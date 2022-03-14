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


using System;
using System.Collections.Generic;

namespace ASC.Feed.Aggregator
{
    public class Feed
    {
        public Feed()
        {
        }

        public Feed(Guid author, DateTime date, bool variate = false)
        {
            AuthorId = author;
            ModifiedBy = author;

            CreatedDate = date;
            ModifiedDate = date;

            Action = FeedAction.Created;
            Variate = variate;
        }

        public string Item { get; set; }

        public string ItemId { get; set; }

        public string Id
        {
            get { return $"{Item}_{ItemId}"; }
        }

        public Guid AuthorId { get; private set; }

        public Guid ModifiedBy { get; set; }

        public DateTime CreatedDate { get; private set; }

        public DateTime ModifiedDate { get; set; }

        public string Product { get; set; }

        public string Module { get; set; }

        public string ExtraLocation { get; set; }

        public string ExtraLocationUrl { get; set; }

        public FeedAction Action { get; set; }

        public string Title { get; set; }

        public string ItemUrl { get; set; }

        public string Description { get; set; }

        public string AdditionalInfo { get; set; }

        public string AdditionalInfo2 { get; set; }

        public string AdditionalInfo3 { get; set; }

        public string AdditionalInfo4 { get; set; }

        public bool HasPreview { get; set; }

        public bool CanComment { get; set; }

        public object Target { get; set; }

        public string CommentApiUrl { get; set; }

        public IEnumerable<FeedComment> Comments { get; set; }


        // это значит, что новость может обновляться (пр. добавление комментариев);
        // следовательно и права доступа могут устаревать
        public bool Variate { get; private set; }

        public string GroupId { get; set; }

        public string Keywords { get; set; }
    }
}