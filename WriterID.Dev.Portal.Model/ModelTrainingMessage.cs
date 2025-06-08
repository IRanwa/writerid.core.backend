using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model;

/// <summary>
/// Queue message for requesting model training.
/// </summary>
public class ModelTrainingMessage : QueueMessageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelTrainingMessage"/> class.
    /// </summary>
    /// <param name="datasetContainerName">The name of the container containing the dataset.</param>
    /// <param name="modelContainerName">The name of the container for storing the model.</param>
    public ModelTrainingMessage(string datasetContainerName, string modelContainerName)
    {
        Task = "train";
        Parameters = new ModelTrainingParameters
        {
            DatasetContainerName = datasetContainerName,
            ModelContainerName = modelContainerName
        };
    }
} 