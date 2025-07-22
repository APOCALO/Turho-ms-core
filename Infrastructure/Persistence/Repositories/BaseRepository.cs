using Application.Common;
using Application.Extensions;
using Application.Interfaces;
using Domain.Primitives;
using ErrorOr;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class BaseRepository<T, TId> : IBaseRepository<T, TId>
        where T : class
        where TId : IValueObject
    {
        private readonly ApplicationDbContext _dbContext;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<T>()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<T>, int totalCount)> GetAllPagedAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var totalCount = await _dbContext.Set<T>().CountAsync(cancellationToken);

            var query = _dbContext.Set<T>().AsQueryable();

            if (typeof(T).GetProperty("Id") != null)
            {
                query = query.OrderBy(e => EF.Property<object>(e, "Id"));
            }

            var result = await query
                .Paginate(paginationParameters)
                .ToListAsync(cancellationToken);

            return (result, totalCount);
        }

        public async Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<T>().FindAsync(id, cancellationToken);
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                Error.Validation("BaseRepository.Update", $"entity {typeof(T)} cannot be null.");
                return;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
