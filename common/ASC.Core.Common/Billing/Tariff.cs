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
using System.Diagnostics;

using ASC.Core.Tenants;

namespace ASC.Core.Billing
{
    [DebuggerDisplay("{QuotaId} ({State} before {DueDate})")]
    [Serializable]
    public class Tariff
    {
        public int QuotaId { get; set; }
        public TariffState State { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DelayDueDate { get; set; }
        public DateTime LicenseDate { get; set; }
        public bool Autorenewal { get; set; }
        public bool Prolongable { get; set; }
        public int Quantity { get; set; }

        public static Tariff CreateDefault()
        {
            return new Tariff
            {
                QuotaId = Tenant.DefaultTenant,
                State = TariffState.Paid,
                DueDate = DateTime.MaxValue,
                DelayDueDate = DateTime.MaxValue,
                LicenseDate = DateTime.MaxValue,
                Quantity = 1
            };
        }

        public override int GetHashCode() => QuotaId.GetHashCode();

        public override bool Equals(object obj) => obj is Tariff t && t.QuotaId == QuotaId;

        public bool EqualsByParams(Tariff t)
        {
            return t != null
                && t.QuotaId == QuotaId
                && t.DueDate == DueDate
                && t.Quantity == Quantity;
        }
    }
}