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

namespace ASC.Core;

[Serializable]
public class AzRecord : IMapFrom<Acl>
{
    public Guid Subject { get; set; }
    public Guid Action { get; set; }
    public string Object { get; set; }
    public AceType AceType { get; set; }
    public int Tenant { get; set; }

    public AzRecord() { }

    public AzRecord(Guid subjectId, Guid actionId, AceType reaction)
        : this(subjectId, actionId, reaction, default(string))
    {
    }

    public AzRecord(Guid subjectId, Guid actionId, AceType reaction, string fullId)
    {
        Subject = subjectId;
        Action = actionId;
        AceType = reaction;
        Object = fullId;
    }

    public static implicit operator AzRecord(AzRecordCache cache)
    {
        var result = new AzRecord()
        {
            Tenant = cache.Tenant
        };


        if (Guid.TryParse(cache.SubjectId, out var subjectId))
        {
            result.Subject = subjectId;
        }

        if (Guid.TryParse(cache.ActionId, out var actionId))
        {
            result.Action = actionId;
        }

        result.Object = cache.ObjectId;

        if (Enum.TryParse<AceType>(cache.Reaction, out var reaction))
        {
            result.AceType = reaction;
        }

        return result;
    }

    public static implicit operator AzRecordCache(AzRecord cache)
    {
        return new AzRecordCache
        {
            SubjectId = cache.Subject.ToString(),
            ActionId = cache.Action.ToString(),
            ObjectId = cache.Object,
            Reaction = cache.AceType.ToString(),
            Tenant = cache.Tenant
        };
    }

    public override bool Equals(object obj)
    {
        return obj is AzRecord r &&
            r.Tenant == Tenant &&
            r.Subject == Subject &&
            r.Action == Action &&
            r.Object == Object &&
            r.AceType == AceType;
    }

    public override int GetHashCode()
    {
        return Tenant.GetHashCode() ^
            Subject.GetHashCode() ^
            Action.GetHashCode() ^
            (Object ?? string.Empty).GetHashCode() ^
            AceType.GetHashCode();
    }
}
