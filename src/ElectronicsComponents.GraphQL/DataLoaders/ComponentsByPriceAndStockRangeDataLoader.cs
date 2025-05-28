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
    public class ComponentsByPriceAndStockRangeDataLoader : BatchDataLoader<PriceAndStockRangeKey, IEnumerable<Component>>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ComponentsByPriceAndStockRangeDataLoader(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IBatchScheduler batchScheduler)
            : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override async Task<IReadOnlyDictionary<PriceAndStockRangeKey, IEnumerable<Component>>> LoadBatchAsync(
            IReadOnlyList<PriceAndStockRangeKey> keys,
            CancellationToken cancellationToken)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var components = await dbContext.Components
                .Where(c => keys.Any(k => 
                    c.Price >= k.MinPrice && c.Price <= k.MaxPrice &&
                    c.StockQuantity >= k.MinStock && c.StockQuantity <= k.MaxStock))
                .ToListAsync(cancellationToken);

            return keys.ToDictionary(
                key => key,
                key => components.Where(c => 
                    c.Price >= key.MinPrice && c.Price <= key.MaxPrice &&
                    c.StockQuantity >= key.MinStock && c.StockQuantity <= key.MaxStock).AsEnumerable());
        }
    }

    public record PriceAndStockRangeKey(
        decimal MinPrice,
        decimal MaxPrice,
        int MinStock,
        int MaxStock);
} 