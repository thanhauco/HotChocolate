using ElectronicsComponents.GraphQL.Data;
using ElectronicsComponents.GraphQL.Models;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronicsComponents.GraphQL
{
    [ExtendObjectType(typeof(Mutation))]
    public class Mutation
    {
        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component> CreateComponent(
            CreateComponentInput input,
            [Service] ApplicationDbContext context)
        {
            var component = new Component
            {
                Name = input.Name,
                Description = input.Description,
                Category = input.Category,
                Price = input.Price,
                StockQuantity = input.StockQuantity,
                Manufacturer = input.Manufacturer,
                PartNumber = input.PartNumber
            };

            context.Components.Add(component);
            await context.SaveChangesAsync();

            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateComponent(
            UpdateComponentInput input,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(input.Id);
            if (component == null)
                return null;

            if (input.Name != null)
                component.Name = input.Name;
            if (input.Description != null)
                component.Description = input.Description;
            if (input.Category != null)
                component.Category = input.Category;
            if (input.Price.HasValue)
                component.Price = input.Price.Value;
            if (input.StockQuantity.HasValue)
                component.StockQuantity = input.StockQuantity.Value;
            if (input.Manufacturer != null)
                component.Manufacturer = input.Manufacturer;
            if (input.PartNumber != null)
                component.PartNumber = input.PartNumber;

            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponent(
            int id,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(id);
            if (component == null)
                return false;

            context.Components.Remove(component);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateStock(
            int id,
            int quantity,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(id);
            if (component == null)
                return null;

            component.StockQuantity += quantity;
            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdatePrice(
            int id,
            decimal newPrice,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(id);
            if (component == null)
                return null;

            component.Price = newPrice;
            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> BulkUpdateStock(
            int[] ids,
            int quantity,
            [Service] ApplicationDbContext context)
        {
            var components = await context.Components
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            if (!components.Any())
                return null;

            foreach (var component in components)
            {
                component.StockQuantity += quantity;
            }

            await context.SaveChangesAsync();
            return components.First();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateComponentPrice(
            int id,
            decimal newPrice,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(id);
            if (component == null)
                return null;

            component.Price = newPrice;
            // You could add price history tracking here
            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> BulkDeleteComponents(
            int[] ids,
            [Service] ApplicationDbContext context)
        {
            var components = await context.Components
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            if (!components.Any())
                return false;

            context.Components.RemoveRange(components);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateComponentCategory(
            int id,
            string newCategory,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(id);
            if (component == null)
                return null;

            component.Category = newCategory;
            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateComponentManufacturer(
            int id,
            string newManufacturer,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(id);
            if (component == null)
                return null;

            component.Manufacturer = newManufacturer;
            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateComponentPartNumber(
            int id,
            string newPartNumber,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(id);
            if (component == null)
                return null;

            component.PartNumber = newPartNumber;
            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateComponentWithHistory(
            UpdateComponentWithHistoryInput input,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(input.Id);
            if (component == null)
                return null;

            // Store old values for history
            var oldPrice = component.Price;
            var oldStock = component.StockQuantity;

            // Update component
            if (input.Name != null)
                component.Name = input.Name;
            if (input.Description != null)
                component.Description = input.Description;
            if (input.Category != null)
                component.Category = input.Category;
            if (input.Price.HasValue)
                component.Price = input.Price.Value;
            if (input.StockQuantity.HasValue)
                component.StockQuantity = input.StockQuantity.Value;
            if (input.Manufacturer != null)
                component.Manufacturer = input.Manufacturer;
            if (input.PartNumber != null)
                component.PartNumber = input.PartNumber;

            // Here you would typically save the history to a separate table
            // For example: await SaveComponentHistory(component.Id, oldPrice, oldStock, input.Reason);

            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> BulkUpdateWithValidation(
            BulkUpdateInput input,
            [Service] ApplicationDbContext context)
        {
            var components = await context.Components
                .Where(c => input.Ids.Contains(c.Id))
                .ToListAsync();

            if (!components.Any())
                return null;

            foreach (var component in components)
            {
                if (input.Price.HasValue && input.Price.Value > 0)
                    component.Price = input.Price.Value;
                
                if (input.StockQuantity.HasValue)
                {
                    var newStock = component.StockQuantity + input.StockQuantity.Value;
                    if (newStock >= 0) // Prevent negative stock
                        component.StockQuantity = newStock;
                }

                if (input.Category != null)
                    component.Category = input.Category;

                if (input.Manufacturer != null)
                    component.Manufacturer = input.Manufacturer;
            }

            await context.SaveChangesAsync();
            return components.First();
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateComponentWithValidation(
            UpdateComponentWithValidationInput input,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(input.Id);
            if (component == null)
                return null;

            // Validate price
            if (input.Price.HasValue)
            {
                if (input.Price.Value < 0)
                    throw new GraphQLException("Price cannot be negative");
                component.Price = input.Price.Value;
            }

            // Validate stock
            if (input.StockQuantity.HasValue)
            {
                if (input.StockQuantity.Value < 0)
                    throw new GraphQLException("Stock quantity cannot be negative");
                component.StockQuantity = input.StockQuantity.Value;
            }

            // Update other fields
            if (input.Name != null)
                component.Name = input.Name;
            if (input.Description != null)
                component.Description = input.Description;
            if (input.Category != null)
                component.Category = input.Category;
            if (input.Manufacturer != null)
                component.Manufacturer = input.Manufacturer;
            if (input.PartNumber != null)
                component.PartNumber = input.PartNumber;

            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateComponentWithInventoryRules(
            UpdateComponentWithInventoryRulesInput input,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(input.Id);
            if (component == null)
                return null;

            // Apply inventory rules
            if (input.StockQuantity.HasValue)
            {
                if (input.StockQuantity.Value < 0)
                    throw new GraphQLException("Stock quantity cannot be negative");

                if (input.StockQuantity.Value > input.MaxStockLimit)
                    throw new GraphQLException($"Stock quantity cannot exceed {input.MaxStockLimit}");

                if (input.StockQuantity.Value < input.MinStockLimit)
                    throw new GraphQLException($"Stock quantity cannot be below {input.MinStockLimit}");

                component.StockQuantity = input.StockQuantity.Value;
            }

            // Apply price rules
            if (input.Price.HasValue)
            {
                if (input.Price.Value < 0)
                    throw new GraphQLException("Price cannot be negative");

                if (input.Price.Value > input.MaxPriceLimit)
                    throw new GraphQLException($"Price cannot exceed {input.MaxPriceLimit}");

                if (input.Price.Value < input.MinPriceLimit)
                    throw new GraphQLException($"Price cannot be below {input.MinPriceLimit}");

                component.Price = input.Price.Value;
            }

            // Update other fields
            if (input.Name != null)
                component.Name = input.Name;
            if (input.Description != null)
                component.Description = input.Description;
            if (input.Category != null)
                component.Category = input.Category;
            if (input.Manufacturer != null)
                component.Manufacturer = input.Manufacturer;
            if (input.PartNumber != null)
                component.PartNumber = input.PartNumber;

            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<Component?> UpdateComponentWithPriceHistory(
            UpdateComponentWithPriceHistoryInput input,
            [Service] ApplicationDbContext context)
        {
            var component = await context.Components.FindAsync(input.Id);
            if (component == null)
                return null;

            var oldPrice = component.Price;
            var oldStock = component.StockQuantity;

            // Update component
            if (input.Price.HasValue)
            {
                if (input.Price.Value < 0)
                    throw new GraphQLException("Price cannot be negative");

                // Calculate price change percentage
                var priceChangePercent = ((input.Price.Value - oldPrice) / oldPrice) * 100;

                // Check if price change is within allowed limits
                if (Math.Abs(priceChangePercent) > input.MaxPriceChangePercent)
                    throw new GraphQLException($"Price change of {priceChangePercent:F2}% exceeds maximum allowed change of {input.MaxPriceChangePercent}%");

                component.Price = input.Price.Value;
            }

            if (input.StockQuantity.HasValue)
            {
                if (input.StockQuantity.Value < 0)
                    throw new GraphQLException("Stock quantity cannot be negative");

                component.StockQuantity = input.StockQuantity.Value;
            }

            // Update other fields
            if (input.Name != null)
                component.Name = input.Name;
            if (input.Description != null)
                component.Description = input.Description;
            if (input.Category != null)
                component.Category = input.Category;
            if (input.Manufacturer != null)
                component.Manufacturer = input.Manufacturer;
            if (input.PartNumber != null)
                component.PartNumber = input.PartNumber;

            // Here you would typically save the price history to a separate table
            // For example: await SavePriceHistory(component.Id, oldPrice, input.Price.Value, input.Reason);

            await context.SaveChangesAsync();
            return component;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> BulkUpdateStock([Service] ApplicationDbContext context, List<StockUpdateInput> updates)
        {
            var components = new List<Component>();
            
            foreach (var update in updates)
            {
                var component = await context.Components.FindAsync(update.ComponentId);
                if (component != null)
                {
                    component.StockQuantity = update.NewStockQuantity;
                    components.Add(component);
                }
            }

            await context.SaveChangesAsync();
            return components;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> BulkUpdatePrices([Service] ApplicationDbContext context, List<PriceUpdateInput> updates)
        {
            var components = new List<Component>();
            
            foreach (var update in updates)
            {
                var component = await context.Components.FindAsync(update.ComponentId);
                if (component != null)
                {
                    if (update.NewPrice <= 0)
                    {
                        throw new GraphQLException("Price must be greater than 0");
                    }
                    component.Price = update.NewPrice;
                    components.Add(component);
                }
            }

            await context.SaveChangesAsync();
            return components;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<List<Component>> BulkDeleteComponents([Service] ApplicationDbContext context, List<int> componentIds)
        {
            var components = await context.Components
                .Where(c => componentIds.Contains(c.Id))
                .ToListAsync();

            context.Components.RemoveRange(components);
            await context.SaveChangesAsync();
            return components;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<InventoryOptimizationResult> OptimizeInventory(
            InventoryOptimizationInput input,
            [Service] ApplicationDbContext context)
        {
            var components = await context.Components
                .Where(c => input.Categories.Contains(c.Category))
                .ToListAsync();

            var recommendations = new List<InventoryRecommendation>();
            var totalCost = 0m;
            var totalSavings = 0m;

            foreach (var component in components)
            {
                var recommendation = new InventoryRecommendation
                {
                    ComponentId = component.Id,
                    ComponentName = component.Name,
                    CurrentStock = component.StockQuantity,
                    RecommendedStock = CalculateOptimalStock(component, input),
                    Cost = component.Price * component.StockQuantity
                };

                if (recommendation.RecommendedStock != component.StockQuantity)
                {
                    var stockDifference = recommendation.RecommendedStock - component.StockQuantity;
                    var costDifference = stockDifference * component.Price;
                    
                    if (stockDifference > 0)
                    {
                        totalCost += costDifference;
                    }
                    else
                    {
                        totalSavings += Math.Abs(costDifference);
                    }

                    recommendations.Add(recommendation);
                }
            }

            return new InventoryOptimizationResult
            {
                Recommendations = recommendations,
                TotalCost = totalCost,
                TotalSavings = totalSavings,
                NetCost = totalCost - totalSavings
            };
        }

        private int CalculateOptimalStock(Component component, InventoryOptimizationInput input)
        {
            // Simple optimization logic - can be enhanced with more sophisticated algorithms
            var baseStock = input.BaseStockLevel;
            var categoryMultiplier = input.CategoryMultipliers.GetValueOrDefault(component.Category, 1.0);
            var priceFactor = component.Price > input.PriceThreshold ? 0.8 : 1.2;

            return (int)(baseStock * categoryMultiplier * priceFactor);
        }
    }

    public record CreateComponentInput(
        string Name,
        string Description,
        string Category,
        decimal Price,
        int StockQuantity,
        string Manufacturer,
        string PartNumber);

    public record UpdateComponentInput(
        int Id,
        string? Name = null,
        string? Description = null,
        string? Category = null,
        decimal? Price = null,
        int? StockQuantity = null,
        string? Manufacturer = null,
        string? PartNumber = null);

    public record UpdateComponentWithHistoryInput(
        int Id,
        string? Name = null,
        string? Description = null,
        string? Category = null,
        decimal? Price = null,
        int? StockQuantity = null,
        string? Manufacturer = null,
        string? PartNumber = null,
        string? Reason = null);

    public record BulkUpdateInput(
        int[] Ids,
        decimal? Price = null,
        int? StockQuantity = null,
        string? Category = null,
        string? Manufacturer = null);

    public record UpdateComponentWithValidationInput(
        int Id,
        string? Name = null,
        string? Description = null,
        string? Category = null,
        decimal? Price = null,
        int? StockQuantity = null,
        string? Manufacturer = null,
        string? PartNumber = null);

    public record UpdateComponentWithInventoryRulesInput(
        int Id,
        string? Name = null,
        string? Description = null,
        string? Category = null,
        decimal? Price = null,
        int? StockQuantity = null,
        string? Manufacturer = null,
        string? PartNumber = null,
        int MaxStockLimit = 1000,
        int MinStockLimit = 0,
        decimal MaxPriceLimit = 1000,
        decimal MinPriceLimit = 0);

    public record UpdateComponentWithPriceHistoryInput(
        int Id,
        string? Name = null,
        string? Description = null,
        string? Category = null,
        decimal? Price = null,
        int? StockQuantity = null,
        string? Manufacturer = null,
        string? PartNumber = null,
        string? Reason = null,
        decimal MaxPriceChangePercent = 20);

    public record StockUpdateInput(int ComponentId, int NewStockQuantity);
    public record PriceUpdateInput(int ComponentId, decimal NewPrice);

    public record InventoryOptimizationInput(
        string[] Categories,
        int BaseStockLevel = 50,
        decimal PriceThreshold = 100,
        Dictionary<string, double> CategoryMultipliers = null);

    public record InventoryOptimizationResult(
        List<InventoryRecommendation> Recommendations,
        decimal TotalCost,
        decimal TotalSavings,
        decimal NetCost);

    public record InventoryRecommendation(
        int ComponentId,
        string ComponentName,
        int CurrentStock,
        int RecommendedStock,
        decimal Cost);
} 