namespace GatherUp.Core.Interfaces
{
    public interface IEmailService
    {
        void Send(string toEmail, string message);
    }
}
