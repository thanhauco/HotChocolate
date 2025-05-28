using ElectronicsComponents.GraphQL.Data;
using ElectronicsComponents.GraphQL.Models;
using GreenDonut;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronicsComponents.GraphQL
{
    public class ComponentsByStockRangeDataLoader : BatchDataLoader<StockRangeKey, IEnumerable<Component>>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ComponentsByStockRangeDataLoader(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IBatchScheduler batchScheduler)
            : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override async Task<IReadOnlyDictionary<StockRangeKey, IEnumerable<Component>>> LoadBatchAsync(
            IReadOnlyList<StockRangeKey> keys,
            CancellationToken cancellationToken)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var components = await dbContext.Components
                .Where(c => keys.Any(k => c.StockQuantity >= k.MinStock && c.StockQuantity <= k.MaxStock))
                .ToListAsync(cancellationToken);

            return keys.ToDictionary(
                key => key,
                key => components.Where(c => c.StockQuantity >= key.MinStock && c.StockQuantity <= key.MaxStock).AsEnumerable());
        }
    }

    public record StockRangeKey(int MinStock, int MaxStock);
} 