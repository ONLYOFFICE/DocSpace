// (c) Copyright Ascensio System SIA 2010-2022
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

namespace ASC.Core;

public class Partner
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Url { get; set; }
    public string Phone { get; set; }
    public string Language { get; set; }
    public string CompanyName { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
    public bool CountryHasVat { get; set; }
    public string Address { get; set; }
    public string VatId { get; set; }
    public DateTime CreationDate { get; set; }
    public PartnerStatus Status { get; set; }
    public string Comment { get; set; }
    public string Portal { get; set; }
    public bool PortalConfirmed { get; set; }
    public bool IsAdmin => PartnerType == PartnerType.Administrator;
    public decimal Limit { get; set; }
    public int Discount { get; set; }
    public string PayPalAccount { get; set; }
    public decimal Deposit { get; set; }
    public bool Removed { get; set; }
    public string Currency { get; set; }
    public string LogoUrl { get; set; }
    public string DisplayName { get; set; }
    public PartnerDisplayType DisplayType { get; set; }
    public string SupportPhone { get; set; }
    public string SupportEmail { get; set; }
    public string SalesEmail { get; set; }
    public string TermsUrl { get; set; }
    public string Theme { get; set; }
    public string RuAccount { get; set; }
    public string RuBank { get; set; }
    public string RuKs { get; set; }
    public string RuKpp { get; set; }
    public string RuBik { get; set; }
    public string RuInn { get; set; }
    public PartnerType PartnerType { get; set; }
    public PartnerPaymentMethod PaymentMethod { get; set; }
    public string PaymentUrl { get; set; }
    public decimal AvailableCredit { get; set; }
    public bool CustomEmailSignature { get; set; }
    public string AuthorizedKey { get; set; }

    public override bool Equals(object obj)
    {
        return obj is Partner p && p.Id == Id;
    }

    public override int GetHashCode()
    {
        return (Id ?? string.Empty).GetHashCode();
    }

    public override string ToString()
    {
        return FirstName + " " + LastName;
    }
}
