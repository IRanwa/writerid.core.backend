namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// The queue service.
/// </summary>
public interface IQueueService
{
    /// <summary>
    /// Sends the message asynchronous.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="messageText">The message text.</param>
    Task SendMessageAsync(string queueName, string messageText);
}