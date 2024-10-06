namespace CityInfo_Dev.Services;

public class LocalMailService(IConfiguration configuration) : IMailService
{
    private string _mailTo = configuration["mailSettings:mailToAddress"] ?? string.Empty;
    private string _mailFrom = configuration["mailSettings:mailFromAddress"] ?? string.Empty;

    public void Send(string subject, string message)
    {
        // send mail - output to console window
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(LocalMailService)}.");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}