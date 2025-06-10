using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WriterID.Dev.Portal.Model.Queue;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The AzureQueueService class.
/// </summary>
public class AzureQueueService : IAzureQueueService
{
    private readonly QueueClient queueClient;
    private readonly ILogger<AzureQueueService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureQueueService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration containing Azure Storage connection string and queue name</param>
    /// <param name="logger">The logger instance for recording operational data.</param>
    public AzureQueueService(IConfiguration configuration, ILogger<AzureQueueService> logger)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");
        var queueName = configuration["Azure:QueueName"];
        this.queueClient = new QueueClient(connectionString, queueName);
        this.logger = logger;
    }

    /// <summary>
    /// Sends a dataset analysis message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The dataset analysis message containing processing parameters.</param>
    public async Task SendDatasetAnalysisMessageAsync(DatasetAnalysisMessage message)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(message);
            await queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageJson)));
            
            logger.LogInformation("Sent dataset analysis message for dataset {DatasetId}", message.DatasetId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending dataset analysis message");
            throw;
        }
    }

    /// <summary>
    /// Sends a model training message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The model training message containing training parameters.</param>
    public async Task SendModelTrainingMessageAsync(ModelTrainingMessage message)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(message);
            await queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageJson)));
            
            logger.LogInformation("Sent model training message for model {ModelId}", message.ModelId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending model training message");
            throw;
        }
    }

    /// <summary>
    /// Sends a writer identification task message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The writer identification task message containing task parameters.</param>
    public async Task SendWriterIdentificationTaskMessageAsync(WriterIdentificationTaskMessage message)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(message);
            await queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageJson)));
            
            logger.LogInformation("Sent writer identification task message for task {TaskId} using {ModelType}", 
                message.TaskId, 
                message.Parameters.UseDefaultModel ? "default model" : "custom model");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending writer identification task message");
            throw;
        }
    }
} 
 