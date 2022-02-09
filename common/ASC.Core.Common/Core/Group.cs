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

namespace ASC.Core
{
    public class Group : IMapFrom<DbGroup>
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public bool Removed { get; set; }
        public DateTime LastModified { get; set; }
        public int Tenant { get; set; }
        public string Sid { get; set; }

        public override string ToString() => Name;

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object obj) => obj is Group g && g.Id == Id;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DbGroup, Group>()
                .ForMember(src => src.CategoryId, opt => opt.NullSubstitute(Guid.Empty))
                .ForMember(src => src.ParentId, opt => opt.NullSubstitute(Guid.Empty));

            profile.CreateMap<GroupInfo, Group>();
        }
    }
}