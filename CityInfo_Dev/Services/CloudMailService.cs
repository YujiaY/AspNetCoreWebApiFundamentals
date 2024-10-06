namespace CityInfo_Dev.Services;

public class CloudMailService : IMailService
{
    private string _mailTo = "admin_cloud@mycompany.com";
    private string _mailFrom = "noreply_cloud@mycompany.com";

    public void Send(string subject, string message)
    {
        // send mail - output to console window
        Console.WriteLine($"Cloud Mail from {_mailFrom} to {_mailTo}, with {nameof(CloudMailService)}.");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}