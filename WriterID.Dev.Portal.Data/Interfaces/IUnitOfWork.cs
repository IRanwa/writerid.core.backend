using WriterID.Dev.Portal.Model.Entities;

namespace WriterID.Dev.Portal.Data.Interfaces;

/// <summary>
/// Unit of Work interface for managing repositories and transactions.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the dataset repository.
    /// </summary>
    IGenericRepository<Dataset> Datasets { get; }

    /// <summary>
    /// Gets the writer identification model repository.
    /// </summary>
    IGenericRepository<WriterIdentificationModel> Models { get; }

    /// <summary>
    /// Gets the writer identification task repository.
    /// </summary>
    IGenericRepository<WriterIdentificationTask> Tasks { get; }

    /// <summary>
    /// Gets the user repository.
    /// </summary>
    IGenericRepository<User> Users { get; }

    /// <summary>
    /// Gets a repository for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The repository for the specified entity type.</returns>
    IGenericRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <returns>The database transaction.</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackTransactionAsync();
} 