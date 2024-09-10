namespace App.Core.Repositories
{
    public interface IBaseRepository<T>
    {
        Task AddAsync(T entity);
        Task DeleteAsync(int id);
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(int id);
        void RemoveRange(T[] entities);
        Task SaveChangesAsync();
        void UpdateAsync(T entity);
    }
}
