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


using System.Collections.Generic;

using ASC.Common.Module;
using ASC.Core.Common.Notify.Jabber;

namespace ASC.Core.Notify.Jabber
{
    public class JabberServiceClientWcf : BaseWcfClient<IJabberService>, IJabberService
    {
        public JabberServiceClientWcf()
        {
        }

        public string GetVersion()
        {
            return Channel.GetVersion();
        }

        public byte AddXmppConnection(string connectionId, string userName, byte state, int tenantId)
        {
            return Channel.AddXmppConnection(connectionId, userName, state, tenantId);
        }

        public byte RemoveXmppConnection(string connectionId, string userName, int tenantId)
        {
            return Channel.RemoveXmppConnection(connectionId, userName, tenantId);
        }

        public int GetNewMessagesCount(int tenantId, string userName)
        {
            return Channel.GetNewMessagesCount(tenantId, userName);
        }

        public string GetUserToken(int tenantId, string userName)
        {
            return Channel.GetUserToken(tenantId, userName);
        }

        public void SendCommand(int tenantId, string from, string to, string command, bool fromTenant)
        {
            Channel.SendCommand(tenantId, from, to, command, fromTenant);
        }

        public void SendMessage(int tenantId, string from, string to, string text, string subject)
        {
            Channel.SendMessage(tenantId, from, to, text, subject);
        }

        public byte SendState(int tenantId, string userName, byte state)
        {
            return Channel.SendState(tenantId, userName, state);
        }

        public MessageClass[] GetRecentMessages(int tenantId, string from, string to, int id)
        {
            return Channel.GetRecentMessages(tenantId, from, to, id);
        }

        public Dictionary<string, byte> GetAllStates(int tenantId, string userName)
        {
            return Channel.GetAllStates(tenantId, userName);
        }

        public byte GetState(int tenantId, string userName)
        {
            return Channel.GetState(tenantId, userName);
        }

        public void Ping(string userId, int tenantId, string userName, byte state)
        {
            Channel.Ping(userId, tenantId, userName, state);
        }

        public string HealthCheck(string userName, int tenantId)
        {
            return Channel.HealthCheck(userName, tenantId);
        }
    }
}
