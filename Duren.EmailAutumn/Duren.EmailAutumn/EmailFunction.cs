using System.Net;
using Azure;
using Azure.Communication.Email;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Duren.EmailAutumn;

public class EmailFunction(EmailClient emailClient, ILogger<EmailFunction> logger)
{
    private readonly string _senderAddress =
        Environment.GetEnvironmentVariable("SENDER_ADDRESS")
        ?? throw new InvalidOperationException("SENDER_ADDRESS environment variable is not set.");
    private readonly EmailClient _emailClient = emailClient;

    [Function(nameof(EmailFunction))]
    public async Task Run([TimerTrigger("0 0 5 * * *")] TimerInfo timerInfo)
    {
        logger.LogInformation("C# Timer trigger function executed at: {Time}", DateTimeOffset.Now);
        var emailMessage = new EmailMessage(
            senderAddress: _senderAddress,
            content: new EmailContent("Hey there cutie")
            {
                PlainText = "This is your boyfriend reminding you that I love you!",
                Html =
                    "<html><body><h1>This is your boyfriend reminding you that I love you!</h1></body></html>",
            },
            recipients: new EmailRecipients([new EmailAddress("temp@temp.com")])
        );
        var operation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
        if (
            operation.HasCompleted
            && operation.GetRawResponse().Status == (int)HttpStatusCode.Accepted
        )
        {
            logger.LogInformation("Email sent successfully.");
        }
        else
        {
            logger.LogError(
                "Failed to send email. Status: {Status}",
                operation.GetRawResponse().Status
            );
        }
        logger.LogInformation("EmailFunction completed at: {Time}", DateTimeOffset.Now);
        logger.LogInformation("Next timer schedule at: {Schedule}", timerInfo.ScheduleStatus?.Next);
    }
}
