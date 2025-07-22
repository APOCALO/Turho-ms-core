using Application.Common;
using Domain.Primitives;
using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IBaseRepository<T, TId>
        where T : class
        where TId : IValueObject
    {
        Task<(List<T>, int totalCount)> GetAllPagedAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task AddAsync(T entity, CancellationToken cancellationToken);
        Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken);
        Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        void Update(T entity);
        void Delete(T entity);
    }
}
