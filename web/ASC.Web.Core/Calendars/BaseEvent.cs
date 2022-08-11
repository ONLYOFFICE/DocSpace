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

namespace ASC.Web.Core.Calendars;

public abstract class BaseEvent : IEvent, ICloneable
{
    public virtual TimeZoneInfo TimeZone { get; set; }

    protected BaseEvent()
    {
        this.Context = new EventContext();
        this.AlertType = EventAlertType.Never;
        this.SharingOptions = new SharingOptions();
        this.RecurrenceRule = new RecurrenceRule();
    }

    #region IEvent Members

    public SharingOptions SharingOptions { get; set; }

    public virtual EventAlertType AlertType { get; set; }

    public virtual bool AllDayLong { get; set; }

    public virtual string CalendarId { get; set; }

    public virtual string Description { get; set; }

    public virtual string Id { get; set; }

    public virtual string Uid { get; set; }

    public virtual string Name { get; set; }

    public virtual Guid OwnerId { get; set; }

    public virtual DateTime UtcEndDate { get; set; }

    public virtual DateTime UtcStartDate { get; set; }

    public virtual DateTime UtcUpdateDate { get; set; }

    public virtual EventContext Context { get; set; }

    public virtual RecurrenceRule RecurrenceRule { get; set; }

    public virtual EventStatus Status { get; set; }

    public virtual bool HasAttachments { get; set; }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
        var e = (BaseEvent)this.MemberwiseClone();
        e.Context = (EventContext)this.Context.Clone();
        e.RecurrenceRule = (RecurrenceRule)this.RecurrenceRule.Clone();
        e.SharingOptions = (SharingOptions)this.SharingOptions.Clone();
        return e;
    }

    #endregion


    #region IiCalFormatView Members

    public virtual string ToiCalFormat()
    {
        var sb = new StringBuilder();

        sb.AppendLine("BEGIN:VEVENT");

        var id = string.IsNullOrEmpty(Uid) ? Id : Uid;

        sb.AppendLine($"UID:{id}");
        sb.AppendLine($"SUMMARY:{Name}");

        if (!string.IsNullOrEmpty(this.Description))
        {
            sb.AppendLine($"DESCRIPTION:{Description.Replace("\n", "\\n")}");
        }

        if (this.AllDayLong)
        {
            DateTime startDate = this.UtcStartDate, endDate = this.UtcEndDate;
            if (this.TimeZone != null)
            {
                if (this.UtcStartDate != DateTime.MinValue && startDate.Kind == DateTimeKind.Utc)
                {
                    startDate = startDate.Add(TimeZone.GetOffset());
                }

                if (this.UtcEndDate != DateTime.MinValue && endDate.Kind == DateTimeKind.Utc)
                {
                    endDate = endDate.Add(TimeZone.GetOffset());
                }
            }

            if (this.UtcStartDate != DateTime.MinValue)
            {
                var start = startDate.ToString("yyyyMMdd");
                sb.AppendLine($"DTSTART;VALUE=DATE:{start}");
            }

            if (this.UtcEndDate != DateTime.MinValue)
            {
                var end = endDate.AddDays(1).ToString("yyyyMMdd");
                sb.AppendLine($"DTEND;VALUE=DATE:{end}");
            }
        }
        else
        {
            if (this.UtcStartDate != DateTime.MinValue)
            {
                var utcStart = UtcStartDate.ToString("yyyyMMdd'T'HHmmss'Z'");
                sb.AppendLine($"DTSTART:{utcStart}");
            }

            if (this.UtcEndDate != DateTime.MinValue)
            {
                var utcEnd = UtcEndDate.ToString("yyyyMMdd'T'HHmmss'Z'");
                sb.AppendLine($"DTEND:{utcEnd}");
            }
        }


        if (this.RecurrenceRule != null)
        {
            sb.AppendLine(this.RecurrenceRule.ToiCalFormat());
        }

        sb.Append("END:VEVENT");
        return sb.ToString();
    }

    #endregion

}
