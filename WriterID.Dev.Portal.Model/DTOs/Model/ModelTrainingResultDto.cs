using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.DTOs.Model;

/// <summary>
/// Data transfer object for model training results.
/// </summary>
public class ModelTrainingResultDto
{
    /// <summary>
    /// Gets or sets the training accuracy percentage.
    /// </summary>
    [JsonPropertyName("accuracy")]
    public double Accuracy { get; set; }

    /// <summary>
    /// Gets or sets the F1 score.
    /// </summary>
    [JsonPropertyName("f1_score")]
    public double F1Score { get; set; }

    /// <summary>
    /// Gets or sets the precision score.
    /// </summary>
    [JsonPropertyName("precision")]
    public double Precision { get; set; }

    /// <summary>
    /// Gets or sets the recall score.
    /// </summary>
    [JsonPropertyName("recall")]
    public double Recall { get; set; }

    /// <summary>
    /// Gets or sets the confusion matrix.
    /// </summary>
    [JsonPropertyName("confusion_matrix")]
    public int[][] ConfusionMatrix { get; set; }

    /// <summary>
    /// Gets or sets the training time in seconds.
    /// </summary>
    [JsonPropertyName("time")]
    public double Time { get; set; }

    /// <summary>
    /// Gets or sets the requested number of episodes.
    /// </summary>
    [JsonPropertyName("requested_episodes")]
    public int RequestedEpisodes { get; set; }

    /// <summary>
    /// Gets or sets the actual number of episodes run.
    /// </summary>
    [JsonPropertyName("actual_episodes_run")]
    public int ActualEpisodesRun { get; set; }

    /// <summary>
    /// Gets or sets the optimal validation episode.
    /// </summary>
    [JsonPropertyName("optimal_val_episode")]
    public int OptimalValEpisode { get; set; }

    /// <summary>
    /// Gets or sets the best validation accuracy.
    /// </summary>
    [JsonPropertyName("best_val_accuracy")]
    public double BestValAccuracy { get; set; }

    /// <summary>
    /// Gets or sets the backbone model used.
    /// </summary>
    [JsonPropertyName("backbone")]
    public string Backbone { get; set; }

    /// <summary>
    /// Gets or sets any error that occurred during training.
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; }

    /// <summary>
    /// Gets or sets the optimal threshold value.
    /// </summary>
    [JsonPropertyName("optimal_threshold")]
    public double OptimalThreshold { get; set; }

    /// <summary>
    /// Gets or sets the threshold accuracy.
    /// </summary>
    [JsonPropertyName("threshold_accuracy")]
    public double ThresholdAccuracy { get; set; }
} 