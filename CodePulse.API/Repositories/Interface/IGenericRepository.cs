namespace CodePulse.API.Repositories.Interface
{
    public interface IGenericRepository<T> where T: class
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> UpdateAsync(T entity);
        Task<T?> DeleteAsync(Guid id);
    }
}
