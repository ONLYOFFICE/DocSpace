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

namespace ASC.Migration.GoogleWorkspace.Models;

public class GwsMigratingCalendar : MigratingCalendar
{
    public override int CalendarsCount => _calendars.Count;

    public override int EventsCount => _calendars.Values.SelectMany(c => c.SelectMany(x => x.Events)).Count();
    public override string ModuleName => MigrationResource.ModuleNameCalendar;

    public override void Parse()
    {
        //var calPath = Path.Combine(rootFolder, "Calendar");
        //if (!Directory.Exists(calPath)) return;

        //var calFiles = Directory.GetFiles(calPath, "*.ics");
        //if (!calFiles.Any()) return;

        //foreach (var calFile in calFiles)
        //{
        //    var events = DDayICalParser.DeserializeCalendar(File.ReadAllText(calFile));
        //    calendars.Add(Path.GetFileNameWithoutExtension(calFile), events);
        //}
    }

    public override Task MigrateAsync()
    {
        if (!ShouldImport)
        {
            return Task.CompletedTask;
        }

        // \portals\module\ASC.Api\ASC.Api.Calendar\CalendarApi.cs#L2311
        throw new NotImplementedException();
    }

    public GwsMigratingCalendar(string rootFolder, Action<string, Exception> log) : base(log)
    {
        _rootFolder = rootFolder;
    }

    private readonly string _rootFolder;
    private readonly Dictionary<string, CalendarCollection> _calendars = new Dictionary<string, CalendarCollection>();
}
