using WriterID.Dev.Portal.Model.Queue;

namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// The IAzureQueueService interface.
/// </summary>
public interface IAzureQueueService
{
    /// <summary>
    /// Sends a dataset analysis message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The dataset analysis message containing processing parameters.</param>
    Task SendDatasetAnalysisMessageAsync(DatasetAnalysisMessage message);

    /// <summary>
    /// Sends a model training message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The model training message containing training parameters.</param>
    Task SendModelTrainingMessageAsync(ModelTrainingMessage message);
} 