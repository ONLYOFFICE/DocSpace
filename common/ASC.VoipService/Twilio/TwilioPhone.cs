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

using HttpMethod = Twilio.Http.HttpMethod;

namespace ASC.VoipService.Twilio;

public class TwilioPhone : VoipPhone
{
    private readonly TwilioRestClient _twilio;
    public TwilioPhone(
        TwilioRestClient twilio,
        AuthContext authContext,
        TenantUtil tenantUtil,
        SecurityContext securityContext,
        BaseCommonLinkUtility baseCommonLinkUtility) :
        base(authContext, tenantUtil, securityContext, baseCommonLinkUtility)
    {
        _twilio = twilio;
        Settings = new TwilioVoipSettings(authContext, tenantUtil, securityContext, baseCommonLinkUtility);
    }

    #region Calls

    public override VoipCall Call(string to, string contactId = null)
    {
        var number = to.Split('#');

        var call = CallResource.Create(new CreateCallOptions(new PhoneNumber("+" + number[0].TrimStart('+')), new PhoneNumber("+" + Number.TrimStart('+')))
        {
            SendDigits = number.Length > 1 ? number[1] + "#" : string.Empty,
            Record = Settings.Caller.Record,
            Url = new Uri(Settings.Connect(contactId: contactId))
        }, _twilio);

        return new VoipCall { Id = call.Sid, NumberFrom = call.From, NumberTo = call.To };
    }

    public override VoipCall LocalCall(string to)
    {
        return Call(Number + "#" + to);
    }

    public override VoipCall RedirectCall(string callId, string to)
    {
        var call = CallResource.Update(callId, url: new Uri(Settings.Redirect(to)), method: HttpMethod.Post, client: _twilio);
        return new VoipCall { Id = call.Sid, NumberTo = to };
    }

    public override VoipCall HoldUp(string callId)
    {
        return RedirectCall(callId, "hold");
    }

    #endregion

    #region Queue

    public Queue CreateQueue(string name, int size, string waitUrl, int waitTime)
    {
        var queues = QueueResource.Read(new ReadQueueOptions(), _twilio);
        var queue = queues.FirstOrDefault(r => r.FriendlyName == name);
        if (queue == null)
        {
            queue = QueueResource.Create(name, client: _twilio);
        }
        return new Queue(queue.Sid, name, size, waitUrl, waitTime);
    }

    public string GetQueue(string name)
    {
        var queues = QueueResource.Read(new ReadQueueOptions(), _twilio);
        return queues.First(r => r.FriendlyName == name).Sid;
    }

    public IEnumerable<string> QueueCalls(string id)
    {
        var calls = MemberResource.Read(id, client: _twilio);
        return calls.Select(r => r.CallSid);
    }

    private void AnswerQueueCall(string queueId, string callId, bool reject = false)
    {
        var calls = QueueCalls(queueId);
        if (calls.Contains(callId))
        {
            MemberResource.Update(queueId, callId, new Uri(Settings.Dequeue(reject)), HttpMethod.Post,
                client: _twilio);
        }
    }

    public override void AnswerQueueCall(string callId)
    {
        AnswerQueueCall(Settings.Queue.Id, callId);
    }

    public override void RejectQueueCall(string callId)
    {
        AnswerQueueCall(Settings.Queue.Id, callId, true);
    }

    #endregion
}
