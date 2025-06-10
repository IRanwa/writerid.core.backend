using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The queue service.
/// </summary>
/// <seealso cref="IQueueService" />
public class QueueService : IQueueService
{
    /// <summary>
    /// The queue service client
    /// </summary>
    private readonly QueueServiceClient queueServiceClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public QueueService(IConfiguration configuration)
    {
        queueServiceClient = new QueueServiceClient(configuration.GetConnectionString("AzureStorage"));
    }

    /// <summary>
    /// Sends the message asynchronous.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="messageText">The message text.</param>
    public async Task SendMessageAsync(string queueName, string messageText)
    {
        var queueClient = queueServiceClient.GetQueueClient(queueName);
        await queueClient.CreateIfNotExistsAsync();
        await queueClient.SendMessageAsync(messageText);
    }
} 