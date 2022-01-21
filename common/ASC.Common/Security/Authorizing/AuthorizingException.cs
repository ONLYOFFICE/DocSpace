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


#region usings

using System;
using System.Runtime.Serialization;
using System.Text;

#endregion

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public class AuthorizingException : Exception
    {
        private readonly string _Message;

        public AuthorizingException(string message)
            : base(message)
        {
        }

        public AuthorizingException(ISubject subject, IAction[] actions)
        {
            if (actions == null || actions.Length == 0) throw new ArgumentNullException(nameof(actions));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Actions = actions;
            var sactions = "";
            Array.ForEach(actions, action => { sactions += action.ToString() + ", "; });
            _Message = string.Format(
                "\"{0}\" access denied \"{1}\"",
                subject,
                sactions
                );
        }

        public AuthorizingException(ISubject subject, IAction[] actions, ISubject[] denySubjects, IAction[] denyActions)
        {
            _Message = FormatErrorMessage(subject, actions, denySubjects, denyActions);
        }

        protected AuthorizingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _Message = info.GetValue("_Message", typeof(string)) as string;
            Subject = info.GetValue("Subject", typeof(ISubject)) as ISubject;
            Actions = info.GetValue("Actions", typeof(IAction[])) as IAction[];
        }

        public override string Message
        {
            get { return _Message; }
        }

        public ISubject Subject { get; internal set; }
        public IAction[] Actions { get; internal set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Subject", Subject, typeof(ISubject));
            info.AddValue("_Message", _Message, typeof(string));
            info.AddValue("Actions", Actions, typeof(IAction[]));
            base.GetObjectData(info, context);
        }

        internal static string FormatErrorMessage(ISubject subject, IAction[] actions, ISubject[] denySubjects,
                                                  IAction[] denyActions)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));
            if (actions == null || actions.Length == 0) throw new ArgumentNullException(nameof(actions));
            if (denySubjects == null || denySubjects.Length == 0) throw new ArgumentNullException(nameof(denySubjects));
            if (denyActions == null || denyActions.Length == 0) throw new ArgumentNullException(nameof(denyActions));
            if (actions.Length != denySubjects.Length || actions.Length != denyActions.Length)
                throw new ArgumentException();

            var sb = new StringBuilder();
            for (var i = 0; i < actions.Length; i++)
            {
                var action = actions[i];
                var denyAction = denyActions[i];
                var denySubject = denySubjects[i];

                string reason;
                if (denySubject != null && denyAction != null)
                    reason = $"{action.Name}:{(denySubject is IRole ? "role:" : "") + denySubject.Name} access denied {denyAction.Name}.";
                else
                    reason = $"{action.Name}: access denied.";
                if (i != actions.Length - 1)
                    reason += ", ";
                 sb.Append(reason);
            }
            var reasons = sb.ToString();
            var sactions = "";
            Array.ForEach(actions, action => { sactions += action.ToString() + ", "; });
            var message = $"\"{(subject is IRole ? "role:" : "") + subject.Name}\" access denied \"{sactions}\". Cause: {reasons}.";
            return message;
        }
    }
}