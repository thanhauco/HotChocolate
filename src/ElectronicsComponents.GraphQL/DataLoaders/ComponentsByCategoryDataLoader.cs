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
    public class ComponentsByCategoryDataLoader : BatchDataLoader<string, IEnumerable<Component>>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ComponentsByCategoryDataLoader(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IBatchScheduler batchScheduler)
            : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override async Task<IReadOnlyDictionary<string, IEnumerable<Component>>> LoadBatchAsync(
            IReadOnlyList<string> keys,
            CancellationToken cancellationToken)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var components = await dbContext.Components
                .Where(c => keys.Contains(c.Category))
                .ToListAsync(cancellationToken);

            return components
                .GroupBy(c => c.Category)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }
    }
} 