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
} 