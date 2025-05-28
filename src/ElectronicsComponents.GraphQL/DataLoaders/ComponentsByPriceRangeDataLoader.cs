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
    public class ComponentsByPriceRangeDataLoader : BatchDataLoader<PriceRangeKey, IEnumerable<Component>>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ComponentsByPriceRangeDataLoader(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IBatchScheduler batchScheduler)
            : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override async Task<IReadOnlyDictionary<PriceRangeKey, IEnumerable<Component>>> LoadBatchAsync(
            IReadOnlyList<PriceRangeKey> keys,
            CancellationToken cancellationToken)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var components = await dbContext.Components
                .Where(c => keys.Any(k => c.Price >= k.MinPrice && c.Price <= k.MaxPrice))
                .ToListAsync(cancellationToken);

            return keys.ToDictionary(
                key => key,
                key => components.Where(c => c.Price >= key.MinPrice && c.Price <= key.MaxPrice).AsEnumerable());
        }
    }

    public record PriceRangeKey(decimal MinPrice, decimal MaxPrice);
} 