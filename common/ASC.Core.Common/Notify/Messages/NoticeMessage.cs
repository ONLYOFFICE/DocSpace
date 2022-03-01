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

namespace ASC.Notify.Messages;

[Serializable]
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
        if (tagValues == null) throw new ArgumentNullException(nameof(tagValues));
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
