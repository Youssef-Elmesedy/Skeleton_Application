using Microsoft.EntityFrameworkCore;
using Skeleton.Domain.Entities;
using Skeleton.Domain.Interfaces.InterfacesRepository;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.Implementation.Repositories;

internal class WriteRepository<TEntity> : IWriteRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public WriteRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    // ✅ async حقيقي لأن AddAsync بترجع ValueTask
    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    // ✅ مفيش await هنا — بقت Task.CompletedTask بدل async فاضية
    public Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    // ✅ نفس الكلام
    public Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}