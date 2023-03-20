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

public class GwsMigratingMail : MigratingMail
{
    private int _messagesCount;
    private readonly string _rootFolder;
    private readonly GwsMigratingUser _user;
    private readonly List<GwsMail> _mails = new List<GwsMail>();

    public override int MessagesCount => _messagesCount;
    public override string ModuleName => MigrationResource.ModuleNameMail;

    public GwsMigratingMail(string rootFolder, GwsMigratingUser user, Action<string, Exception> log) : base(log)
    {
        _rootFolder = rootFolder;
        _user = user;
    }

    public override Task MigrateAsync()
    {
        throw new NotImplementedException();
    }

    public override void Parse()
    {
        var path = Path.Combine(_rootFolder, "Mail");
        var foldersName = Directory.GetFiles(path);
        foreach (var item in foldersName)
        {
            var mail = new GwsMail();
            var messagesList = new List<MimeMessage>();
            using (var sr = File.OpenRead(item))
            {
                var parser = new MimeParser(sr, MimeFormat.Mbox);
                while (!parser.IsEndOfStream)
                {
                    messagesList.Add(parser.ParseMessage());
                    _messagesCount++;
                }
            }
            var folder = item.Split(Path.DirectorySeparatorChar);
            mail.ParentFolder = folder[folder.Length - 1].Split('.')[0].ToLower() == "All mail Including Spam and Trash".ToLower() ? "inbox" : folder[folder.Length - 1].Split('.')[0];
            mail.Message = messagesList;
            _mails.Add(mail);
        }
        throw new NotImplementedException();
    }
}
