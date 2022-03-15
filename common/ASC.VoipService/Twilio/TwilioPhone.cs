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
            Url = new System.Uri(Settings.Connect(contactId: contactId))
        }, _twilio);

        return new VoipCall { Id = call.Sid, NumberFrom = call.From, NumberTo = call.To };
    }

    public override VoipCall LocalCall(string to)
    {
        return Call(Number + "#" + to);
    }

    public override VoipCall RedirectCall(string callId, string to)
    {
        var call = CallResource.Update(callId, url: new System.Uri(Settings.Redirect(to)), method: HttpMethod.Post, client: _twilio);
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
            MemberResource.Update(queueId, callId, new System.Uri(Settings.Dequeue(reject)), HttpMethod.Post,
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
