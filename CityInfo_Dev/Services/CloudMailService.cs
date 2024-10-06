namespace CityInfo_Dev.Services;

public class CloudMailService(IConfiguration configuration) : IMailService
{
    private string _mailTo = configuration["mailSettings:mailToAddress"] ?? string.Empty;
    private string _mailFrom = configuration["mailSettings:mailFromAddress"] ?? string.Empty;

    public void Send(string subject, string message)
    {
        // send mail - output to console window
        Console.WriteLine($"Cloud Mail from {_mailFrom} to {_mailTo}, with {nameof(CloudMailService)}.");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}