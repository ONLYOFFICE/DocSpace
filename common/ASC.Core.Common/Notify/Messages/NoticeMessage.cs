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

namespace ASC.Notify.Messages;

public class NoticeMessage : INoticeMessage
{
    [NonSerialized]
    private readonly List<ITagValue> _arguments = new List<ITagValue>();

    [NonSerialized]
    private IPattern _pattern;

    public NoticeMessage() { }

    public NoticeMessage(IDirectRecipient recipient, INotifyAction action, string objectID)
    {
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        Action = action;
        ObjectID = objectID;
    }

    public NoticeMessage(IDirectRecipient recipient, INotifyAction action, string objectID, IPattern pattern)
    {
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        Action = action;
        Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        ObjectID = objectID;
        ContentType = pattern.ContentType;
    }

    public NoticeMessage(IDirectRecipient recipient, string subject, string body, string contentType)
    {
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        Subject = subject;
        Body = body ?? throw new ArgumentNullException(nameof(body));
        ContentType = contentType;
    }

    public string ObjectID { get; private set; }

    public IDirectRecipient Recipient { get; private set; }

    public IPattern Pattern
    {
        get => _pattern;
        internal set => _pattern = value;
    }

    public INotifyAction Action { get; private set; }

    public ITagValue[] Arguments => _arguments.ToArray();

    public void AddArgument(params ITagValue[] tagValues)
    {
        ArgumentNullException.ThrowIfNull(tagValues);

        Array.ForEach(tagValues,
            tagValue =>
            {
                if (!_arguments.Exists(tv => Equals(tv.Tag, tagValue.Tag)))
                {
                    _arguments.Add(tagValue);
                }
            });
    }

    public ITagValue GetArgument(string tag)
    {
        return _arguments.Find(r => r.Tag == tag);
    }

    public string Subject { get; set; }
    public string Body { get; set; }
    public string ContentType { get; internal set; }
}
