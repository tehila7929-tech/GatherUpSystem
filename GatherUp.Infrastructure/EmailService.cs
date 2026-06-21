using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly string _logPath;

        public EmailService(string folderPath)
        {
            _logPath = Path.Combine(folderPath, "emails.txt");
        }

        public void Send(string toEmail, string message)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] TO: {toEmail} | {message}";
            File.AppendAllText(_logPath, line + Environment.NewLine);
        }
    }
}
