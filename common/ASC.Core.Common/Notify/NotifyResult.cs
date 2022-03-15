namespace ASC.Notify;

public class NotifyResult
{
    public SendResult Result { get; internal set; }
    public List<SendResponse> Responses { get; set; }

    internal NotifyResult(SendResult result, List<SendResponse> responses)
    {
        Result = result;
        Responses = responses ?? new List<SendResponse>();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendFormat("SendResult: {0} whith {1} sub-results", Result, Responses.Count);
        foreach (var responce in Responses)
        {
            var recipient = "<recipient:nomessage>";
            var error = "";
            if (responce.NoticeMessage != null)
            {
                if (responce.NoticeMessage.Recipient != null)
                {
                    recipient = responce.NoticeMessage.Recipient.Addresses.Length > 0 ?
                        responce.NoticeMessage.Recipient.Addresses[0] :
                        "<no-address>";
                }
                else
                {
                    recipient = "<null-address>";
                }
            }
            if (responce.Exception != null)
            {
                error = responce.Exception.Message;
            }

            sb.AppendLine();
            sb.AppendFormat("   {3}->{0}({1})={2} {4}", recipient, responce.SenderName, responce.Result, responce.NotifyAction.ID, error);
        }

        return sb.ToString();
    }
}
