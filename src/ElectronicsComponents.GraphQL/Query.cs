using ElectronicsComponents.GraphQL.Data;
using ElectronicsComponents.GraphQL.Models;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronicsComponents.GraphQL
{
    [ExtendObjectType(typeof(Query))]
    public class Query
    {
        [UseDbContext(typeof(ApplicationDbContext))]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Component> GetComponents([Service] ApplicationDbContext context)
        {
            return context.Components;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> GetComponentById(
            int id,
            [Service] ApplicationDbContext context,
            ComponentByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            return await dataLoader.LoadAsync(id, cancellationToken);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByCategory(
            string category,
            [Service] ApplicationDbContext context,
            ComponentsByCategoryDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            return await dataLoader.LoadAsync(category, cancellationToken);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> SearchComponents(
            string searchTerm,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.Name.Contains(searchTerm) || 
                           c.Description.Contains(searchTerm) ||
                           c.Category.Contains(searchTerm))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsInStock(
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.StockQuantity > 0)
                .OrderByDescending(c => c.StockQuantity)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetLowStockComponents(
            int threshold,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.StockQuantity <= threshold)
                .OrderBy(c => c.StockQuantity)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByPriceRange(
            decimal minPrice,
            decimal maxPrice,
            [Service] ApplicationDbContext context,
            ComponentsByPriceRangeDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            return await dataLoader.LoadAsync(new PriceRangeKey(minPrice, maxPrice), cancellationToken);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByManufacturer(
            string manufacturer,
            [Service] ApplicationDbContext context,
            ComponentsByManufacturerDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            return await dataLoader.LoadAsync(manufacturer, cancellationToken);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByPartNumber(
            string partNumber,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.PartNumber.Contains(partNumber))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByStockStatus(
            bool inStock,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => inStock ? c.StockQuantity > 0 : c.StockQuantity == 0)
                .OrderByDescending(c => c.StockQuantity)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByPriceThreshold(
            decimal priceThreshold,
            bool aboveThreshold,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => aboveThreshold ? c.Price >= priceThreshold : c.Price <= priceThreshold)
                .OrderBy(c => c.Price)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByMultipleCategories(
            string[] categories,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => categories.Contains(c.Category))
                .OrderBy(c => c.Category)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByMultipleManufacturers(
            string[] manufacturers,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => manufacturers.Contains(c.Manufacturer))
                .OrderBy(c => c.Manufacturer)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByStockRange(
            int minStock,
            int maxStock,
            [Service] ApplicationDbContext context,
            ComponentsByStockRangeDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            return await dataLoader.LoadAsync(new StockRangeKey(minStock, maxStock), cancellationToken);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByStockAndPrice(
            int minStock,
            decimal maxPrice,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.StockQuantity >= minStock && c.Price <= maxPrice)
                .OrderBy(c => c.Price)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByManufacturerAndCategory(
            string manufacturer,
            string category,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.Manufacturer == manufacturer && c.Category == category)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsBySearchCriteria(
            string? searchTerm = null,
            string? category = null,
            string? manufacturer = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minStock = null,
            [Service] ApplicationDbContext context)
        {
            var query = context.Components.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => 
                    c.Name.Contains(searchTerm) || 
                    c.Description.Contains(searchTerm) ||
                    c.PartNumber.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(category))
                query = query.Where(c => c.Category == category);

            if (!string.IsNullOrEmpty(manufacturer))
                query = query.Where(c => c.Manufacturer == manufacturer);

            if (minPrice.HasValue)
                query = query.Where(c => c.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.Price <= maxPrice.Value);

            if (minStock.HasValue)
                query = query.Where(c => c.StockQuantity >= minStock.Value);

            return await query.OrderBy(c => c.Name).ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByStockTrend(
            bool increasing,
            [Service] ApplicationDbContext context)
        {
            // This is a placeholder for a more complex implementation
            // In a real system, you would track stock history and calculate trends
            return await context.Components
                .OrderBy(c => increasing ? c.StockQuantity : -c.StockQuantity)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByPriceTrend(
            bool increasing,
            [Service] ApplicationDbContext context)
        {
            // This is a placeholder for a more complex implementation
            // In a real system, you would track price history and calculate trends
            return await context.Components
                .OrderBy(c => increasing ? c.Price : -c.Price)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByAdvancedSearch(
            AdvancedSearchInput input,
            [Service] ApplicationDbContext context)
        {
            var query = context.Components.AsQueryable();

            // Text search
            if (!string.IsNullOrEmpty(input.SearchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(input.SearchTerm) ||
                    c.Description.Contains(input.SearchTerm) ||
                    c.PartNumber.Contains(input.SearchTerm) ||
                    c.Manufacturer.Contains(input.SearchTerm));
            }

            // Category filter
            if (input.Categories?.Any() == true)
                query = query.Where(c => input.Categories.Contains(c.Category));

            // Manufacturer filter
            if (input.Manufacturers?.Any() == true)
                query = query.Where(c => input.Manufacturers.Contains(c.Manufacturer));

            // Price range
            if (input.MinPrice.HasValue)
                query = query.Where(c => c.Price >= input.MinPrice.Value);
            if (input.MaxPrice.HasValue)
                query = query.Where(c => c.Price <= input.MaxPrice.Value);

            // Stock range
            if (input.MinStock.HasValue)
                query = query.Where(c => c.StockQuantity >= input.MinStock.Value);
            if (input.MaxStock.HasValue)
                query = query.Where(c => c.StockQuantity <= input.MaxStock.Value);

            // Sorting
            query = input.SortBy switch
            {
                "name" => input.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "price" => input.SortDescending ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price),
                "stock" => input.SortDescending ? query.OrderByDescending(c => c.StockQuantity) : query.OrderBy(c => c.StockQuantity),
                "category" => input.SortDescending ? query.OrderByDescending(c => c.Category) : query.OrderBy(c => c.Category),
                "manufacturer" => input.SortDescending ? query.OrderByDescending(c => c.Manufacturer) : query.OrderBy(c => c.Manufacturer),
                _ => query.OrderBy(c => c.Name)
            };

            return await query.ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByInventoryStatus(
            InventoryStatus status,
            [Service] ApplicationDbContext context)
        {
            return status switch
            {
                InventoryStatus.InStock => await context.Components.Where(c => c.StockQuantity > 0).ToListAsync(),
                InventoryStatus.LowStock => await context.Components.Where(c => c.StockQuantity > 0 && c.StockQuantity <= 10).ToListAsync(),
                InventoryStatus.OutOfStock => await context.Components.Where(c => c.StockQuantity == 0).ToListAsync(),
                InventoryStatus.Overstocked => await context.Components.Where(c => c.StockQuantity > 100).ToListAsync(),
                _ => await context.Components.ToListAsync()
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByPriceCategory(
            PriceCategory category,
            [Service] ApplicationDbContext context)
        {
            return category switch
            {
                PriceCategory.Budget => await context.Components.Where(c => c.Price < 10).ToListAsync(),
                PriceCategory.MidRange => await context.Components.Where(c => c.Price >= 10 && c.Price <= 50).ToListAsync(),
                PriceCategory.Premium => await context.Components.Where(c => c.Price > 50).ToListAsync(),
                _ => await context.Components.ToListAsync()
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<IEnumerable<Component>> GetComponentsByPriceAndStockRange(
            decimal minPrice,
            decimal maxPrice,
            int minStock,
            int maxStock,
            [Service] ApplicationDbContext context,
            ComponentsByPriceAndStockRangeDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            return await dataLoader.LoadAsync(
                new PriceAndStockRangeKey(minPrice, maxPrice, minStock, maxStock),
                cancellationToken
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<int> GetTotalComponentCount([Service] ApplicationDbContext context)
        {
            return await context.Components.CountAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<decimal> GetTotalStockValue([Service] ApplicationDbContext context)
        {
            return await context.Components.SumAsync(c => c.Price * c.StockQuantity);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<PriceStats> GetComponentPriceStats([Service] ApplicationDbContext context)
        {
            var prices = await context.Components.Select(c => c.Price).ToListAsync();
            if (!prices.Any()) return new PriceStats(0, 0, 0);
            return new PriceStats(prices.Average(), prices.Min(), prices.Max());
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<TopComponent>> GetTopNMostExpensiveComponents(int n, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .OrderByDescending(c => c.Price)
                .Take(n)
                .Select(c => new TopComponent(c.Id, c.Name, c.Price, c.StockQuantity))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<TopComponent>> GetTopNLowestStockComponents(int n, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .OrderBy(c => c.StockQuantity)
                .Take(n)
                .Select(c => new TopComponent(c.Id, c.Name, c.Price, c.StockQuantity))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<CategoryStockSummary>> GetStockSummaryByCategory([Service] ApplicationDbContext context)
        {
            return await context.Components
                .GroupBy(c => c.Category)
                .Select(g => new CategoryStockSummary(g.Key, g.Count(), g.Sum(c => c.StockQuantity)))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ManufacturerStockSummary>> GetStockSummaryByManufacturer([Service] ApplicationDbContext context)
        {
            return await context.Components
                .GroupBy(c => c.Manufacturer)
                .Select(g => new ManufacturerStockSummary(g.Key, g.Count(), g.Sum(c => c.StockQuantity)))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetLowStockAlertComponents(int threshold, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.StockQuantity < threshold)
                .OrderBy(c => c.StockQuantity)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetOverstockedAlertComponents(int threshold, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.StockQuantity > threshold)
                .OrderByDescending(c => c.StockQuantity)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetHighPriceAlertComponents(decimal threshold, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.Price > threshold)
                .OrderByDescending(c => c.Price)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetLowPriceAlertComponents(decimal threshold, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.Price < threshold)
                .OrderBy(c => c.Price)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetLowStockAndHighPriceAlertComponents(int stockThreshold, decimal priceThreshold, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.StockQuantity < stockThreshold && c.Price > priceThreshold)
                .OrderBy(c => c.StockQuantity)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetOverstockedAndLowPriceAlertComponents(int stockThreshold, decimal priceThreshold, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.StockQuantity > stockThreshold && c.Price < priceThreshold)
                .OrderByDescending(c => c.StockQuantity)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentAnalytics> GetComponentAnalytics([Service] ApplicationDbContext context)
        {
            var totalComponents = await context.Components.CountAsync();
            var totalValue = await context.Components.SumAsync(c => c.Price * c.StockQuantity);
            var lowStockCount = await context.Components.CountAsync(c => c.StockQuantity < 10);
            var outOfStockCount = await context.Components.CountAsync(c => c.StockQuantity == 0);
            
            var categoryStats = await context.Components
                .GroupBy(c => c.Category)
                .Select(g => new CategoryStats
                {
                    Category = g.Key,
                    Count = g.Count(),
                    TotalValue = g.Sum(c => c.Price * c.StockQuantity)
                })
                .ToListAsync();

            return new ComponentAnalytics
            {
                TotalComponents = totalComponents,
                TotalInventoryValue = totalValue,
                LowStockCount = lowStockCount,
                OutOfStockCount = outOfStockCount,
                CategoryStatistics = categoryStats
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByPriceRange(decimal minPrice, decimal maxPrice, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
                .OrderBy(c => c.Price)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByManufacturer(string manufacturer, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.Manufacturer == manufacturer)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByPartNumber(string partNumber, [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Where(c => c.PartNumber.Contains(partNumber))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentTrends> GetComponentTrends([Service] ApplicationDbContext context)
        {
            var components = await context.Components.ToListAsync();
            
            var priceTrends = components
                .GroupBy(c => c.Category)
                .Select(g => new PriceTrend
                {
                    Category = g.Key,
                    AveragePrice = g.Average(c => c.Price),
                    MinPrice = g.Min(c => c.Price),
                    MaxPrice = g.Max(c => c.Price)
                })
                .ToList();

            var stockTrends = components
                .GroupBy(c => c.Category)
                .Select(g => new StockTrend
                {
                    Category = g.Key,
                    TotalStock = g.Sum(c => c.StockQuantity),
                    LowStockCount = g.Count(c => c.StockQuantity < 10),
                    OutOfStockCount = g.Count(c => c.StockQuantity == 0)
                })
                .ToList();

            return new ComponentTrends
            {
                PriceTrends = priceTrends,
                StockTrends = stockTrends
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<InventoryHealth> GetInventoryHealth([Service] ApplicationDbContext context)
        {
            var components = await context.Components.ToListAsync();
            
            var healthMetrics = new InventoryHealth
            {
                TotalComponents = components.Count,
                TotalValue = components.Sum(c => c.Price * c.StockQuantity),
                LowStockItems = components.Count(c => c.StockQuantity < 10),
                OutOfStockItems = components.Count(c => c.StockQuantity == 0),
                OverstockedItems = components.Count(c => c.StockQuantity > 100),
                HealthScore = CalculateHealthScore(components)
            };

            return healthMetrics;
        }

        private double CalculateHealthScore(List<Component> components)
        {
            if (!components.Any()) return 0;

            var totalItems = components.Count;
            var lowStockWeight = 0.3;
            var outOfStockWeight = 0.4;
            var overstockedWeight = 0.3;

            var lowStockScore = 1 - (components.Count(c => c.StockQuantity < 10) / (double)totalItems);
            var outOfStockScore = 1 - (components.Count(c => c.StockQuantity == 0) / (double)totalItems);
            var overstockedScore = 1 - (components.Count(c => c.StockQuantity > 100) / (double)totalItems);

            return (lowStockScore * lowStockWeight) +
                   (outOfStockScore * outOfStockWeight) +
                   (overstockedScore * overstockedWeight);
        }
    }

    public record AdvancedSearchInput(
        string? SearchTerm = null,
        string[]? Categories = null,
        string[]? Manufacturers = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        int? MinStock = null,
        int? MaxStock = null,
        string SortBy = "name",
        bool SortDescending = false);

    public enum InventoryStatus
    {
        InStock,
        LowStock,
        OutOfStock,
        Overstocked
    }

    public enum PriceCategory
    {
        Budget,
        MidRange,
        Premium
    }

    public record PriceStats(double Average, decimal Min, decimal Max);
    public record TopComponent(int Id, string Name, decimal Price, int StockQuantity);
    public record CategoryStockSummary(string Category, int ComponentCount, int TotalStock);
    public record ManufacturerStockSummary(string Manufacturer, int ComponentCount, int TotalStock);
    public record ComponentAnalytics(
        int TotalComponents,
        decimal TotalInventoryValue,
        int LowStockCount,
        int OutOfStockCount,
        List<CategoryStats> CategoryStatistics);
    public record CategoryStats(string Category, int Count, decimal TotalValue);
    public record ComponentTrends(
        List<PriceTrend> PriceTrends,
        List<StockTrend> StockTrends);

    public record PriceTrend(
        string Category,
        double AveragePrice,
        decimal MinPrice,
        decimal MaxPrice);

    public record StockTrend(
        string Category,
        int TotalStock,
        int LowStockCount,
        int OutOfStockCount);

    public record InventoryHealth(
        int TotalComponents,
        decimal TotalValue,
        int LowStockItems,
        int OutOfStockItems,
        int OverstockedItems,
        double HealthScore);
} 