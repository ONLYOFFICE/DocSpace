namespace ASC.MessagingSystem.Core.Sender;

public interface IMessageSender
{
    void Send(EventMessage message);
}
