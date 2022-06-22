﻿// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Common.Radicale;

public class CardDavItem
{
    public Guid ID { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserName { get; set; }

    public DateTime? BirthDate { get; set; }

    public bool? Sex { get; set; }

    public string Title { get; set; }

    public string Email { get; set; }

    public List<string> Contacts { get; set; }

    public string MobilePhone { get; set; }

    public override string ToString()
    {
        return String.Format("{0} {1}", FirstName, LastName).Trim();
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }

    public CardDavItem(Guid iD, string firstName, string lastName, string userName, DateTime? birthDate, bool? sex, string title, string email, List<string> contacts, string mobilePhone)
    {
        ID = iD;
        FirstName = firstName;
        LastName = lastName;
        UserName = userName;
        BirthDate = birthDate;
        Sex = sex;
        Title = title;
        Email = email;
        Contacts = contacts;
        MobilePhone = mobilePhone;
    }
}
