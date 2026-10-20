
using DAL.Entities;

namespace Repositories
{
    public interface IDbContextFactory
    {
        STKDbContext DbContext { get; }
    }
}
