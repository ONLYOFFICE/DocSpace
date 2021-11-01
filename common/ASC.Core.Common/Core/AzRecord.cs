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

using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.Core
{
    [Serializable]
    public class AzRecord
    {
        public Guid SubjectId { get; set; }

        public Guid ActionId { get; set; }

        public string ObjectId { get; set; }

        public AceType Reaction { get; set; }

        public int Tenant { get; set; }


        public AzRecord()
        {
        }

        public AzRecord(Guid subjectId, Guid actionId, AceType reaction)
            : this(subjectId, actionId, reaction, default(string))
        {
        }

        public AzRecord(Guid subjectId, Guid actionId, AceType reaction, ISecurityObjectId objectId)
            : this(subjectId, actionId, reaction, AzObjectIdHelper.GetFullObjectId(objectId))
        {
        }


        internal AzRecord(Guid subjectId, Guid actionId, AceType reaction, string objectId)
        {
            SubjectId = subjectId;
            ActionId = actionId;
            Reaction = reaction;
            ObjectId = objectId;
        }


        public static implicit operator AzRecord(AzRecordCache cache)
        {
            var result = new AzRecord()
            {
                Tenant = cache.Tenant
            };


            if (Guid.TryParse(cache.SubjectId, out var subjectId))
            {
                result.SubjectId = subjectId;
            }

            if (Guid.TryParse(cache.ActionId, out var actionId))
            {
                result.ActionId = actionId;
            }

            result.ObjectId = cache.ObjectId;

            if (Enum.TryParse<AceType>(cache.Reaction, out var reaction))
            {
                result.Reaction = reaction;
            }

            return result;
        }

        public static implicit operator AzRecordCache(AzRecord cache)
        {
            return new AzRecordCache
            {
                SubjectId = cache.SubjectId.ToString(),
                ActionId = cache.ActionId.ToString(),
                ObjectId = cache.ObjectId,
                Reaction = cache.Reaction.ToString(),
                Tenant = cache.Tenant
            };
        }

        public override bool Equals(object obj)
        {
            return obj is AzRecord r &&
                r.Tenant == Tenant &&
                r.SubjectId == SubjectId &&
                r.ActionId == ActionId &&
                r.ObjectId == ObjectId &&
                r.Reaction == Reaction;
        }

        public override int GetHashCode()
        {
            return Tenant.GetHashCode() ^
                SubjectId.GetHashCode() ^
                ActionId.GetHashCode() ^
                (ObjectId ?? string.Empty).GetHashCode() ^
                Reaction.GetHashCode();
        }
    }
}
