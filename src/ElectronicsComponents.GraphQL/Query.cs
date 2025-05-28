using ElectronicsComponents.GraphQL.Data;
using ElectronicsComponents.GraphQL.Models;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

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
        public async Task<ComponentAnalytics> GetComponentAnalytics(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components.ToListAsync(cancellationToken);
            
            return new ComponentAnalytics
            {
                TotalCount = components.Count,
                TotalValue = components.Sum(c => c.Price * c.StockQuantity),
                LowStockCount = components.Count(c => c.StockQuantity < c.MinimumStockLevel),
                OutOfStockCount = components.Count(c => c.StockQuantity == 0),
                AveragePrice = components.Average(c => c.Price),
                MostExpensiveComponent = components.OrderByDescending(c => c.Price).FirstOrDefault()?.Name,
                LeastExpensiveComponent = components.OrderBy(c => c.Price).FirstOrDefault()?.Name,
                MostStockedComponent = components.OrderByDescending(c => c.StockQuantity).FirstOrDefault()?.Name,
                LeastStockedComponent = components.OrderBy(c => c.StockQuantity).FirstOrDefault()?.Name
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<CategoryStatistics> GetCategoryStatistics(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components.ToListAsync(cancellationToken);
            var categories = components.GroupBy(c => c.Category);
            
            return new CategoryStatistics
            {
                TotalCategories = categories.Count(),
                CategoryBreakdown = categories.Select(g => new CategoryBreakdown
                {
                    Category = g.Key,
                    Count = g.Count(),
                    TotalValue = g.Sum(c => c.Price * c.StockQuantity),
                    AveragePrice = g.Average(c => c.Price),
                    LowStockCount = g.Count(c => c.StockQuantity < c.MinimumStockLevel),
                    OutOfStockCount = g.Count(c => c.StockQuantity == 0)
                }).ToList()
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<PriceStatistics> GetPriceStatistics(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components.ToListAsync(cancellationToken);
            
            return new PriceStatistics
            {
                AveragePrice = components.Average(c => c.Price),
                MinPrice = components.Min(c => c.Price),
                MaxPrice = components.Max(c => c.Price),
                PriceRanges = new List<PriceRange>
                {
                    new() { Range = "0-10", Count = components.Count(c => c.Price <= 10) },
                    new() { Range = "11-50", Count = components.Count(c => c.Price > 10 && c.Price <= 50) },
                    new() { Range = "51-100", Count = components.Count(c => c.Price > 50 && c.Price <= 100) },
                    new() { Range = "101+", Count = components.Count(c => c.Price > 100) }
                }
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<StockStatistics> GetStockStatistics(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components.ToListAsync(cancellationToken);
            
            return new StockStatistics
            {
                TotalStock = components.Sum(c => c.StockQuantity),
                AverageStock = components.Average(c => c.StockQuantity),
                LowStockCount = components.Count(c => c.StockQuantity < c.MinimumStockLevel),
                OutOfStockCount = components.Count(c => c.StockQuantity == 0),
                OverstockedCount = components.Count(c => c.StockQuantity > c.MaximumStockLevel),
                StockRanges = new List<StockRange>
                {
                    new() { Range = "0", Count = components.Count(c => c.StockQuantity == 0) },
                    new() { Range = "1-10", Count = components.Count(c => c.StockQuantity > 0 && c.StockQuantity <= 10) },
                    new() { Range = "11-50", Count = components.Count(c => c.StockQuantity > 10 && c.StockQuantity <= 50) },
                    new() { Range = "51+", Count = components.Count(c => c.StockQuantity > 50) }
                }
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentTrends> GetComponentTrends(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components.ToListAsync(cancellationToken);
            var categories = components.GroupBy(c => c.Category);
            
            return new ComponentTrends
            {
                PriceTrends = categories.Select(g => new PriceTrend
                {
                    Category = g.Key,
                    AveragePrice = g.Average(c => c.Price),
                    MinPrice = g.Min(c => c.Price),
                    MaxPrice = g.Max(c => c.Price)
                }).ToList(),
                StockTrends = categories.Select(g => new StockTrend
                {
                    Category = g.Key,
                    TotalStock = g.Sum(c => c.StockQuantity),
                    LowStockCount = g.Count(c => c.StockQuantity < c.MinimumStockLevel),
                    OutOfStockCount = g.Count(c => c.StockQuantity == 0)
                }).ToList()
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<InventoryHealth> GetInventoryHealth(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components.ToListAsync(cancellationToken);
            var totalValue = components.Sum(c => c.Price * c.StockQuantity);
            var lowStockItems = components.Count(c => c.StockQuantity < c.MinimumStockLevel);
            var outOfStockItems = components.Count(c => c.StockQuantity == 0);
            var overstockedItems = components.Count(c => c.StockQuantity > c.MaximumStockLevel);
            
            // Calculate health score (0-100)
            var healthScore = 100;
            healthScore -= (lowStockItems * 5); // -5 points per low stock item
            healthScore -= (outOfStockItems * 10); // -10 points per out of stock item
            healthScore -= (overstockedItems * 3); // -3 points per overstocked item
            healthScore = Math.Max(0, healthScore); // Ensure score doesn't go below 0
            
            return new InventoryHealth
            {
                TotalComponents = components.Count,
                TotalValue = totalValue,
                LowStockItems = lowStockItems,
                OutOfStockItems = outOfStockItems,
                OverstockedItems = overstockedItems,
                HealthScore = healthScore
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

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<SupplierAnalytics> GetSupplierAnalytics(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var suppliers = await context.Suppliers
                .Include(s => s.Components)
                .ToListAsync(cancellationToken);

            return new SupplierAnalytics
            {
                TotalSuppliers = suppliers.Count,
                TotalComponents = suppliers.Sum(s => s.Components.Count),
                AverageComponentsPerSupplier = suppliers.Average(s => s.Components.Count),
                TopSuppliers = suppliers
                    .OrderByDescending(s => s.Components.Count)
                    .Take(5)
                    .Select(s => new SupplierSummary
                    {
                        Name = s.Name,
                        ComponentCount = s.Components.Count,
                        TotalValue = s.Components.Sum(c => c.Price * c.StockQuantity),
                        AveragePrice = s.Components.Average(c => c.Price)
                    })
                    .ToList()
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentLifecycle> GetComponentLifecycle(
            int componentId,
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var component = await context.Components
                .Include(c => c.PriceHistory)
                .Include(c => c.StockHistory)
                .FirstOrDefaultAsync(c => c.Id == componentId, cancellationToken);

            if (component == null)
                throw new GraphQLException($"Component with ID {componentId} not found.");

            return new ComponentLifecycle
            {
                ComponentId = component.Id,
                ComponentName = component.Name,
                CurrentStatus = component.Status,
                PriceHistory = component.PriceHistory
                    .OrderByDescending(p => p.ChangedAt)
                    .Select(p => new PriceHistoryEntry
                    {
                        OldPrice = p.OldPrice,
                        NewPrice = p.NewPrice,
                        ChangedAt = p.ChangedAt,
                        ChangedBy = p.ChangedBy,
                        Reason = p.Reason
                    })
                    .ToList(),
                StockHistory = component.StockHistory
                    .OrderByDescending(s => s.ChangedAt)
                    .Select(s => new StockHistoryEntry
                    {
                        OldQuantity = s.OldQuantity,
                        NewQuantity = s.NewQuantity,
                        ChangedAt = s.ChangedAt,
                        ChangedBy = s.ChangedBy,
                        Reason = s.Reason
                    })
                    .ToList(),
                LastRestockDate = component.LastRestockDate,
                LastPriceUpdateDate = component.LastPriceUpdateDate,
                DaysSinceLastRestock = component.LastRestockDate.HasValue 
                    ? (DateTime.UtcNow - component.LastRestockDate.Value).Days 
                    : null,
                DaysSinceLastPriceUpdate = component.LastPriceUpdateDate.HasValue 
                    ? (DateTime.UtcNow - component.LastPriceUpdateDate.Value).Days 
                    : null
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<SupplierPerformance> GetSupplierPerformance(
            int supplierId,
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var supplier = await context.Suppliers
                .Include(s => s.Components)
                .FirstOrDefaultAsync(s => s.Id == supplierId, cancellationToken);

            if (supplier == null)
                throw new GraphQLException($"Supplier with ID {supplierId} not found.");

            var components = supplier.Components;
            var totalComponents = components.Count;
            var lowStockComponents = components.Count(c => c.StockQuantity < c.MinimumStockLevel);
            var outOfStockComponents = components.Count(c => c.StockQuantity == 0);
            var overstockedComponents = components.Count(c => c.StockQuantity > c.MaximumStockLevel);

            return new SupplierPerformance
            {
                SupplierId = supplier.Id,
                SupplierName = supplier.Name,
                TotalComponents = totalComponents,
                TotalValue = components.Sum(c => c.Price * c.StockQuantity),
                AveragePrice = components.Average(c => c.Price),
                LowStockCount = lowStockComponents,
                OutOfStockCount = outOfStockComponents,
                OverstockedCount = overstockedComponents,
                HealthScore = CalculateSupplierHealthScore(
                    totalComponents,
                    lowStockComponents,
                    outOfStockComponents,
                    overstockedComponents
                ),
                ComponentBreakdown = new ComponentBreakdown
                {
                    ByCategory = components
                        .GroupBy(c => c.Category)
                        .Select(g => new CategorySummary
                        {
                            Category = g.Key,
                            Count = g.Count(),
                            TotalValue = g.Sum(c => c.Price * c.StockQuantity)
                        })
                        .ToList(),
                    ByStatus = new StatusSummary
                    {
                        Active = components.Count(c => c.Status == ComponentStatus.Active),
                        Discontinued = components.Count(c => c.Status == ComponentStatus.Discontinued),
                        OutOfStock = components.Count(c => c.Status == ComponentStatus.OutOfStock),
                        LowStock = components.Count(c => c.Status == ComponentStatus.LowStock),
                        Overstocked = components.Count(c => c.Status == ComponentStatus.Overstocked)
                    }
                }
            };
        }

        private double CalculateSupplierHealthScore(
            int totalComponents,
            int lowStockComponents,
            int outOfStockComponents,
            int overstockedComponents)
        {
            if (totalComponents == 0) return 0;

            var healthScore = 100.0;
            healthScore -= (lowStockComponents * 5.0 / totalComponents);
            healthScore -= (outOfStockComponents * 10.0 / totalComponents);
            healthScore -= (overstockedComponents * 3.0 / totalComponents);

            return Math.Max(0, healthScore);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<InventoryPredictions> GetInventoryPredictions(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components
                .Include(c => c.StockHistory)
                .ToListAsync(cancellationToken);

            var predictions = new List<StockPrediction>();
            foreach (var component in components)
            {
                var stockHistory = component.StockHistory
                    .OrderByDescending(s => s.ChangedAt)
                    .Take(10)
                    .ToList();

                if (stockHistory.Count >= 3)
                {
                    var averageConsumption = CalculateAverageConsumption(stockHistory);
                    var daysUntilLowStock = CalculateDaysUntilLowStock(component, averageConsumption);
                    var recommendedOrderQuantity = CalculateRecommendedOrderQuantity(component, averageConsumption);

                    predictions.Add(new StockPrediction
                    {
                        ComponentId = component.Id,
                        ComponentName = component.Name,
                        CurrentStock = component.StockQuantity,
                        AverageDailyConsumption = averageConsumption,
                        DaysUntilLowStock = daysUntilLowStock,
                        RecommendedOrderQuantity = recommendedOrderQuantity,
                        UrgencyLevel = CalculateUrgencyLevel(daysUntilLowStock),
                        ConfidenceScore = CalculateConfidenceScore(stockHistory.Count)
                    });
                }
            }

            return new InventoryPredictions(
                Predictions = predictions.OrderBy(p => p.DaysUntilLowStock).ToList(),
                HighUrgencyCount = predictions.Count(p => p.UrgencyLevel == UrgencyLevel.High),
                MediumUrgencyCount = predictions.Count(p => p.UrgencyLevel == UrgencyLevel.Medium),
                LowUrgencyCount = predictions.Count(p => p.UrgencyLevel == UrgencyLevel.Low)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<PricePredictions> GetPricePredictions(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components
                .Include(c => c.PriceHistory)
                .ToListAsync(cancellationToken);

            var predictions = new List<PricePrediction>();
            foreach (var component in components)
            {
                var priceHistory = component.PriceHistory
                    .OrderByDescending(p => p.ChangedAt)
                    .Take(10)
                    .ToList();

                if (priceHistory.Count >= 3)
                {
                    var priceTrend = AnalyzePriceTrend(priceHistory);
                    var predictedPrice = PredictNextPrice(component.Price, priceTrend);
                    var priceVolatility = CalculatePriceVolatility(priceHistory);

                    predictions.Add(new PricePrediction
                    {
                        ComponentId = component.Id,
                        ComponentName = component.Name,
                        CurrentPrice = component.Price,
                        PredictedPrice = predictedPrice,
                        PriceTrend = priceTrend,
                        PriceVolatility = priceVolatility,
                        ConfidenceScore = CalculatePriceConfidenceScore(priceHistory.Count, priceVolatility),
                        Recommendation = GeneratePriceRecommendation(priceTrend, priceVolatility)
                    });
                }
            }

            return new PricePredictions(
                Predictions = predictions.OrderByDescending(p => p.PriceVolatility).ToList(),
                IncreasingPriceCount = predictions.Count(p => p.PriceTrend == PriceTrend.Increasing),
                DecreasingPriceCount = predictions.Count(p => p.PriceTrend == PriceTrend.Decreasing),
                StablePriceCount = predictions.Count(p => p.PriceTrend == PriceTrend.Stable)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<InventoryRecommendations> GetInventoryRecommendations(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components
                .Include(c => c.StockHistory)
                .Include(c => c.PriceHistory)
                .ToListAsync(cancellationToken);

            var recommendations = new List<InventoryRecommendation>();
            foreach (var component in components)
            {
                var stockHistory = component.StockHistory
                    .OrderByDescending(s => s.ChangedAt)
                    .Take(10)
                    .ToList();

                var priceHistory = component.PriceHistory
                    .OrderByDescending(p => p.ChangedAt)
                    .Take(10)
                    .ToList();

                if (stockHistory.Count >= 3 && priceHistory.Count >= 3)
                {
                    var stockPrediction = GenerateStockPrediction(component, stockHistory);
                    var pricePrediction = GeneratePricePrediction(component, priceHistory);
                    var recommendation = GenerateInventoryRecommendation(
                        component,
                        stockPrediction,
                        pricePrediction
                    );

                    recommendations.Add(recommendation);
                }
            }

            return new InventoryRecommendations(
                Recommendations = recommendations.OrderByDescending(r => r.Priority).ToList(),
                HighPriorityCount = recommendations.Count(r => r.Priority == Priority.High),
                MediumPriorityCount = recommendations.Count(r => r.Priority == Priority.Medium),
                LowPriorityCount = recommendations.Count(r => r.Priority == Priority.Low)
            );
        }

        private double CalculateAverageConsumption(List<StockHistory> history)
        {
            var totalConsumption = 0;
            var days = 0;

            for (int i = 0; i < history.Count - 1; i++)
            {
                var current = history[i];
                var next = history[i + 1];
                var timeSpan = (current.ChangedAt - next.ChangedAt).TotalDays;
                
                if (timeSpan > 0)
                {
                    totalConsumption += Math.Abs(current.NewQuantity - next.NewQuantity);
                    days += (int)timeSpan;
                }
            }

            return days > 0 ? (double)totalConsumption / days : 0;
        }

        private int CalculateDaysUntilLowStock(Component component, double averageConsumption)
        {
            if (averageConsumption <= 0) return int.MaxValue;
            return (int)((component.StockQuantity - component.MinimumStockLevel) / averageConsumption);
        }

        private int CalculateRecommendedOrderQuantity(Component component, double averageConsumption)
        {
            var safetyStock = component.MinimumStockLevel;
            var leadTime = 7; // Assuming 7 days lead time
            var recommendedQuantity = (int)(averageConsumption * leadTime + safetyStock - component.StockQuantity);
            return Math.Max(0, recommendedQuantity);
        }

        private UrgencyLevel CalculateUrgencyLevel(int daysUntilLowStock)
        {
            return daysUntilLowStock switch
            {
                <= 7 => UrgencyLevel.High,
                <= 14 => UrgencyLevel.Medium,
                _ => UrgencyLevel.Low
            };
        }

        private double CalculateConfidenceScore(int dataPoints)
        {
            return Math.Min(1.0, dataPoints / 10.0);
        }

        private PriceTrend AnalyzePriceTrend(List<PriceHistory> history)
        {
            var priceChanges = new List<decimal>();
            for (int i = 0; i < history.Count - 1; i++)
            {
                priceChanges.Add(history[i].NewPrice - history[i + 1].NewPrice);
            }

            var averageChange = priceChanges.Average();
            var threshold = 0.05m; // 5% threshold

            return averageChange switch
            {
                > threshold => PriceTrend.Increasing,
                < -threshold => PriceTrend.Decreasing,
                _ => PriceTrend.Stable
            };
        }

        private decimal PredictNextPrice(decimal currentPrice, PriceTrend trend)
        {
            return trend switch
            {
                PriceTrend.Increasing => currentPrice * 1.05m,
                PriceTrend.Decreasing => currentPrice * 0.95m,
                _ => currentPrice
            };
        }

        private double CalculatePriceVolatility(List<PriceHistory> history)
        {
            var priceChanges = history
                .Select(p => (double)p.NewPrice)
                .ToList();

            if (priceChanges.Count < 2) return 0;

            var mean = priceChanges.Average();
            var variance = priceChanges.Sum(p => Math.Pow(p - mean, 2)) / (priceChanges.Count - 1);
            return Math.Sqrt(variance);
        }

        private double CalculatePriceConfidenceScore(int dataPoints, double volatility)
        {
            var baseScore = Math.Min(1.0, dataPoints / 10.0);
            var volatilityFactor = Math.Max(0, 1 - volatility / 100);
            return baseScore * volatilityFactor;
        }

        private string GeneratePriceRecommendation(PriceTrend trend, double volatility)
        {
            return trend switch
            {
                PriceTrend.Increasing => volatility > 10 
                    ? "Consider increasing prices gradually" 
                    : "Consider immediate price increase",
                PriceTrend.Decreasing => volatility > 10 
                    ? "Consider reducing prices gradually" 
                    : "Consider immediate price reduction",
                _ => "Maintain current pricing"
            };
        }

        private StockPrediction GenerateStockPrediction(Component component, List<StockHistory> history)
        {
            var averageConsumption = CalculateAverageConsumption(history);
            var daysUntilLowStock = CalculateDaysUntilLowStock(component, averageConsumption);
            var recommendedOrderQuantity = CalculateRecommendedOrderQuantity(component, averageConsumption);

            return new StockPrediction
            {
                ComponentId = component.Id,
                ComponentName = component.Name,
                CurrentStock = component.StockQuantity,
                AverageDailyConsumption = averageConsumption,
                DaysUntilLowStock = daysUntilLowStock,
                RecommendedOrderQuantity = recommendedOrderQuantity,
                UrgencyLevel = CalculateUrgencyLevel(daysUntilLowStock),
                ConfidenceScore = CalculateConfidenceScore(history.Count)
            };
        }

        private PricePrediction GeneratePricePrediction(Component component, List<PriceHistory> history)
        {
            var priceTrend = AnalyzePriceTrend(history);
            var predictedPrice = PredictNextPrice(component.Price, priceTrend);
            var priceVolatility = CalculatePriceVolatility(history);

            return new PricePrediction
            {
                ComponentId = component.Id,
                ComponentName = component.Name,
                CurrentPrice = component.Price,
                PredictedPrice = predictedPrice,
                PriceTrend = priceTrend,
                PriceVolatility = priceVolatility,
                ConfidenceScore = CalculatePriceConfidenceScore(history.Count, priceVolatility),
                Recommendation = GeneratePriceRecommendation(priceTrend, priceVolatility)
            };
        }

        private InventoryRecommendation GenerateInventoryRecommendation(
            Component component,
            StockPrediction stockPrediction,
            PricePrediction pricePrediction)
        {
            var priority = CalculateRecommendationPriority(stockPrediction, pricePrediction);
            var action = GenerateRecommendationAction(stockPrediction, pricePrediction);
            var reasoning = GenerateRecommendationReasoning(stockPrediction, pricePrediction);

            return new InventoryRecommendation
            {
                ComponentId = component.Id,
                ComponentName = component.Name,
                Priority = priority,
                Action = action,
                Reasoning = reasoning,
                StockPrediction = stockPrediction,
                PricePrediction = pricePrediction
            };
        }

        private Priority CalculateRecommendationPriority(
            StockPrediction stockPrediction,
            PricePrediction pricePrediction)
        {
            var stockUrgency = stockPrediction.UrgencyLevel switch
            {
                UrgencyLevel.High => 3,
                UrgencyLevel.Medium => 2,
                _ => 1
            };

            var priceUrgency = pricePrediction.PriceTrend switch
            {
                PriceTrend.Increasing => 2,
                PriceTrend.Decreasing => 1,
                _ => 0
            };

            var totalScore = stockUrgency + priceUrgency;
            return totalScore switch
            {
                >= 4 => Priority.High,
                >= 2 => Priority.Medium,
                _ => Priority.Low
            };
        }

        private string GenerateRecommendationAction(
            StockPrediction stockPrediction,
            PricePrediction pricePrediction)
        {
            var actions = new List<string>();

            if (stockPrediction.UrgencyLevel == UrgencyLevel.High)
                actions.Add($"Order {stockPrediction.RecommendedOrderQuantity} units");

            if (pricePrediction.PriceTrend == PriceTrend.Increasing)
                actions.Add("Consider price increase");

            if (pricePrediction.PriceTrend == PriceTrend.Decreasing)
                actions.Add("Consider price reduction");

            return string.Join(" and ", actions);
        }

        private string GenerateRecommendationReasoning(
            StockPrediction stockPrediction,
            PricePrediction pricePrediction)
        {
            var reasons = new List<string>();

            if (stockPrediction.DaysUntilLowStock <= 7)
                reasons.Add($"Stock will be low in {stockPrediction.DaysUntilLowStock} days");

            if (pricePrediction.PriceTrend != PriceTrend.Stable)
                reasons.Add($"Price is {pricePrediction.PriceTrend.ToString().ToLower()}");

            return string.Join("; ", reasons);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<MLPredictions> GetMLPredictions(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components
                .Include(c => c.StockHistory)
                .Include(c => c.PriceHistory)
                .ToListAsync(cancellationToken);

            var predictions = new List<MLPrediction>();
            foreach (var component in components)
            {
                var stockHistory = component.StockHistory
                    .OrderByDescending(s => s.ChangedAt)
                    .Take(30)
                    .ToList();

                var priceHistory = component.PriceHistory
                    .OrderByDescending(p => p.ChangedAt)
                    .Take(30)
                    .ToList();

                if (stockHistory.Count >= 10 && priceHistory.Count >= 10)
                {
                    var seasonalPattern = DetectSeasonalPattern(stockHistory);
                    var demandForecast = ForecastDemand(stockHistory, seasonalPattern);
                    var priceForecast = ForecastPrice(priceHistory);
                    var optimalPrice = CalculateOptimalPrice(component, demandForecast, priceForecast);
                    var optimalStock = CalculateOptimalStock(component, demandForecast, seasonalPattern);

                    predictions.Add(new MLPrediction
                    {
                        ComponentId = component.Id,
                        ComponentName = component.Name,
                        SeasonalPattern = seasonalPattern,
                        DemandForecast = demandForecast,
                        PriceForecast = priceForecast,
                        OptimalPrice = optimalPrice,
                        OptimalStock = optimalStock,
                        ConfidenceScore = CalculateMLConfidenceScore(stockHistory.Count, priceHistory.Count),
                        Recommendations = GenerateMLRecommendations(
                            component,
                            demandForecast,
                            priceForecast,
                            optimalPrice,
                            optimalStock
                        )
                    });
                }
            }

            return new MLPredictions(
                Predictions = predictions.OrderByDescending(p => p.ConfidenceScore).ToList(),
                HighConfidenceCount = predictions.Count(p => p.ConfidenceScore >= 0.8),
                MediumConfidenceCount = predictions.Count(p => p.ConfidenceScore >= 0.5 && p.ConfidenceScore < 0.8),
                LowConfidenceCount = predictions.Count(p => p.ConfidenceScore < 0.5)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<OptimizationResults> GetOptimizationResults(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components
                .Include(c => c.StockHistory)
                .Include(c => c.PriceHistory)
                .ToListAsync(cancellationToken);

            var results = new List<OptimizationResult>();
            foreach (var component in components)
            {
                var stockHistory = component.StockHistory
                    .OrderByDescending(s => s.ChangedAt)
                    .Take(30)
                    .ToList();

                var priceHistory = component.PriceHistory
                    .OrderByDescending(p => p.ChangedAt)
                    .Take(30)
                    .ToList();

                if (stockHistory.Count >= 10 && priceHistory.Count >= 10)
                {
                    var optimization = OptimizeComponent(component, stockHistory, priceHistory);
                    results.Add(optimization);
                }
            }

            return new OptimizationResults(
                Results = results.OrderByDescending(r => r.PotentialSavings).ToList(),
                TotalPotentialSavings = results.Sum(r => r.PotentialSavings),
                HighImpactCount = results.Count(r => r.ImpactLevel == ImpactLevel.High),
                MediumImpactCount = results.Count(r => r.ImpactLevel == ImpactLevel.Medium),
                LowImpactCount = results.Count(r => r.ImpactLevel == ImpactLevel.Low)
            );
        }

        private SeasonalPattern DetectSeasonalPattern(List<StockHistory> history)
        {
            var dailyConsumption = new Dictionary<DayOfWeek, List<int>>();
            foreach (var record in history)
            {
                var dayOfWeek = record.ChangedAt.DayOfWeek;
                if (!dailyConsumption.ContainsKey(dayOfWeek))
                    dailyConsumption[dayOfWeek] = new List<int>();

                dailyConsumption[dayOfWeek].Add(Math.Abs(record.NewQuantity - record.OldQuantity));
            }

            var pattern = new SeasonalPattern();
            foreach (var day in dailyConsumption)
            {
                pattern.DailyPattern[day.Key] = day.Value.Average();
            }

            return pattern;
        }

        private DemandForecast ForecastDemand(List<StockHistory> history, SeasonalPattern pattern)
        {
            var averageConsumption = CalculateAverageConsumption(history);
            var dailyForecasts = new Dictionary<DayOfWeek, double>();

            foreach (var day in pattern.DailyPattern)
            {
                dailyForecasts[day.Key] = averageConsumption * day.Value;
            }

            return new DemandForecast
            {
                AverageDailyDemand = averageConsumption,
                DailyForecasts = dailyForecasts,
                WeeklyForecast = dailyForecasts.Values.Sum(),
                MonthlyForecast = dailyForecasts.Values.Sum() * 30
            };
        }

        private PriceForecast ForecastPrice(List<PriceHistory> history)
        {
            var priceChanges = new List<decimal>();
            for (int i = 0; i < history.Count - 1; i++)
            {
                priceChanges.Add(history[i].NewPrice - history[i + 1].NewPrice);
            }

            var averageChange = priceChanges.Average();
            var volatility = CalculatePriceVolatility(history);

            return new PriceForecast
            {
                CurrentPrice = history.First().NewPrice,
                PredictedChange = averageChange,
                PredictedVolatility = volatility,
                ConfidenceInterval = new ConfidenceInterval
                {
                    LowerBound = history.First().NewPrice * (1 - volatility),
                    UpperBound = history.First().NewPrice * (1 + volatility)
                }
            };
        }

        private decimal CalculateOptimalPrice(
            Component component,
            DemandForecast demandForecast,
            PriceForecast priceForecast)
        {
            var elasticity = CalculatePriceElasticity(component, demandForecast);
            var currentPrice = component.Price;
            var optimalPrice = currentPrice;

            if (elasticity < -1) // Elastic demand
            {
                optimalPrice = currentPrice * 0.95m; // Slight price decrease
            }
            else if (elasticity > -1) // Inelastic demand
            {
                optimalPrice = currentPrice * 1.05m; // Slight price increase
            }

            return Math.Max(optimalPrice, component.Price * 0.5m); // Ensure minimum 50% of current price
        }

        private int CalculateOptimalStock(
            Component component,
            DemandForecast demandForecast,
            SeasonalPattern pattern)
        {
            var safetyStock = component.MinimumStockLevel;
            var leadTime = 7; // Assuming 7 days lead time
            var maxDailyDemand = pattern.DailyPattern.Values.Max();
            
            return (int)(maxDailyDemand * leadTime + safetyStock);
        }

        private double CalculatePriceElasticity(
            Component component,
            DemandForecast demandForecast)
        {
            // Simplified elasticity calculation
            // In a real system, this would use more sophisticated methods
            return -1.5; // Assuming slightly elastic demand
        }

        private double CalculateMLConfidenceScore(int stockDataPoints, int priceDataPoints)
        {
            var stockScore = Math.Min(1.0, stockDataPoints / 30.0);
            var priceScore = Math.Min(1.0, priceDataPoints / 30.0);
            return (stockScore + priceScore) / 2;
        }

        private List<string> GenerateMLRecommendations(
            Component component,
            DemandForecast demandForecast,
            PriceForecast priceForecast,
            decimal optimalPrice,
            int optimalStock)
        {
            var recommendations = new List<string>();

            if (Math.Abs(component.Price - optimalPrice) / component.Price > 0.1m)
            {
                recommendations.Add($"Consider adjusting price to {optimalPrice:C}");
            }

            if (Math.Abs(component.StockQuantity - optimalStock) > component.MinimumStockLevel)
            {
                recommendations.Add($"Adjust stock level to {optimalStock} units");
            }

            if (demandForecast.WeeklyForecast > component.StockQuantity)
            {
                recommendations.Add("Increase stock levels to meet forecasted demand");
            }

            if (priceForecast.PredictedVolatility > 0.1)
            {
                recommendations.Add("Monitor price volatility closely");
            }

            return recommendations;
        }

        private OptimizationResult OptimizeComponent(
            Component component,
            List<StockHistory> stockHistory,
            List<PriceHistory> priceHistory)
        {
            var currentValue = component.Price * component.StockQuantity;
            var optimalPrice = CalculateOptimalPrice(
                component,
                ForecastDemand(stockHistory, DetectSeasonalPattern(stockHistory)),
                ForecastPrice(priceHistory)
            );
            var optimalStock = CalculateOptimalStock(
                component,
                ForecastDemand(stockHistory, DetectSeasonalPattern(stockHistory)),
                DetectSeasonalPattern(stockHistory)
            );
            var optimizedValue = optimalPrice * optimalStock;
            var potentialSavings = currentValue - optimizedValue;

            return new OptimizationResult
            {
                ComponentId = component.Id,
                ComponentName = component.Name,
                CurrentValue = currentValue,
                OptimizedValue = optimizedValue,
                PotentialSavings = potentialSavings,
                ImpactLevel = CalculateImpactLevel(potentialSavings, currentValue),
                Recommendations = GenerateOptimizationRecommendations(
                    component,
                    optimalPrice,
                    optimalStock,
                    potentialSavings
                )
            };
        }

        private ImpactLevel CalculateImpactLevel(decimal potentialSavings, decimal currentValue)
        {
            var savingsPercentage = potentialSavings / currentValue;
            return savingsPercentage switch
            {
                >= 0.2m => ImpactLevel.High,
                >= 0.1m => ImpactLevel.Medium,
                _ => ImpactLevel.Low
            };
        }

        private List<string> GenerateOptimizationRecommendations(
            Component component,
            decimal optimalPrice,
            int optimalStock,
            decimal potentialSavings)
        {
            var recommendations = new List<string>();

            if (Math.Abs(component.Price - optimalPrice) / component.Price > 0.1m)
            {
                recommendations.Add($"Adjust price from {component.Price:C} to {optimalPrice:C}");
            }

            if (Math.Abs(component.StockQuantity - optimalStock) > component.MinimumStockLevel)
            {
                recommendations.Add($"Adjust stock from {component.StockQuantity} to {optimalStock} units");
            }

            if (potentialSavings > 0)
            {
                recommendations.Add($"Potential savings: {potentialSavings:C}");
            }

            return recommendations;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<RealTimeMonitoring> GetRealTimeMonitoring(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components
                .Include(c => c.StockHistory)
                .Include(c => c.PriceHistory)
                .ToListAsync(cancellationToken);

            var monitoringData = new List<ComponentMonitoring>();
            foreach (var component in components)
            {
                var stockHistory = component.StockHistory
                    .OrderByDescending(s => s.ChangedAt)
                    .Take(10)
                    .ToList();

                var priceHistory = component.PriceHistory
                    .OrderByDescending(p => p.ChangedAt)
                    .Take(10)
                    .ToList();

                var stockVelocity = CalculateStockVelocity(stockHistory);
                var priceVelocity = CalculatePriceVelocity(priceHistory);
                var alerts = GenerateAlerts(component, stockVelocity, priceVelocity);

                monitoringData.Add(new ComponentMonitoring
                {
                    ComponentId = component.Id,
                    ComponentName = component.Name,
                    CurrentStock = component.StockQuantity,
                    CurrentPrice = component.Price,
                    StockVelocity = stockVelocity,
                    PriceVelocity = priceVelocity,
                    StockTrend = AnalyzeStockTrend(stockHistory),
                    PriceTrend = AnalyzePriceTrend(priceHistory),
                    Alerts = alerts,
                    LastUpdated = DateTime.UtcNow
                });
            }

            return new RealTimeMonitoring
            {
                Components = monitoringData,
                CriticalAlerts = monitoringData.Count(c => c.Alerts.Any(a => a.Severity == AlertSeverity.Critical)),
                WarningAlerts = monitoringData.Count(c => c.Alerts.Any(a => a.Severity == AlertSeverity.Warning)),
                InfoAlerts = monitoringData.Count(c => c.Alerts.Any(a => a.Severity == AlertSeverity.Info))
            };
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<AutomatedAlerts> GetAutomatedAlerts(
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var components = await context.Components
                .Include(c => c.StockHistory)
                .Include(c => c.PriceHistory)
                .ToListAsync(cancellationToken);

            var alerts = new List<Alert>();
            foreach (var component in components)
            {
                var stockHistory = component.StockHistory
                    .OrderByDescending(s => s.ChangedAt)
                    .Take(10)
                    .ToList();

                var priceHistory = component.PriceHistory
                    .OrderByDescending(p => p.ChangedAt)
                    .Take(10)
                    .ToList();

                var stockAlerts = GenerateStockAlerts(component, stockHistory);
                var priceAlerts = GeneratePriceAlerts(component, priceHistory);
                var trendAlerts = GenerateTrendAlerts(component, stockHistory, priceHistory);

                alerts.AddRange(stockAlerts);
                alerts.AddRange(priceAlerts);
                alerts.AddRange(trendAlerts);
            }

            return new AutomatedAlerts
            {
                Alerts = alerts.OrderByDescending(a => a.Severity).ToList(),
                CriticalCount = alerts.Count(a => a.Severity == AlertSeverity.Critical),
                WarningCount = alerts.Count(a => a.Severity == AlertSeverity.Warning),
                InfoCount = alerts.Count(a => a.Severity == AlertSeverity.Info)
            };
        }

        private double CalculateStockVelocity(List<StockHistory> history)
        {
            if (history.Count < 2) return 0;

            var changes = new List<double>();
            for (int i = 0; i < history.Count - 1; i++)
            {
                var timeSpan = (history[i].ChangedAt - history[i + 1].ChangedAt).TotalHours;
                if (timeSpan > 0)
                {
                    var change = (double)(history[i].NewQuantity - history[i + 1].NewQuantity) / timeSpan;
                    changes.Add(change);
                }
            }

            return changes.Any() ? changes.Average() : 0;
        }

        private decimal CalculatePriceVelocity(List<PriceHistory> history)
        {
            if (history.Count < 2) return 0;

            var changes = new List<decimal>();
            for (int i = 0; i < history.Count - 1; i++)
            {
                var timeSpan = (history[i].ChangedAt - history[i + 1].ChangedAt).TotalHours;
                if (timeSpan > 0)
                {
                    var change = (history[i].NewPrice - history[i + 1].NewPrice) / (decimal)timeSpan;
                    changes.Add(change);
                }
            }

            return changes.Any() ? changes.Average() : 0;
        }

        private List<Alert> GenerateAlerts(
            Component component,
            double stockVelocity,
            decimal priceVelocity)
        {
            var alerts = new List<Alert>();

            // Stock alerts
            if (component.StockQuantity <= component.MinimumStockLevel)
            {
                alerts.Add(new Alert
                {
                    ComponentId = component.Id,
                    ComponentName = component.Name,
                    Type = AlertType.Stock,
                    Message = $"Critical stock level reached: {component.StockQuantity} units",
                    Severity = AlertSeverity.Critical,
                    Timestamp = DateTime.UtcNow
                });
            }
            else if (stockVelocity < -5) // Rapid stock decrease
            {
                alerts.Add(new Alert
                {
                    ComponentId = component.Id,
                    ComponentName = component.Name,
                    Type = AlertType.Stock,
                    Message = $"Rapid stock decrease detected: {stockVelocity:F2} units/hour",
                    Severity = AlertSeverity.Warning,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Price alerts
            if (priceVelocity > 0.1m) // Rapid price increase
            {
                alerts.Add(new Alert
                {
                    ComponentId = component.Id,
                    ComponentName = component.Name,
                    Type = AlertType.Price,
                    Message = $"Rapid price increase detected: {priceVelocity:C}/hour",
                    Severity = AlertSeverity.Warning,
                    Timestamp = DateTime.UtcNow
                });
            }
            else if (priceVelocity < -0.1m) // Rapid price decrease
            {
                alerts.Add(new Alert
                {
                    ComponentId = component.Id,
                    ComponentName = component.Name,
                    Type = AlertType.Price,
                    Message = $"Rapid price decrease detected: {priceVelocity:C}/hour",
                    Severity = AlertSeverity.Info,
                    Timestamp = DateTime.UtcNow
                });
            }

            return alerts;
        }

        private List<Alert> GenerateStockAlerts(Component component, List<StockHistory> history)
        {
            var alerts = new List<Alert>();

            if (history.Count >= 3)
            {
                var recentStock = history.Take(3).Select(h => h.NewQuantity).ToList();
                var stockTrend = AnalyzeStockTrend(history);

                if (stockTrend == StockTrend.RapidDecrease && component.StockQuantity <= component.MinimumStockLevel)
                {
                    alerts.Add(new Alert
                    {
                        ComponentId = component.Id,
                        ComponentName = component.Name,
                        Type = AlertType.Stock,
                        Message = "Critical: Rapid stock decrease with low inventory",
                        Severity = AlertSeverity.Critical,
                        Timestamp = DateTime.UtcNow
                    });
                }
                else if (stockTrend == StockTrend.Decrease && component.StockQuantity <= component.MinimumStockLevel * 1.5)
                {
                    alerts.Add(new Alert
                    {
                        ComponentId = component.Id,
                        ComponentName = component.Name,
                        Type = AlertType.Stock,
                        Message = "Warning: Stock decreasing with moderate inventory",
                        Severity = AlertSeverity.Warning,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            return alerts;
        }

        private List<Alert> GeneratePriceAlerts(Component component, List<PriceHistory> history)
        {
            var alerts = new List<Alert>();

            if (history.Count >= 3)
            {
                var recentPrices = history.Take(3).Select(h => h.NewPrice).ToList();
                var priceTrend = AnalyzePriceTrend(history);
                var volatility = CalculatePriceVolatility(history);

                if (priceTrend == PriceTrend.Increasing && volatility > 0.1)
                {
                    alerts.Add(new Alert
                    {
                        ComponentId = component.Id,
                        ComponentName = component.Name,
                        Type = AlertType.Price,
                        Message = "Warning: High price volatility with increasing trend",
                        Severity = AlertSeverity.Warning,
                        Timestamp = DateTime.UtcNow
                    });
                }
                else if (priceTrend == PriceTrend.Decreasing && volatility > 0.1)
                {
                    alerts.Add(new Alert
                    {
                        ComponentId = component.Id,
                        ComponentName = component.Name,
                        Type = AlertType.Price,
                        Message = "Info: High price volatility with decreasing trend",
                        Severity = AlertSeverity.Info,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            return alerts;
        }

        private List<Alert> GenerateTrendAlerts(
            Component component,
            List<StockHistory> stockHistory,
            List<PriceHistory> priceHistory)
        {
            var alerts = new List<Alert>();

            if (stockHistory.Count >= 3 && priceHistory.Count >= 3)
            {
                var stockTrend = AnalyzeStockTrend(stockHistory);
                var priceTrend = AnalyzePriceTrend(priceHistory);

                if (stockTrend == StockTrend.RapidDecrease && priceTrend == PriceTrend.Increasing)
                {
                    alerts.Add(new Alert
                    {
                        ComponentId = component.Id,
                        ComponentName = component.Name,
                        Type = AlertType.Trend,
                        Message = "Critical: Stock rapidly decreasing while prices are increasing",
                        Severity = AlertSeverity.Critical,
                        Timestamp = DateTime.UtcNow
                    });
                }
                else if (stockTrend == StockTrend.Increase && priceTrend == PriceTrend.Decreasing)
                {
                    alerts.Add(new Alert
                    {
                        ComponentId = component.Id,
                        ComponentName = component.Name,
                        Type = AlertType.Trend,
                        Message = "Info: Stock increasing while prices are decreasing",
                        Severity = AlertSeverity.Info,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            return alerts;
        }

        private StockTrend AnalyzeStockTrend(List<StockHistory> history)
        {
            // Implementation of AnalyzeStockTrend method
            // This is a placeholder and should be implemented based on your specific requirements
            return StockTrend.Stable; // Placeholder return, actual implementation needed
        }

        private PriceTrend AnalyzePriceTrend(List<PriceHistory> history)
        {
            // Implementation of AnalyzePriceTrend method
            // This is a placeholder and should be implemented based on your specific requirements
            return PriceTrend.Stable; // Placeholder return, actual implementation needed
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentDependency>> GetComponentDependencies(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentDependencies
                .Include(d => d.DependentComponent)
                .Where(d => d.ComponentId == componentId)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentCompatibility>> GetComponentCompatibilities(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentCompatibilities
                .Include(c => c.CompatibleComponent)
                .Where(c => c.ComponentId == componentId)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentSpecification>> GetComponentSpecifications(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentSpecifications
                .Where(s => s.ComponentId == componentId)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentDocumentation>> GetComponentDocumentation(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentDocumentation
                .Where(d => d.ComponentId == componentId)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentReview>> GetComponentReviews(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentReviews
                .Where(r => r.ComponentId == componentId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentRating> GetComponentRating(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentRatings
                .FirstOrDefaultAsync(r => r.ComponentId == componentId);
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentTag>> GetComponentTags(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentTags
                .Where(t => t.ComponentId == componentId)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentImage>> GetComponentImages(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentImages
                .Where(i => i.ComponentId == componentId)
                .OrderBy(i => i.Type)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentVideo>> GetComponentVideos(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentVideos
                .Where(v => v.ComponentId == componentId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentAttachment>> GetComponentAttachments(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentAttachments
                .Where(a => a.ComponentId == componentId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentNote>> GetComponentNotes(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentNotes
                .Where(n => n.ComponentId == componentId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentAlert>> GetComponentAlerts(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentAlerts
                .Where(a => a.ComponentId == componentId && a.IsActive)
                .OrderByDescending(a => a.Severity)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentEvent>> GetComponentEvents(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentEvents
                .Where(e => e.ComponentId == componentId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<ComponentAudit>> GetComponentAuditTrail(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            return await context.ComponentAudits
                .Where(a => a.ComponentId == componentId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByTag(
            string tagName,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Tags)
                .Where(c => c.Tags.Any(t => t.Name == tagName))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByRating(
            double minRating,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Rating)
                .Where(c => c.Rating.AverageRating >= minRating)
                .OrderByDescending(c => c.Rating.AverageRating)
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsBySpecification(
            string specName,
            string specValue,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Specifications)
                .Where(c => c.Specifications.Any(s => 
                    s.Name == specName && s.Value == specValue))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByDocumentationType(
            DocumentationType docType,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Documentation)
                .Where(c => c.Documentation.Any(d => d.Type == docType))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsWithActiveAlerts(
            AlertSeverity severity,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Alerts)
                .Where(c => c.Alerts.Any(a => 
                    a.IsActive && a.Severity == severity))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByEventType(
            string eventType,
            DateTime startDate,
            DateTime endDate,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Events)
                .Where(c => c.Events.Any(e => 
                    e.EventType == eventType &&
                    e.CreatedAt >= startDate &&
                    e.CreatedAt <= endDate))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByAuditAction(
            string action,
            DateTime startDate,
            DateTime endDate,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.AuditTrail)
                .Where(c => c.AuditTrail.Any(a => 
                    a.Action == action &&
                    a.CreatedAt >= startDate &&
                    a.CreatedAt <= endDate))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByImageType(
            ImageType imageType,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Images)
                .Where(c => c.Images.Any(i => i.Type == imageType))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByVideoType(
            VideoType videoType,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Videos)
                .Where(c => c.Videos.Any(v => v.Type == videoType))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByAttachmentType(
            string fileType,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Attachments)
                .Where(c => c.Attachments.Any(a => a.FileType == fileType))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByNoteContent(
            string searchTerm,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Notes)
                .Where(c => c.Notes.Any(n => 
                    n.Content.Contains(searchTerm)))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByDependencyType(
            DependencyType dependencyType,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.Dependencies)
                .Where(c => c.Dependencies.Any(d => d.Type == dependencyType))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> GetComponentsByCompatibilityType(
            CompatibilityType compatibilityType,
            [Service] ApplicationDbContext context)
        {
            return await context.Components
                .Include(c => c.CompatibleComponents)
                .Where(c => c.CompatibleComponents.Any(cc => 
                    cc.Type == compatibilityType))
                .ToListAsync();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<DependencyChainAnalysis> AnalyzeDependencyChain(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components
                .Include(c => c.Dependencies)
                    .ThenInclude(d => d.DependentComponent)
                .FirstOrDefaultAsync(c => c.Id == componentId);

            if (component == null)
                throw new GraphQLException($"Component with ID {componentId} not found.");

            var dependencyChain = new List<DependencyNode>();
            var visited = new HashSet<int>();
            var circularDependencies = new List<CircularDependency>();

            void BuildDependencyChain(Component comp, int depth = 0)
            {
                if (visited.Contains(comp.Id))
                {
                    circularDependencies.Add(new CircularDependency(
                        comp.Id,
                        comp.Name,
                        depth
                    ));
                    return;
                }

                visited.Add(comp.Id);
                dependencyChain.Add(new DependencyNode(
                    comp.Id,
                    comp.Name,
                    depth,
                    comp.Dependencies.Select(d => new DependencyInfo(
                        d.DependentComponentId,
                        d.DependentComponent.Name,
                        d.Type,
                        d.IsRequired,
                        d.Quantity
                    )).ToList()
                ));

                foreach (var dep in comp.Dependencies)
                {
                    BuildDependencyChain(dep.DependentComponent, depth + 1);
                }

                visited.Remove(comp.Id);
            }

            BuildDependencyChain(component);

            return new DependencyChainAnalysis(
                componentId,
                component.Name,
                dependencyChain,
                circularDependencies,
                dependencyChain.Count,
                circularDependencies.Count
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<CompatibilityMatrix> GenerateCompatibilityMatrix(
            List<int> componentIds,
            [Service] ApplicationDbContext context)
        {
            var components = await context.Components
                .Include(c => c.CompatibleComponents)
                    .ThenInclude(cc => cc.CompatibleComponent)
                .Where(c => componentIds.Contains(c.Id))
                .ToListAsync();

            var matrix = new List<CompatibilityEntry>();
            var compatibilityTypes = Enum.GetValues(typeof(CompatibilityType))
                .Cast<CompatibilityType>()
                .ToList();

            foreach (var comp1 in components)
            {
                foreach (var comp2 in components.Where(c => c.Id > comp1.Id))
                {
                    var compatibility = comp1.CompatibleComponents
                        .FirstOrDefault(cc => cc.CompatibleComponentId == comp2.Id);

                    matrix.Add(new CompatibilityEntry(
                        comp1.Id,
                        comp1.Name,
                        comp2.Id,
                        comp2.Name,
                        compatibility?.Type ?? CompatibilityType.None,
                        compatibility?.Description ?? "No compatibility information"
                    ));
                }
            }

            return new CompatibilityMatrix(
                components.Select(c => new ComponentInfo(c.Id, c.Name)).ToList(),
                matrix,
                compatibilityTypes
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<DependencyImpactAnalysis> AnalyzeDependencyImpact(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components
                .Include(c => c.Dependencies)
                    .ThenInclude(d => d.DependentComponent)
                .Include(c => c.DependentComponents)
                    .ThenInclude(d => d.Component)
                .FirstOrDefaultAsync(c => c.Id == componentId);

            if (component == null)
                throw new GraphQLException($"Component with ID {componentId} not found.");

            var directDependencies = component.Dependencies
                .Select(d => new DependencyImpact(
                    d.DependentComponentId,
                    d.DependentComponent.Name,
                    d.Type,
                    d.IsRequired,
                    d.Quantity,
                    "Direct"
                )).ToList();

            var dependentComponents = component.DependentComponents
                .Select(d => new DependencyImpact(
                    d.ComponentId,
                    d.Component.Name,
                    d.Type,
                    d.IsRequired,
                    d.Quantity,
                    "Reverse"
                )).ToList();

            var totalImpact = directDependencies.Count + dependentComponents.Count;
            var criticalDependencies = directDependencies
                .Count(d => d.IsRequired && d.Type == DependencyType.Required);

            return new DependencyImpactAnalysis(
                componentId,
                component.Name,
                directDependencies,
                dependentComponents,
                totalImpact,
                criticalDependencies
            );
        }

        public record RealTimeMonitoring(
            List<ComponentMonitoring> Components,
            int CriticalAlerts,
            int WarningAlerts,
            int InfoAlerts);

        public record ComponentMonitoring(
            int ComponentId,
            string ComponentName,
            int CurrentStock,
            decimal CurrentPrice,
            double StockVelocity,
            decimal PriceVelocity,
            StockTrend StockTrend,
            PriceTrend PriceTrend,
            List<Alert> Alerts,
            DateTime LastUpdated);

        public record AutomatedAlerts(
            List<Alert> Alerts,
            int CriticalCount,
            int WarningCount,
            int InfoCount);

        public record Alert(
            int ComponentId,
            string ComponentName,
            AlertType Type,
            string Message,
            AlertSeverity Severity,
            DateTime Timestamp);

        public enum AlertType
        {
            Stock,
            Price,
            Trend
        }

        public enum AlertSeverity
        {
            Info,
            Warning,
            Critical
        }

        public enum StockTrend
        {
            RapidDecrease,
            Decrease,
            Stable,
            Increase,
            RapidIncrease
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<QualityControlReport> GetQualityControlReport(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components
                .Include(c => c.QualityTests)
                .Include(c => c.TestResults)
                .FirstOrDefaultAsync(c => c.Id == componentId);

            if (component == null)
                throw new GraphQLException($"Component with ID {componentId} not found.");

            var testResults = component.TestResults
                .OrderByDescending(t => t.TestDate)
                .ToList();

            var qualityMetrics = new QualityMetrics
            {
                TotalTests = testResults.Count,
                PassedTests = testResults.Count(t => t.Status == TestStatus.Passed),
                FailedTests = testResults.Count(t => t.Status == TestStatus.Failed),
                AverageScore = testResults.Average(t => t.Score),
                LastTestDate = testResults.FirstOrDefault()?.TestDate,
                QualityScore = CalculateQualityScore(testResults)
            };

            var testHistory = testResults
                .Select(t => new TestHistoryEntry(
                    t.TestId,
                    t.TestName,
                    t.Status,
                    t.Score,
                    t.TestDate,
                    t.Notes
                ))
                .ToList();

            return new QualityControlReport(
                componentId,
                component.Name,
                qualityMetrics,
                testHistory,
                GenerateQualityRecommendations(qualityMetrics, testHistory)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<TestResults> GetComponentTestResults(
            int componentId,
            DateTime startDate,
            DateTime endDate,
            [Service] ApplicationDbContext context)
        {
            var testResults = await context.TestResults
                .Where(t => t.ComponentId == componentId &&
                            t.TestDate >= startDate &&
                            t.TestDate <= endDate)
                .OrderByDescending(t => t.TestDate)
                .ToListAsync();

            var testCategories = testResults
                .GroupBy(t => t.TestCategory)
                .Select(g => new TestCategorySummary(
                    g.Key,
                    g.Count(),
                    g.Count(t => t.Status == TestStatus.Passed),
                    g.Average(t => t.Score)
                ))
                .ToList();

            return new TestResults(
                componentId,
                testResults.Count,
                testResults.Count(t => t.Status == TestStatus.Passed),
                testResults.Count(t => t.Status == TestStatus.Failed),
                testResults.Average(t => t.Score),
                testCategories,
                testResults.Select(t => new TestResultDetail(
                    t.TestId,
                    t.TestName,
                    t.TestCategory,
                    t.Status,
                    t.Score,
                    t.TestDate,
                    t.Notes
                )).ToList()
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<QualityTrends> GetQualityTrends(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            var testResults = await context.TestResults
                .Where(t => t.ComponentId == componentId)
                .OrderBy(t => t.TestDate)
                .ToListAsync();

            var monthlyTrends = testResults
                .GroupBy(t => new { t.TestDate.Year, t.TestDate.Month })
                .Select(g => new QualityTrendEntry(
                    new DateTime(g.Key.Year, g.Key.Month, 1),
                    g.Count(),
                    g.Count(t => t.Status == TestStatus.Passed),
                    g.Average(t => t.Score)
                ))
                .OrderBy(t => t.Month)
                .ToList();

            var categoryTrends = testResults
                .GroupBy(t => t.TestCategory)
                .Select(g => new CategoryTrendEntry(
                    g.Key,
                    g.Count(),
                    g.Count(t => t.Status == TestStatus.Passed),
                    g.Average(t => t.Score)
                ))
                .ToList();

            return new QualityTrends(
                componentId,
                monthlyTrends,
                categoryTrends,
                CalculateQualityImprovement(monthlyTrends)
            );
        }

        private double CalculateQualityScore(List<TestResult> testResults)
        {
            if (!testResults.Any()) return 0;

            var recentTests = testResults.Take(10).ToList();
            var passRate = (double)recentTests.Count(t => t.Status == TestStatus.Passed) / recentTests.Count;
            var averageScore = recentTests.Average(t => t.Score);
            var consistencyScore = CalculateConsistencyScore(recentTests);

            return (passRate * 0.4 + averageScore * 0.4 + consistencyScore * 0.2) * 100;
        }

        private double CalculateConsistencyScore(List<TestResult> testResults)
        {
            if (testResults.Count < 2) return 1.0;

            var scores = testResults.Select(t => t.Score).ToList();
            var mean = scores.Average();
            var variance = scores.Sum(s => Math.Pow(s - mean, 2)) / scores.Count;
            var standardDeviation = Math.Sqrt(variance);

            return Math.Max(0, 1 - (standardDeviation / mean));
        }

        private double CalculateQualityImprovement(List<QualityTrendEntry> monthlyTrends)
        {
            if (monthlyTrends.Count < 2) return 0;

            var firstMonth = monthlyTrends.First();
            var lastMonth = monthlyTrends.Last();
            var firstScore = firstMonth.AverageScore;
            var lastScore = lastMonth.AverageScore;

            return ((lastScore - firstScore) / firstScore) * 100;
        }

        private List<string> GenerateQualityRecommendations(
            QualityMetrics metrics,
            List<TestHistoryEntry> testHistory)
        {
            var recommendations = new List<string>();

            if (metrics.QualityScore < 70)
            {
                recommendations.Add("Critical: Immediate quality improvement needed");
            }
            else if (metrics.QualityScore < 85)
            {
                recommendations.Add("Warning: Quality standards need attention");
            }

            var recentFailures = testHistory
                .Where(t => t.Status == TestStatus.Failed)
                .Take(5)
                .ToList();

            if (recentFailures.Any())
            {
                recommendations.Add($"Recent failures detected in: {string.Join(", ", recentFailures.Select(t => t.TestName))}");
            }

            if (metrics.AverageScore < 0.8)
            {
                recommendations.Add("Consider implementing additional quality control measures");
            }

            return recommendations;
        }

        public record QualityControlReport(
            int ComponentId,
            string ComponentName,
            QualityMetrics Metrics,
            List<TestHistoryEntry> TestHistory,
            List<string> Recommendations
        );

        public record QualityMetrics(
            int TotalTests,
            int PassedTests,
            int FailedTests,
            double AverageScore,
            DateTime? LastTestDate,
            double QualityScore
        );

        public record TestHistoryEntry(
            int TestId,
            string TestName,
            TestStatus Status,
            double Score,
            DateTime TestDate,
            string? Notes
        );

        public record TestResults(
            int ComponentId,
            int TotalTests,
            int PassedTests,
            int FailedTests,
            double AverageScore,
            List<TestCategorySummary> TestCategories,
            List<TestResultDetail> TestDetails
        );

        public record TestCategorySummary(
            string Category,
            int TotalTests,
            int PassedTests,
            double AverageScore
        );

        public record TestResultDetail(
            int TestId,
            string TestName,
            string TestCategory,
            TestStatus Status,
            double Score,
            DateTime TestDate,
            string? Notes
        );

        public record QualityTrends(
            int ComponentId,
            List<QualityTrendEntry> MonthlyTrends,
            List<CategoryTrendEntry> CategoryTrends,
            double QualityImprovement
        );

        public record QualityTrendEntry(
            DateTime Month,
            int TotalTests,
            int PassedTests,
            double AverageScore
        );

        public record CategoryTrendEntry(
            string Category,
            int TotalTests,
            int PassedTests,
            double AverageScore
        );

        public enum TestStatus
        {
            Passed,
            Failed,
            InProgress,
            Cancelled
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<SupplyChainRiskAnalysis> GetSupplyChainRiskAnalysis(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components
                .Include(c => c.Supplier)
                .Include(c => c.StockHistory)
                .Include(c => c.PriceHistory)
                .FirstOrDefaultAsync(c => c.Id == componentId);

            if (component == null)
                throw new GraphQLException($"Component with ID {componentId} not found.");

            var stockHistory = component.StockHistory
                .OrderByDescending(s => s.ChangedAt)
                .Take(30)
                .ToList();

            var priceHistory = component.PriceHistory
                .OrderByDescending(p => p.ChangedAt)
                .Take(30)
                .ToList();

            var riskMetrics = new SupplyChainRiskMetrics
            {
                StockRisk = CalculateStockRisk(component, stockHistory),
                PriceRisk = CalculatePriceRisk(component, priceHistory),
                SupplierRisk = CalculateSupplierRisk(component.Supplier),
                LeadTimeRisk = CalculateLeadTimeRisk(component),
                OverallRisk = CalculateOverallRisk(component, stockHistory, priceHistory)
            };

            var riskFactors = new List<RiskFactor>
            {
                new("Stock Level", riskMetrics.StockRisk, "Current stock level and stock volatility"),
                new("Price Volatility", riskMetrics.PriceRisk, "Price stability and trends"),
                new("Supplier Reliability", riskMetrics.SupplierRisk, "Supplier performance and reliability"),
                new("Lead Time", riskMetrics.LeadTimeRisk, "Order fulfillment time and consistency")
            };

            return new SupplyChainRiskAnalysis(
                componentId,
                component.Name,
                riskMetrics,
                riskFactors,
                GenerateRiskMitigationStrategies(riskMetrics, riskFactors)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<SupplierRiskProfile> GetSupplierRiskProfile(
            int supplierId,
            [Service] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var supplier = await context.Suppliers
                .Include(s => s.Components)
                .FirstOrDefaultAsync(s => s.Id == supplierId, cancellationToken);

            if (supplier == null)
                throw new GraphQLException($"Supplier with ID {supplierId} not found.");

            var components = supplier.Components;
            var riskMetrics = new SupplierRiskMetrics
            {
                ComponentCount = components.Count,
                AverageLeadTime = components.Average(c => c.LeadTime),
                OnTimeDeliveryRate = CalculateOnTimeDeliveryRate(components),
                QualityIssueRate = CalculateQualityIssueRate(components),
                PriceStabilityScore = CalculatePriceStabilityScore(components),
                OverallRiskScore = CalculateSupplierOverallRisk(components)
            };

            var riskHistory = await context.SupplierRiskHistory
                .Where(r => r.SupplierId == supplierId)
                .OrderByDescending(r => r.Date)
                .Take(12)
                .ToListAsync();

            return new SupplierRiskProfile(
                supplierId,
                supplier.Name,
                riskMetrics,
                riskHistory.Select(r => new RiskHistoryEntry(
                    r.Date,
                    r.RiskScore,
                    r.RiskFactors,
                    r.MitigationActions
                )).ToList(),
                GenerateSupplierRecommendations(riskMetrics, riskHistory)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<SupplyChainResilience> GetSupplyChainResilience(
            [Service] ApplicationDbContext context)
        {
            var components = await context.Components
                .Include(c => c.Supplier)
                .Include(c => c.StockHistory)
                .Include(c => c.PriceHistory)
                .ToListAsync();

            var resilienceMetrics = new ResilienceMetrics
            {
                TotalComponents = components.Count,
                HighRiskComponents = components.Count(c => CalculateOverallRisk(c, c.StockHistory.ToList(), c.PriceHistory.ToList()) > 0.7),
                MediumRiskComponents = components.Count(c => CalculateOverallRisk(c, c.StockHistory.ToList(), c.PriceHistory.ToList()) > 0.4),
                LowRiskComponents = components.Count(c => CalculateOverallRisk(c, c.StockHistory.ToList(), c.PriceHistory.ToList()) <= 0.4),
                AverageLeadTime = components.Average(c => c.LeadTime),
                SupplyChainHealthScore = CalculateSupplyChainHealthScore(components)
            };

            var criticalPaths = IdentifyCriticalPaths(components);
            var alternativeSuppliers = FindAlternativeSuppliers(components);

            return new SupplyChainResilience(
                resilienceMetrics,
                criticalPaths,
                alternativeSuppliers,
                GenerateResilienceRecommendations(resilienceMetrics, criticalPaths, alternativeSuppliers)
            );
        }

        private double CalculateStockRisk(Component component, List<StockHistory> stockHistory)
        {
            if (!stockHistory.Any()) return 0;

            var currentStock = component.StockQuantity;
            var minStock = component.MinimumStockLevel;
            var stockVolatility = CalculateStockVolatility(stockHistory);
            var stockTrend = AnalyzeStockTrend(stockHistory);

            var stockLevelRisk = currentStock <= minStock ? 1.0 : 
                (double)(minStock * 2 - currentStock) / minStock;
            var volatilityRisk = stockVolatility * 0.3;
            var trendRisk = stockTrend == StockTrend.RapidDecrease ? 0.3 : 0;

            return Math.Min(1.0, Math.Max(0, stockLevelRisk + volatilityRisk + trendRisk));
        }

        private double CalculatePriceRisk(Component component, List<PriceHistory> priceHistory)
        {
            if (!priceHistory.Any()) return 0;

            var priceVolatility = CalculatePriceVolatility(priceHistory);
            var priceTrend = AnalyzePriceTrend(priceHistory);
            var priceChange = Math.Abs(priceHistory.First().NewPrice - priceHistory.Last().NewPrice) / priceHistory.Last().NewPrice;

            var volatilityRisk = priceVolatility * 0.4;
            var trendRisk = priceTrend == PriceTrend.Increasing ? 0.3 : 0;
            var changeRisk = priceChange * 0.3;

            return Math.Min(1.0, Math.Max(0, volatilityRisk + trendRisk + changeRisk));
        }

        private double CalculateSupplierRisk(Supplier supplier)
        {
            if (supplier == null) return 1.0;

            var reliabilityScore = supplier.ReliabilityScore;
            var performanceScore = supplier.PerformanceScore;
            var qualityScore = supplier.QualityScore;

            return 1 - ((reliabilityScore + performanceScore + qualityScore) / 3);
        }

        private double CalculateLeadTimeRisk(Component component)
        {
            var leadTime = component.LeadTime;
            var maxAcceptableLeadTime = 30; // 30 days

            return Math.Min(1.0, (double)leadTime / maxAcceptableLeadTime);
        }

        private double CalculateOverallRisk(
            Component component,
            List<StockHistory> stockHistory,
            List<PriceHistory> priceHistory)
        {
            var stockRisk = CalculateStockRisk(component, stockHistory);
            var priceRisk = CalculatePriceRisk(component, priceHistory);
            var supplierRisk = CalculateSupplierRisk(component.Supplier);
            var leadTimeRisk = CalculateLeadTimeRisk(component);

            return (stockRisk * 0.3 + priceRisk * 0.3 + supplierRisk * 0.2 + leadTimeRisk * 0.2);
        }

        private double CalculateOnTimeDeliveryRate(List<Component> components)
        {
            var totalDeliveries = components.Sum(c => c.DeliveryCount);
            var onTimeDeliveries = components.Sum(c => c.OnTimeDeliveryCount);

            return totalDeliveries > 0 ? (double)onTimeDeliveries / totalDeliveries : 0;
        }

        private double CalculateQualityIssueRate(List<Component> components)
        {
            var totalComponents = components.Count;
            var componentsWithIssues = components.Count(c => c.QualityIssues > 0);

            return totalComponents > 0 ? (double)componentsWithIssues / totalComponents : 0;
        }

        private double CalculatePriceStabilityScore(List<Component> components)
        {
            if (!components.Any()) return 0;

            var stabilityScores = components.Select(c =>
            {
                var priceHistory = c.PriceHistory.OrderByDescending(p => p.ChangedAt).Take(10).ToList();
                return 1 - CalculatePriceVolatility(priceHistory);
            });

            return stabilityScores.Average();
        }

        private double CalculateSupplierOverallRisk(List<Component> components)
        {
            var onTimeDeliveryRate = CalculateOnTimeDeliveryRate(components);
            var qualityIssueRate = CalculateQualityIssueRate(components);
            var priceStabilityScore = CalculatePriceStabilityScore(components);

            return 1 - ((onTimeDeliveryRate * 0.4 + (1 - qualityIssueRate) * 0.4 + priceStabilityScore * 0.2));
        }

        private double CalculateSupplyChainHealthScore(List<Component> components)
        {
            var riskScores = components.Select(c => 
                1 - CalculateOverallRisk(c, c.StockHistory.ToList(), c.PriceHistory.ToList()));

            return riskScores.Any() ? riskScores.Average() : 0;
        }

        private List<CriticalPath> IdentifyCriticalPaths(List<Component> components)
        {
            return components
                .Where(c => CalculateOverallRisk(c, c.StockHistory.ToList(), c.PriceHistory.ToList()) > 0.7)
                .Select(c => new CriticalPath(
                    c.Id,
                    c.Name,
                    c.Supplier?.Name ?? "Unknown",
                    c.LeadTime,
                    CalculateOverallRisk(c, c.StockHistory.ToList(), c.PriceHistory.ToList())
                ))
                .ToList();
        }

        private List<AlternativeSupplier> FindAlternativeSuppliers(List<Component> components)
        {
            return components
                .Where(c => c.Supplier != null)
                .Select(c => new AlternativeSupplier(
                    c.Id,
                    c.Name,
                    c.Supplier.Name,
                    c.Supplier.ReliabilityScore,
                    c.Supplier.PerformanceScore,
                    c.Supplier.QualityScore
                ))
                .ToList();
        }

        private List<string> GenerateRiskMitigationStrategies(
            SupplyChainRiskMetrics metrics,
            List<RiskFactor> riskFactors)
        {
            var strategies = new List<string>();

            if (metrics.StockRisk > 0.7)
            {
                strategies.Add("Implement safety stock levels");
                strategies.Add("Establish backup suppliers");
            }

            if (metrics.PriceRisk > 0.7)
            {
                strategies.Add("Negotiate long-term pricing agreements");
                strategies.Add("Implement price hedging strategies");
            }

            if (metrics.SupplierRisk > 0.7)
            {
                strategies.Add("Develop supplier contingency plans");
                strategies.Add("Regular supplier performance reviews");
            }

            if (metrics.LeadTimeRisk > 0.7)
            {
                strategies.Add("Optimize order quantities");
                strategies.Add("Implement just-in-time inventory management");
            }

            return strategies;
        }

        private List<string> GenerateSupplierRecommendations(
            SupplierRiskMetrics metrics,
            List<SupplierRiskHistory> riskHistory)
        {
            var recommendations = new List<string>();

            if (metrics.OverallRiskScore > 0.7)
            {
                recommendations.Add("Critical: Immediate supplier evaluation needed");
            }
            else if (metrics.OverallRiskScore > 0.4)
            {
                recommendations.Add("Warning: Supplier performance needs improvement");
            }

            if (metrics.OnTimeDeliveryRate < 0.8)
            {
                recommendations.Add("Improve delivery reliability");
            }

            if (metrics.QualityIssueRate > 0.1)
            {
                recommendations.Add("Address quality control issues");
            }

            if (metrics.PriceStabilityScore < 0.7)
            {
                recommendations.Add("Stabilize pricing strategy");
            }

            return recommendations;
        }

        private List<string> GenerateResilienceRecommendations(
            ResilienceMetrics metrics,
            List<CriticalPath> criticalPaths,
            List<AlternativeSupplier> alternativeSuppliers)
        {
            var recommendations = new List<string>();

            if (metrics.HighRiskComponents > 0)
            {
                recommendations.Add($"Address {metrics.HighRiskComponents} high-risk components");
            }

            if (metrics.AverageLeadTime > 20)
            {
                recommendations.Add("Optimize supply chain lead times");
            }

            if (metrics.SupplyChainHealthScore < 0.7)
            {
                recommendations.Add("Implement supply chain resilience measures");
            }

            if (criticalPaths.Any())
            {
                recommendations.Add("Develop contingency plans for critical paths");
            }

            return recommendations;
        }

        public record SupplyChainRiskAnalysis(
            int ComponentId,
            string ComponentName,
            SupplyChainRiskMetrics RiskMetrics,
            List<RiskFactor> RiskFactors,
            List<string> MitigationStrategies
        );

        public record SupplyChainRiskMetrics(
            double StockRisk,
            double PriceRisk,
            double SupplierRisk,
            double LeadTimeRisk,
            double OverallRisk
        );

        public record RiskFactor(
            string Name,
            double RiskLevel,
            string Description
        );

        public record SupplierRiskProfile(
            int SupplierId,
            string SupplierName,
            SupplierRiskMetrics RiskMetrics,
            List<RiskHistoryEntry> RiskHistory,
            List<string> Recommendations
        );

        public record SupplierRiskMetrics(
            int ComponentCount,
            double AverageLeadTime,
            double OnTimeDeliveryRate,
            double QualityIssueRate,
            double PriceStabilityScore,
            double OverallRiskScore
        );

        public record RiskHistoryEntry(
            DateTime Date,
            double RiskScore,
            List<string> RiskFactors,
            List<string> MitigationActions
        );

        public record SupplyChainResilience(
            ResilienceMetrics Metrics,
            List<CriticalPath> CriticalPaths,
            List<AlternativeSupplier> AlternativeSuppliers,
            List<string> Recommendations
        );

        public record ResilienceMetrics(
            int TotalComponents,
            int HighRiskComponents,
            int MediumRiskComponents,
            int LowRiskComponents,
            double AverageLeadTime,
            double SupplyChainHealthScore
        );

        public record CriticalPath(
            int ComponentId,
            string ComponentName,
            string SupplierName,
            int LeadTime,
            double RiskScore
        );

        public record AlternativeSupplier(
            int ComponentId,
            string ComponentName,
            string SupplierName,
            double ReliabilityScore,
            double PerformanceScore,
            double QualityScore
        );

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<EnvironmentalImpact> GetEnvironmentalImpact(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components
                .Include(c => c.EnvironmentalData)
                .Include(c => c.ComplianceData)
                .FirstOrDefaultAsync(c => c.Id == componentId);

            if (component == null)
                throw new GraphQLException($"Component with ID {componentId} not found.");

            var environmentalData = component.EnvironmentalData;
            var complianceData = component.ComplianceData;

            var impactMetrics = new EnvironmentalMetrics
            {
                CarbonFootprint = CalculateCarbonFootprint(environmentalData),
                EnergyConsumption = CalculateEnergyConsumption(environmentalData),
                WaterUsage = CalculateWaterUsage(environmentalData),
                WasteGeneration = CalculateWasteGeneration(environmentalData),
                RecyclabilityScore = CalculateRecyclabilityScore(environmentalData),
                OverallImpact = CalculateOverallEnvironmentalImpact(environmentalData)
            };

            var complianceStatus = new ComplianceStatus
            {
                RoHSCompliant = complianceData.RoHSCompliant,
                REACHCompliant = complianceData.REACHCompliant,
                WEEERegistered = complianceData.WEEERegistered,
                ConflictMineralsCompliant = complianceData.ConflictMineralsCompliant,
                ComplianceScore = CalculateComplianceScore(complianceData)
            };

            return new EnvironmentalImpact(
                componentId,
                component.Name,
                impactMetrics,
                complianceStatus,
                GenerateEnvironmentalRecommendations(impactMetrics, complianceStatus)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<EnvironmentalTrends> GetEnvironmentalTrends(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            var environmentalHistory = await context.EnvironmentalHistory
                .Where(e => e.ComponentId == componentId)
                .OrderBy(e => e.Date)
                .ToListAsync();

            var monthlyTrends = environmentalHistory
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new EnvironmentalTrendEntry(
                    new DateTime(g.Key.Year, g.Key.Month, 1),
                    g.Average(e => e.CarbonFootprint),
                    g.Average(e => e.EnergyConsumption),
                    g.Average(e => e.WaterUsage),
                    g.Average(e => e.WasteGeneration)
                ))
                .OrderBy(t => t.Month)
                .ToList();

            var improvementMetrics = new EnvironmentalImprovementMetrics
            {
                CarbonFootprintReduction = CalculateReductionRate(monthlyTrends, t => t.CarbonFootprint),
                EnergyConsumptionReduction = CalculateReductionRate(monthlyTrends, t => t.EnergyConsumption),
                WaterUsageReduction = CalculateReductionRate(monthlyTrends, t => t.WaterUsage),
                WasteGenerationReduction = CalculateReductionRate(monthlyTrends, t => t.WasteGeneration)
            };

            return new EnvironmentalTrends(
                componentId,
                monthlyTrends,
                improvementMetrics,
                GenerateTrendRecommendations(monthlyTrends, improvementMetrics)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<SustainabilityReport> GetSustainabilityReport(
            [Service] ApplicationDbContext context)
        {
            var components = await context.Components
                .Include(c => c.EnvironmentalData)
                .Include(c => c.ComplianceData)
                .ToListAsync();

            var sustainabilityMetrics = new SustainabilityMetrics
            {
                TotalComponents = components.Count,
                CompliantComponents = components.Count(c => IsFullyCompliant(c.ComplianceData)),
                HighImpactComponents = components.Count(c => CalculateOverallEnvironmentalImpact(c.EnvironmentalData) > 0.7),
                AverageCarbonFootprint = components.Average(c => CalculateCarbonFootprint(c.EnvironmentalData)),
                AverageEnergyConsumption = components.Average(c => CalculateEnergyConsumption(c.EnvironmentalData)),
                AverageWaterUsage = components.Average(c => CalculateWaterUsage(c.EnvironmentalData)),
                AverageWasteGeneration = components.Average(c => CalculateWasteGeneration(c.EnvironmentalData)),
                OverallSustainabilityScore = CalculateOverallSustainabilityScore(components)
            };

            var categoryBreakdown = components
                .GroupBy(c => c.Category)
                .Select(g => new CategoryEnvironmentalSummary(
                    g.Key,
                    g.Count(),
                    g.Average(c => CalculateOverallEnvironmentalImpact(c.EnvironmentalData)),
                    g.Count(c => IsFullyCompliant(c.ComplianceData))
                ))
                .ToList();

            return new SustainabilityReport(
                sustainabilityMetrics,
                categoryBreakdown,
                GenerateSustainabilityRecommendations(sustainabilityMetrics, categoryBreakdown)
            );
        }

        private double CalculateCarbonFootprint(EnvironmentalData data)
        {
            if (data == null) return 0;

            var manufacturingEmissions = data.ManufacturingEmissions;
            var transportationEmissions = data.TransportationEmissions;
            var usageEmissions = data.UsageEmissions;
            var disposalEmissions = data.DisposalEmissions;

            return manufacturingEmissions + transportationEmissions + usageEmissions + disposalEmissions;
        }

        private double CalculateEnergyConsumption(EnvironmentalData data)
        {
            if (data == null) return 0;

            var manufacturingEnergy = data.ManufacturingEnergy;
            var transportationEnergy = data.TransportationEnergy;
            var usageEnergy = data.UsageEnergy;
            var disposalEnergy = data.DisposalEnergy;

            return manufacturingEnergy + transportationEnergy + usageEnergy + disposalEnergy;
        }

        private double CalculateWaterUsage(EnvironmentalData data)
        {
            if (data == null) return 0;

            var manufacturingWater = data.ManufacturingWater;
            var usageWater = data.UsageWater;
            var disposalWater = data.DisposalWater;

            return manufacturingWater + usageWater + disposalWater;
        }

        private double CalculateWasteGeneration(EnvironmentalData data)
        {
            if (data == null) return 0;

            var manufacturingWaste = data.ManufacturingWaste;
            var packagingWaste = data.PackagingWaste;
            var disposalWaste = data.DisposalWaste;

            return manufacturingWaste + packagingWaste + disposalWaste;
        }

        private double CalculateRecyclabilityScore(EnvironmentalData data)
        {
            if (data == null) return 0;

            var recyclableMaterials = data.RecyclableMaterials;
            var totalMaterials = data.TotalMaterials;
            var recyclingEfficiency = data.RecyclingEfficiency;

            return (recyclableMaterials / totalMaterials) * recyclingEfficiency;
        }

        private double CalculateOverallEnvironmentalImpact(EnvironmentalData data)
        {
            if (data == null) return 0;

            var carbonFootprint = CalculateCarbonFootprint(data);
            var energyConsumption = CalculateEnergyConsumption(data);
            var waterUsage = CalculateWaterUsage(data);
            var wasteGeneration = CalculateWasteGeneration(data);
            var recyclabilityScore = CalculateRecyclabilityScore(data);

            return (carbonFootprint * 0.3 + 
                    energyConsumption * 0.3 + 
                    waterUsage * 0.2 + 
                    wasteGeneration * 0.1 + 
                    (1 - recyclabilityScore) * 0.1);
        }

        private double CalculateComplianceScore(ComplianceData data)
        {
            if (data == null) return 0;

            var complianceFactors = new[]
            {
                data.RoHSCompliant ? 1.0 : 0.0,
                data.REACHCompliant ? 1.0 : 0.0,
                data.WEEERegistered ? 1.0 : 0.0,
                data.ConflictMineralsCompliant ? 1.0 : 0.0
            };

            return complianceFactors.Average();
        }

        private double CalculateReductionRate(
            List<EnvironmentalTrendEntry> trends,
            Func<EnvironmentalTrendEntry, double> metricSelector)
        {
            if (trends.Count < 2) return 0;

            var firstValue = metricSelector(trends.First());
            var lastValue = metricSelector(trends.Last());

            return firstValue > 0 ? ((firstValue - lastValue) / firstValue) * 100 : 0;
        }

        private bool IsFullyCompliant(ComplianceData data)
        {
            if (data == null) return false;

            return data.RoHSCompliant &&
                   data.REACHCompliant &&
                   data.WEEERegistered &&
                   data.ConflictMineralsCompliant;
        }

        private double CalculateOverallSustainabilityScore(List<Component> components)
        {
            if (!components.Any()) return 0;

            var scores = components.Select(c => 
            {
                var environmentalScore = 1 - CalculateOverallEnvironmentalImpact(c.EnvironmentalData);
                var complianceScore = CalculateComplianceScore(c.ComplianceData);
                return (environmentalScore * 0.7 + complianceScore * 0.3);
            });

            return scores.Average();
        }

        private List<string> GenerateEnvironmentalRecommendations(
            EnvironmentalMetrics metrics,
            ComplianceStatus compliance)
        {
            var recommendations = new List<string>();

            if (metrics.CarbonFootprint > 1000)
            {
                recommendations.Add("Implement carbon reduction initiatives");
            }

            if (metrics.EnergyConsumption > 500)
            {
                recommendations.Add("Optimize energy efficiency");
            }

            if (metrics.WaterUsage > 100)
            {
                recommendations.Add("Reduce water consumption");
            }

            if (metrics.WasteGeneration > 50)
            {
                recommendations.Add("Implement waste reduction programs");
            }

            if (metrics.RecyclabilityScore < 0.7)
            {
                recommendations.Add("Improve recyclability of materials");
            }

            if (!compliance.RoHSCompliant)
            {
                recommendations.Add("Ensure RoHS compliance");
            }

            if (!compliance.REACHCompliant)
            {
                recommendations.Add("Ensure REACH compliance");
            }

            return recommendations;
        }

        private List<string> GenerateTrendRecommendations(
            List<EnvironmentalTrendEntry> trends,
            EnvironmentalImprovementMetrics improvements)
        {
            var recommendations = new List<string>();

            if (improvements.CarbonFootprintReduction < 5)
            {
                recommendations.Add("Accelerate carbon footprint reduction efforts");
            }

            if (improvements.EnergyConsumptionReduction < 5)
            {
                recommendations.Add("Increase energy efficiency initiatives");
            }

            if (improvements.WaterUsageReduction < 5)
            {
                recommendations.Add("Enhance water conservation measures");
            }

            if (improvements.WasteGenerationReduction < 5)
            {
                recommendations.Add("Strengthen waste reduction programs");
            }

            return recommendations;
        }

        private List<string> GenerateSustainabilityRecommendations(
            SustainabilityMetrics metrics,
            List<CategoryEnvironmentalSummary> categoryBreakdown)
        {
            var recommendations = new List<string>();

            if (metrics.HighImpactComponents > 0)
            {
                recommendations.Add($"Address {metrics.HighImpactComponents} high-impact components");
            }

            if (metrics.OverallSustainabilityScore < 0.7)
            {
                recommendations.Add("Implement comprehensive sustainability improvements");
            }

            var highImpactCategories = categoryBreakdown
                .Where(c => c.AverageImpact > 0.7)
                .Select(c => c.Category);

            if (highImpactCategories.Any())
            {
                recommendations.Add($"Focus on improving sustainability in categories: {string.Join(", ", highImpactCategories)}");
            }

            return recommendations;
        }

        public record EnvironmentalImpact(
            int ComponentId,
            string ComponentName,
            EnvironmentalMetrics ImpactMetrics,
            ComplianceStatus ComplianceStatus,
            List<string> Recommendations
        );

        public record EnvironmentalMetrics(
            double CarbonFootprint,
            double EnergyConsumption,
            double WaterUsage,
            double WasteGeneration,
            double RecyclabilityScore,
            double OverallImpact
        );

        public record ComplianceStatus(
            bool RoHSCompliant,
            bool REACHCompliant,
            bool WEEERegistered,
            bool ConflictMineralsCompliant,
            double ComplianceScore
        );

        public record EnvironmentalTrends(
            int ComponentId,
            List<EnvironmentalTrendEntry> MonthlyTrends,
            EnvironmentalImprovementMetrics ImprovementMetrics,
            List<string> Recommendations
        );

        public record EnvironmentalTrendEntry(
            DateTime Month,
            double CarbonFootprint,
            double EnergyConsumption,
            double WaterUsage,
            double WasteGeneration
        );

        public record EnvironmentalImprovementMetrics(
            double CarbonFootprintReduction,
            double EnergyConsumptionReduction,
            double WaterUsageReduction,
            double WasteGenerationReduction
        );

        public record SustainabilityReport(
            SustainabilityMetrics Metrics,
            List<CategoryEnvironmentalSummary> CategoryBreakdown,
            List<string> Recommendations
        );

        public record SustainabilityMetrics(
            int TotalComponents,
            int CompliantComponents,
            int HighImpactComponents,
            double AverageCarbonFootprint,
            double AverageEnergyConsumption,
            double AverageWaterUsage,
            double AverageWasteGeneration,
            double OverallSustainabilityScore
        );

        public record CategoryEnvironmentalSummary(
            string Category,
            int ComponentCount,
            double AverageImpact,
            int CompliantCount
        );

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<CostOptimizationAnalysis> GetCostOptimizationAnalysis(
            int componentId,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components
                .Include(c => c.SupplierPrices)
                .Include(c => c.PriceHistory)
                .FirstOrDefaultAsync(c => c.Id == componentId);

            if (component == null)
                throw new GraphQLException($"Component with ID {componentId} not found.");

            var supplierPrices = component.SupplierPrices
                .OrderBy(p => p.Price)
                .ToList();

            var priceHistory = component.PriceHistory
                .OrderByDescending(p => p.ChangedAt)
                .Take(30)
                .ToList();

            var optimizationMetrics = new CostOptimizationMetrics
            {
                CurrentPrice = component.Price,
                LowestSupplierPrice = supplierPrices.FirstOrDefault()?.Price ?? component.Price,
                HighestSupplierPrice = supplierPrices.LastOrDefault()?.Price ?? component.Price,
                AverageSupplierPrice = supplierPrices.Any() ? supplierPrices.Average(p => p.Price) : component.Price,
                PriceVolatility = CalculatePriceVolatility(priceHistory),
                PotentialSavings = CalculatePotentialSavings(component.Price, supplierPrices),
                PriceTrend = AnalyzePriceTrend(priceHistory)
            };

            var supplierAnalysis = supplierPrices
                .Select(p => new SupplierPriceAnalysis(
                    p.SupplierId,
                    p.SupplierName,
                    p.Price,
                    p.MinimumOrderQuantity,
                    p.LeadTime,
                    p.QualityScore,
                    p.ReliabilityScore,
                    CalculateSupplierValueScore(p)
                ))
                .ToList();

            return new CostOptimizationAnalysis(
                componentId,
                component.Name,
                optimizationMetrics,
                supplierAnalysis,
                GenerateCostOptimizationRecommendations(optimizationMetrics, supplierAnalysis)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<BulkCostOptimization> GetBulkCostOptimization(
            List<int> componentIds,
            [Service] ApplicationDbContext context)
        {
            var components = await context.Components
                .Include(c => c.SupplierPrices)
                .Where(c => componentIds.Contains(c.Id))
                .ToListAsync();

            var optimizationResults = new List<ComponentOptimization>();
            var totalPotentialSavings = 0m;
            var highImpactCount = 0;
            var mediumImpactCount = 0;
            var lowImpactCount = 0;

            foreach (var component in components)
            {
                var supplierPrices = component.SupplierPrices
                    .OrderBy(p => p.Price)
                    .ToList();

                var currentCost = component.Price * component.StockQuantity;
                var optimizedCost = CalculateOptimizedCost(component, supplierPrices);
                var potentialSavings = currentCost - optimizedCost;
                var impactLevel = CalculateOptimizationImpact(potentialSavings, currentCost);

                switch (impactLevel)
                {
                    case ImpactLevel.High:
                        highImpactCount++;
                        break;
                    case ImpactLevel.Medium:
                        mediumImpactCount++;
                        break;
                    case ImpactLevel.Low:
                        lowImpactCount++;
                        break;
                }

                totalPotentialSavings += potentialSavings;

                optimizationResults.Add(new ComponentOptimization(
                    component.Id,
                    component.Name,
                    currentCost,
                    optimizedCost,
                    potentialSavings,
                    impactLevel,
                    GenerateComponentOptimizationRecommendations(component, supplierPrices)
                ));
            }

            return new BulkCostOptimization(
                optimizationResults.OrderByDescending(r => r.PotentialSavings).ToList(),
                totalPotentialSavings,
                highImpactCount,
                mediumImpactCount,
                lowImpactCount,
                GenerateBulkOptimizationRecommendations(optimizationResults)
            );
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<SupplierCostAnalysis> GetSupplierCostAnalysis(
            int supplierId,
            [Service] ApplicationDbContext context)
        {
            var supplier = await context.Suppliers
                .Include(s => s.Components)
                    .ThenInclude(c => c.SupplierPrices)
                .FirstOrDefaultAsync(s => s.Id == supplierId);

            if (supplier == null)
                throw new GraphQLException($"Supplier with ID {supplierId} not found.");

            var componentAnalyses = supplier.Components
                .Select(c => new ComponentCostAnalysis(
                    c.Id,
                    c.Name,
                    c.Price,
                    c.SupplierPrices.FirstOrDefault(p => p.SupplierId == supplierId)?.Price ?? c.Price,
                    c.StockQuantity,
                    CalculatePriceDifference(c.Price, c.SupplierPrices.FirstOrDefault(p => p.SupplierId == supplierId)?.Price ?? c.Price)
                ))
                .OrderByDescending(a => a.PriceDifference)
                .ToList();

            var costMetrics = new SupplierCostMetrics(
                componentAnalyses.Count,
                componentAnalyses.Sum(a => a.CurrentCost),
                componentAnalyses.Sum(a => a.OptimizedCost),
                componentAnalyses.Sum(a => a.PriceDifference),
                CalculateAveragePriceDifference(componentAnalyses),
                CalculateSupplierCompetitiveness(componentAnalyses)
            );

            return new SupplierCostAnalysis(
                supplierId,
                supplier.Name,
                costMetrics,
                componentAnalyses,
                GenerateSupplierCostRecommendations(costMetrics, componentAnalyses)
            );
        }

        private decimal CalculatePotentialSavings(decimal currentPrice, List<SupplierPrice> supplierPrices)
        {
            if (!supplierPrices.Any()) return 0;

            var lowestPrice = supplierPrices.First().Price;
            return currentPrice > lowestPrice ? currentPrice - lowestPrice : 0;
        }

        private double CalculateSupplierValueScore(SupplierPrice supplierPrice)
        {
            var priceScore = 1 - (supplierPrice.Price / supplierPrice.HighestPrice);
            var qualityScore = supplierPrice.QualityScore;
            var reliabilityScore = supplierPrice.ReliabilityScore;
            var leadTimeScore = 1 - (supplierPrice.LeadTime / 30.0); // Assuming 30 days is max lead time

            return (priceScore * 0.4 + qualityScore * 0.3 + reliabilityScore * 0.2 + leadTimeScore * 0.1);
        }

        private decimal CalculateOptimizedCost(Component component, List<SupplierPrice> supplierPrices)
        {
            if (!supplierPrices.Any()) return component.Price * component.StockQuantity;

            var bestSupplier = supplierPrices
                .OrderBy(p => CalculateSupplierValueScore(p))
                .First();

            return bestSupplier.Price * component.StockQuantity;
        }

        private ImpactLevel CalculateOptimizationImpact(decimal potentialSavings, decimal currentCost)
        {
            var savingsPercentage = potentialSavings / currentCost;
            return savingsPercentage switch
            {
                >= 0.2m => ImpactLevel.High,
                >= 0.1m => ImpactLevel.Medium,
                _ => ImpactLevel.Low
            };
        }

        private decimal CalculatePriceDifference(decimal currentPrice, decimal supplierPrice)
        {
            return currentPrice - supplierPrice;
        }

        private double CalculateAveragePriceDifference(List<ComponentCostAnalysis> analyses)
        {
            if (!analyses.Any()) return 0;

            return analyses.Average(a => (double)(a.PriceDifference / a.CurrentPrice));
        }

        private double CalculateSupplierCompetitiveness(List<ComponentCostAnalysis> analyses)
        {
            if (!analyses.Any()) return 0;

            var competitiveCount = analyses.Count(a => a.PriceDifference > 0);
            return (double)competitiveCount / analyses.Count;
        }

        private List<string> GenerateCostOptimizationRecommendations(
            CostOptimizationMetrics metrics,
            List<SupplierPriceAnalysis> supplierAnalysis)
        {
            var recommendations = new List<string>();

            if (metrics.PotentialSavings > 0)
            {
                var bestSupplier = supplierAnalysis.OrderByDescending(s => s.ValueScore).First();
                recommendations.Add($"Consider switching to {bestSupplier.SupplierName} for potential savings of {metrics.PotentialSavings:C}");
            }

            if (metrics.PriceVolatility > 0.1)
            {
                recommendations.Add("Implement price hedging strategies due to high volatility");
            }

            if (metrics.PriceTrend == PriceTrend.Increasing)
            {
                recommendations.Add("Consider bulk purchasing to lock in current prices");
            }

            return recommendations;
        }

        private List<string> GenerateComponentOptimizationRecommendations(
            Component component,
            List<SupplierPrice> supplierPrices)
        {
            var recommendations = new List<string>();

            if (supplierPrices.Any())
            {
                var bestSupplier = supplierPrices
                    .OrderBy(p => CalculateSupplierValueScore(p))
                    .First();

                var potentialSavings = CalculatePotentialSavings(component.Price, supplierPrices);
                if (potentialSavings > 0)
                {
                    recommendations.Add($"Switch to {bestSupplier.SupplierName} for savings of {potentialSavings:C}");
                }
            }

            if (component.StockQuantity > component.MaximumStockLevel)
            {
                recommendations.Add("Reduce stock levels to optimize holding costs");
            }

            return recommendations;
        }

        private List<string> GenerateBulkOptimizationRecommendations(
            List<ComponentOptimization> optimizationResults)
        {
            var recommendations = new List<string>();

            var highImpactComponents = optimizationResults
                .Where(r => r.ImpactLevel == ImpactLevel.High)
                .ToList();

            if (highImpactComponents.Any())
            {
                recommendations.Add($"Prioritize optimization for {highImpactComponents.Count} high-impact components");
            }

            var totalSavings = optimizationResults.Sum(r => r.PotentialSavings);
            if (totalSavings > 0)
            {
                recommendations.Add($"Total potential savings: {totalSavings:C}");
            }

            return recommendations;
        }

        private List<string> GenerateSupplierCostRecommendations(
            SupplierCostMetrics metrics,
            List<ComponentCostAnalysis> analyses)
        {
            var recommendations = new List<string>();

            if (metrics.PriceDifference > 0)
            {
                recommendations.Add($"Potential savings of {metrics.PriceDifference:C} available");
            }

            if (metrics.AveragePriceDifference > 0.1)
            {
                recommendations.Add("Supplier prices are significantly higher than market average");
            }

            if (metrics.CompetitivenessScore < 0.5)
            {
                recommendations.Add("Consider alternative suppliers for better pricing");
            }

            return recommendations;
        }

        public record CostOptimizationAnalysis(
            int ComponentId,
            string ComponentName,
            CostOptimizationMetrics Metrics,
            List<SupplierPriceAnalysis> SupplierAnalysis,
            List<string> Recommendations
        );

        public record CostOptimizationMetrics(
            decimal CurrentPrice,
            decimal LowestSupplierPrice,
            decimal HighestSupplierPrice,
            decimal AverageSupplierPrice,
            double PriceVolatility,
            decimal PotentialSavings,
            PriceTrend PriceTrend
        );

        public record SupplierPriceAnalysis(
            int SupplierId,
            string SupplierName,
            decimal Price,
            int MinimumOrderQuantity,
            int LeadTime,
            double QualityScore,
            double ReliabilityScore,
            double ValueScore
        );

        public record BulkCostOptimization(
            List<ComponentOptimization> Results,
            decimal TotalPotentialSavings,
            int HighImpactCount,
            int MediumImpactCount,
            int LowImpactCount,
            List<string> Recommendations
        );

        public record ComponentOptimization(
            int ComponentId,
            string ComponentName,
            decimal CurrentCost,
            decimal OptimizedCost,
            decimal PotentialSavings,
            ImpactLevel ImpactLevel,
            List<string> Recommendations
        );

        public record SupplierCostAnalysis(
            int SupplierId,
            string SupplierName,
            SupplierCostMetrics Metrics,
            List<ComponentCostAnalysis> ComponentAnalyses,
            List<string> Recommendations
        );

        public record SupplierCostMetrics(
            int ComponentCount,
            decimal CurrentCost,
            decimal OptimizedCost,
            decimal PriceDifference,
            double AveragePriceDifference,
            double CompetitivenessScore
        );

        public record ComponentCostAnalysis(
            int ComponentId,
            string ComponentName,
            decimal CurrentPrice,
            decimal SupplierPrice,
            int StockQuantity,
            decimal PriceDifference
        );
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
        int TotalCount,
        decimal TotalValue,
        int LowStockCount,
        int OutOfStockCount,
        double AveragePrice,
        string? MostExpensiveComponent,
        string? LeastExpensiveComponent,
        string? MostStockedComponent,
        string? LeastStockedComponent);
    public record CategoryStatistics(
        int TotalCategories,
        List<CategoryBreakdown> CategoryBreakdown);
    public record CategoryBreakdown(
        string Category,
        int Count,
        decimal TotalValue,
        double AveragePrice,
        int LowStockCount,
        int OutOfStockCount);
    public record PriceStatistics(
        double AveragePrice,
        decimal MinPrice,
        decimal MaxPrice,
        List<PriceRange> PriceRanges);
    public record PriceRange(string Range, int Count);
    public record StockStatistics(
        int TotalStock,
        double AverageStock,
        int LowStockCount,
        int OutOfStockCount,
        int OverstockedCount,
        List<StockRange> StockRanges);
    public record StockRange(string Range, int Count);
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

    public record SupplierAnalytics(
        int TotalSuppliers,
        int TotalComponents,
        double AverageComponentsPerSupplier,
        List<SupplierSummary> TopSuppliers);

    public record SupplierSummary(
        string Name,
        int ComponentCount,
        decimal TotalValue,
        double AveragePrice);

    public record ComponentLifecycle(
        int ComponentId,
        string ComponentName,
        ComponentStatus CurrentStatus,
        List<PriceHistoryEntry> PriceHistory,
        List<StockHistoryEntry> StockHistory,
        DateTime? LastRestockDate,
        DateTime? LastPriceUpdateDate,
        int? DaysSinceLastRestock,
        int? DaysSinceLastPriceUpdate);

    public record PriceHistoryEntry(
        decimal OldPrice,
        decimal NewPrice,
        DateTime ChangedAt,
        string? ChangedBy,
        string? Reason);

    public record StockHistoryEntry(
        int OldQuantity,
        int NewQuantity,
        DateTime ChangedAt,
        string? ChangedBy,
        string? Reason);

    public record SupplierPerformance(
        int SupplierId,
        string SupplierName,
        int TotalComponents,
        decimal TotalValue,
        double AveragePrice,
        int LowStockCount,
        int OutOfStockCount,
        int OverstockedCount,
        double HealthScore,
        ComponentBreakdown ComponentBreakdown);

    public record ComponentBreakdown(
        List<CategorySummary> ByCategory,
        StatusSummary ByStatus);

    public record CategorySummary(
        string Category,
        int Count,
        decimal TotalValue);

    public record StatusSummary(
        int Active,
        int Discontinued,
        int OutOfStock,
        int LowStock,
        int Overstocked);

    public record InventoryPredictions(
        List<StockPrediction> Predictions,
        int HighUrgencyCount,
        int MediumUrgencyCount,
        int LowUrgencyCount);

    public record StockPrediction(
        int ComponentId,
        string ComponentName,
        int CurrentStock,
        double AverageDailyConsumption,
        int DaysUntilLowStock,
        int RecommendedOrderQuantity,
        UrgencyLevel UrgencyLevel,
        double ConfidenceScore);

    public record PricePredictions(
        List<PricePrediction> Predictions,
        int IncreasingPriceCount,
        int DecreasingPriceCount,
        int StablePriceCount);

    public record PricePrediction(
        int ComponentId,
        string ComponentName,
        decimal CurrentPrice,
        decimal PredictedPrice,
        PriceTrend PriceTrend,
        double PriceVolatility,
        double ConfidenceScore,
        string Recommendation);

    public record InventoryRecommendations(
        List<InventoryRecommendation> Recommendations,
        int HighPriorityCount,
        int MediumPriorityCount,
        int LowPriorityCount);

    public record InventoryRecommendation(
        int ComponentId,
        string ComponentName,
        Priority Priority,
        string Action,
        string Reasoning,
        StockPrediction StockPrediction,
        PricePrediction PricePrediction);

    public enum UrgencyLevel
    {
        Low,
        Medium,
        High
    }

    public enum PriceTrend
    {
        Increasing,
        Decreasing,
        Stable
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public record MLPredictions(
        List<MLPrediction> Predictions,
        int HighConfidenceCount,
        int MediumConfidenceCount,
        int LowConfidenceCount);

    public record MLPrediction(
        int ComponentId,
        string ComponentName,
        SeasonalPattern SeasonalPattern,
        DemandForecast DemandForecast,
        PriceForecast PriceForecast,
        decimal OptimalPrice,
        int OptimalStock,
        double ConfidenceScore,
        List<string> Recommendations);

    public record SeasonalPattern(
        Dictionary<DayOfWeek, double> DailyPattern = null);

    public record DemandForecast(
        double AverageDailyDemand,
        Dictionary<DayOfWeek, double> DailyForecasts,
        double WeeklyForecast,
        double MonthlyForecast);

    public record PriceForecast(
        decimal CurrentPrice,
        decimal PredictedChange,
        double PredictedVolatility,
        ConfidenceInterval ConfidenceInterval);

    public record ConfidenceInterval(
        decimal LowerBound,
        decimal UpperBound);

    public record OptimizationResults(
        List<OptimizationResult> Results,
        decimal TotalPotentialSavings,
        int HighImpactCount,
        int MediumImpactCount,
        int LowImpactCount);

    public record OptimizationResult(
        int ComponentId,
        string ComponentName,
        decimal CurrentValue,
        decimal OptimizedValue,
        decimal PotentialSavings,
        ImpactLevel ImpactLevel,
        List<string> Recommendations);

    public enum ImpactLevel
    {
        Low,
        Medium,
        High
    }

    public record DependencyChainAnalysis(
        int RootComponentId,
        string RootComponentName,
        List<DependencyNode> DependencyChain,
        List<CircularDependency> CircularDependencies,
        int TotalDependencies,
        int CircularDependencyCount
    );

    public record DependencyNode(
        int ComponentId,
        string ComponentName,
        int Depth,
        List<DependencyInfo> Dependencies
    );

    public record DependencyInfo(
        int DependentComponentId,
        string DependentComponentName,
        DependencyType Type,
        bool IsRequired,
        int Quantity
    );

    public record CircularDependency(
        int ComponentId,
        string ComponentName,
        int Depth
    );

    public record CompatibilityMatrix(
        List<ComponentInfo> Components,
        List<CompatibilityEntry> Matrix,
        List<CompatibilityType> CompatibilityTypes
    );

    public record ComponentInfo(
        int Id,
        string Name
    );

    public record CompatibilityEntry(
        int Component1Id,
        string Component1Name,
        int Component2Id,
        string Component2Name,
        CompatibilityType Type,
        string Description
    );

    public record DependencyImpactAnalysis(
        int ComponentId,
        string ComponentName,
        List<DependencyImpact> DirectDependencies,
        List<DependencyImpact> DependentComponents,
        int TotalImpact,
        int CriticalDependencies
    );

    public record DependencyImpact(
        int ComponentId,
        string ComponentName,
        DependencyType Type,
        bool IsRequired,
        int Quantity,
        string ImpactType
    );
} 