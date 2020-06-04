using Microsoft.EntityFrameworkCore;
using Miki.Patterns.Repositories;

namespace Miki.Framework
{
    public class EntityRepositoryFactory<T> : IRepositoryFactory<T>
        where T : class
    {
        public IAsyncRepository<T> Build(DbContext context)
        {
            return new EntityRepository<T>(context);
        }
    }
}