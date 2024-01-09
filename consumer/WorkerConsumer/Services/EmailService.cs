using System.Net;
using System.Net.Mail;
using WorkerCreateUserConsumer.Contracts;
using WorkerCreateUserConsumer.Exceptions;

namespace WorkerCreateUserConsumer.Services;
public class EmailService : IEmailService
{
    public EmailService(string name, string email, string password, string host = "smtp.gmail.com", int port = 587) =>
        (MyName, MyEmail, MyPassword, Host, Port) = (name, email, password, host, port);

    public string MyEmail { get; }
    public string MyName { get; }
    public string MyPassword { get; }
    public string Host { get; }
    public int Port { get; }
    private string Subject = "Hellow!";
    private string Message = "Thanks for register. My name is rodolfojesus and send-me a message in linkeIn :)";

    public async Task Send((string toMail, string toName) receiver)
    {
        if (CheckValues())
            throw new EmailInvalidDataException("The properties for email are invalid");

        using (var client = new SmtpClient())
        {
            client.Host = Host;
            client.Port = Port;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(MyEmail, MyPassword);
            using (var message = new MailMessage(
                from: new MailAddress(MyEmail, MyName),
                to: new MailAddress(receiver.toMail, receiver.toName)
                ))
            {

                message.Subject = Subject;
                message.Body = Message;

                client.SendMailAsync(message);
            }
        }
    }

    private bool CheckValues() =>
        string.IsNullOrEmpty(MyEmail) || string.IsNullOrEmpty(MyPassword);
}
