namespace CityInfo_Dev.Services;

public interface IMailService
{
    void Send(string subject, string message);
}