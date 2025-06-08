using Microsoft.EntityFrameworkCore.Storage;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Data.Repositories;
using WriterID.Dev.Portal.Model.Entities;

namespace WriterID.Dev.Portal.Data;

/// <summary>
/// Unit of Work implementation for managing repositories and transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;
    private readonly Dictionary<Type, object> repositories;
    private IDbContextTransaction transaction;

    private IGenericRepository<Dataset> datasets;
    private IGenericRepository<WriterIdentificationModel> models;
    private IGenericRepository<WriterIdentificationTask> tasks;
    private IGenericRepository<User> users;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(ApplicationDbContext context)
    {
        this.context = context;
        this.repositories = new Dictionary<Type, object>();
    }

    /// <summary>
    /// Gets the dataset repository.
    /// </summary>
    public IGenericRepository<Dataset> Datasets 
    {
        get { return datasets ??= new GenericRepository<Dataset>(context); }
    }

    /// <summary>
    /// Gets the writer identification model repository.
    /// </summary>
    public IGenericRepository<WriterIdentificationModel> Models 
    {
        get { return models ??= new GenericRepository<WriterIdentificationModel>(context); }
    }

    /// <summary>
    /// Gets the writer identification task repository.
    /// </summary>
    public IGenericRepository<WriterIdentificationTask> Tasks 
    {
        get { return tasks ??= new GenericRepository<WriterIdentificationTask>(context); }
    }

    /// <summary>
    /// Gets the user repository.
    /// </summary>
    public IGenericRepository<User> Users 
    {
        get { return users ??= new GenericRepository<User>(context); }
    }

    /// <summary>
    /// Gets a repository for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The repository for the specified entity type.</returns>
    public IGenericRepository<T> Repository<T>() where T : class
    {
        if (repositories.ContainsKey(typeof(T)))
        {
            return (IGenericRepository<T>)repositories[typeof(T)];
        }

        var repository = new GenericRepository<T>(context);
        repositories.Add(typeof(T), repository);
        return repository;
    }

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <returns>The database transaction.</returns>
    public async Task BeginTransactionAsync()
    {
        transaction = await context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        if (transaction != null)
        {
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        if (transaction != null)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();
            transaction = null;
        }
    }

    /// <summary>
    /// Disposes the Unit of Work and its resources.
    /// </summary>
    public void Dispose()
    {
        transaction?.Dispose();
        context.Dispose();
    }
} 