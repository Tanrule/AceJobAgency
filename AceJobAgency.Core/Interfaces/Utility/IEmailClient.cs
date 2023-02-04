namespace AceJobAgency.Core.Interfaces.Utility 
{ 
    public interface IEmailClient
    {
        bool SendMail(string toEmail,
            string firstName,
            string body,
            string subject);
    }
}