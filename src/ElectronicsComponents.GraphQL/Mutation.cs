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

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentDependency> AddComponentDependency(
            AddComponentDependencyInput input,
            [Service] ApplicationDbContext context)
        {
            var dependency = new ComponentDependency
            {
                ComponentId = input.ComponentId,
                DependentComponentId = input.DependentComponentId,
                Type = input.Type,
                Description = input.Description,
                IsRequired = input.IsRequired,
                Quantity = input.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentDependencies.Add(dependency);
            await context.SaveChangesAsync();
            return dependency;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentCompatibility> AddComponentCompatibility(
            AddComponentCompatibilityInput input,
            [Service] ApplicationDbContext context)
        {
            var compatibility = new ComponentCompatibility
            {
                ComponentId = input.ComponentId,
                CompatibleComponentId = input.CompatibleComponentId,
                Type = input.Type,
                Description = input.Description,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentCompatibilities.Add(compatibility);
            await context.SaveChangesAsync();
            return compatibility;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentSpecification> AddComponentSpecification(
            AddComponentSpecificationInput input,
            [Service] ApplicationDbContext context)
        {
            var specification = new ComponentSpecification
            {
                ComponentId = input.ComponentId,
                Name = input.Name,
                Value = input.Value,
                Unit = input.Unit,
                Description = input.Description,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentSpecifications.Add(specification);
            await context.SaveChangesAsync();
            return specification;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentDocumentation> AddComponentDocumentation(
            AddComponentDocumentationInput input,
            [Service] ApplicationDbContext context)
        {
            var documentation = new ComponentDocumentation
            {
                ComponentId = input.ComponentId,
                Title = input.Title,
                Content = input.Content,
                Type = input.Type,
                FileUrl = input.FileUrl,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentDocumentation.Add(documentation);
            await context.SaveChangesAsync();
            return documentation;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentReview> AddComponentReview(
            AddComponentReviewInput input,
            [Service] ApplicationDbContext context)
        {
            var review = new ComponentReview
            {
                ComponentId = input.ComponentId,
                UserId = input.UserId,
                UserName = input.UserName,
                Rating = input.Rating,
                Comment = input.Comment,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentReviews.Add(review);

            // Update component rating
            var component = await context.Components
                .Include(c => c.Rating)
                .FirstOrDefaultAsync(c => c.Id == input.ComponentId);

            if (component.Rating == null)
            {
                component.Rating = new ComponentRating
                {
                    ComponentId = input.ComponentId,
                    AverageRating = input.Rating,
                    TotalReviews = 1,
                    FiveStarCount = input.Rating == 5 ? 1 : 0,
                    FourStarCount = input.Rating == 4 ? 1 : 0,
                    ThreeStarCount = input.Rating == 3 ? 1 : 0,
                    TwoStarCount = input.Rating == 2 ? 1 : 0,
                    OneStarCount = input.Rating == 1 ? 1 : 0,
                    LastUpdated = DateTime.UtcNow
                };
            }
            else
            {
                var rating = component.Rating;
                rating.TotalReviews++;
                rating.AverageRating = ((rating.AverageRating * (rating.TotalReviews - 1)) + input.Rating) / rating.TotalReviews;

                switch (input.Rating)
                {
                    case 5: rating.FiveStarCount++; break;
                    case 4: rating.FourStarCount++; break;
                    case 3: rating.ThreeStarCount++; break;
                    case 2: rating.TwoStarCount++; break;
                    case 1: rating.OneStarCount++; break;
                }

                rating.LastUpdated = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            return review;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentTag> AddComponentTag(
            AddComponentTagInput input,
            [Service] ApplicationDbContext context)
        {
            var tag = new ComponentTag
            {
                ComponentId = input.ComponentId,
                Name = input.Name,
                Description = input.Description,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentTags.Add(tag);
            await context.SaveChangesAsync();
            return tag;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentImage> AddComponentImage(
            AddComponentImageInput input,
            [Service] ApplicationDbContext context)
        {
            var image = new ComponentImage
            {
                ComponentId = input.ComponentId,
                Url = input.Url,
                AltText = input.AltText,
                Type = input.Type,
                Width = input.Width,
                Height = input.Height,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentImages.Add(image);
            await context.SaveChangesAsync();
            return image;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentVideo> AddComponentVideo(
            AddComponentVideoInput input,
            [Service] ApplicationDbContext context)
        {
            var video = new ComponentVideo
            {
                ComponentId = input.ComponentId,
                Url = input.Url,
                Title = input.Title,
                Description = input.Description,
                Type = input.Type,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentVideos.Add(video);
            await context.SaveChangesAsync();
            return video;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentAttachment> AddComponentAttachment(
            AddComponentAttachmentInput input,
            [Service] ApplicationDbContext context)
        {
            var attachment = new ComponentAttachment
            {
                ComponentId = input.ComponentId,
                Name = input.Name,
                FileUrl = input.FileUrl,
                FileType = input.FileType,
                FileSize = input.FileSize,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentAttachments.Add(attachment);
            await context.SaveChangesAsync();
            return attachment;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentNote> AddComponentNote(
            AddComponentNoteInput input,
            [Service] ApplicationDbContext context)
        {
            var note = new ComponentNote
            {
                ComponentId = input.ComponentId,
                Content = input.Content,
                UserId = input.UserId,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentNotes.Add(note);
            await context.SaveChangesAsync();
            return note;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentAlert> AddComponentAlert(
            AddComponentAlertInput input,
            [Service] ApplicationDbContext context)
        {
            var alert = new ComponentAlert
            {
                ComponentId = input.ComponentId,
                Message = input.Message,
                Type = input.Type,
                Severity = input.Severity,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentAlerts.Add(alert);
            await context.SaveChangesAsync();
            return alert;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentEvent> AddComponentEvent(
            AddComponentEventInput input,
            [Service] ApplicationDbContext context)
        {
            var componentEvent = new ComponentEvent
            {
                ComponentId = input.ComponentId,
                EventType = input.EventType,
                Description = input.Description,
                UserId = input.UserId,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentEvents.Add(componentEvent);
            await context.SaveChangesAsync();
            return componentEvent;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<ComponentAudit> AddComponentAudit(
            AddComponentAuditInput input,
            [Service] ApplicationDbContext context)
        {
            var audit = new ComponentAudit
            {
                ComponentId = input.ComponentId,
                Action = input.Action,
                UserId = input.UserId,
                Details = input.Details,
                CreatedAt = DateTime.UtcNow
            };

            context.ComponentAudits.Add(audit);
            await context.SaveChangesAsync();
            return audit;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> UpdateComponentAlertStatus(
            int alertId,
            bool isActive,
            [Service] ApplicationDbContext context)
        {
            var alert = await context.ComponentAlerts.FindAsync(alertId);
            if (alert == null)
                return false;

            alert.IsActive = isActive;
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentDependency(
            int dependencyId,
            [Service] ApplicationDbContext context)
        {
            var dependency = await context.ComponentDependencies.FindAsync(dependencyId);
            if (dependency == null)
                return false;

            context.ComponentDependencies.Remove(dependency);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentCompatibility(
            int compatibilityId,
            [Service] ApplicationDbContext context)
        {
            var compatibility = await context.ComponentCompatibilities.FindAsync(compatibilityId);
            if (compatibility == null)
                return false;

            context.ComponentCompatibilities.Remove(compatibility);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentSpecification(
            int specificationId,
            [Service] ApplicationDbContext context)
        {
            var specification = await context.ComponentSpecifications.FindAsync(specificationId);
            if (specification == null)
                return false;

            context.ComponentSpecifications.Remove(specification);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentDocumentation(
            int documentationId,
            [Service] ApplicationDbContext context)
        {
            var documentation = await context.ComponentDocumentation.FindAsync(documentationId);
            if (documentation == null)
                return false;

            context.ComponentDocumentation.Remove(documentation);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentReview(
            int reviewId,
            [Service] ApplicationDbContext context)
        {
            var review = await context.ComponentReviews.FindAsync(reviewId);
            if (review == null)
                return false;

            context.ComponentReviews.Remove(review);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentTag(
            int tagId,
            [Service] ApplicationDbContext context)
        {
            var tag = await context.ComponentTags.FindAsync(tagId);
            if (tag == null)
                return false;

            context.ComponentTags.Remove(tag);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentImage(
            int imageId,
            [Service] ApplicationDbContext context)
        {
            var image = await context.ComponentImages.FindAsync(imageId);
            if (image == null)
                return false;

            context.ComponentImages.Remove(image);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentVideo(
            int videoId,
            [Service] ApplicationDbContext context)
        {
            var video = await context.ComponentVideos.FindAsync(videoId);
            if (video == null)
                return false;

            context.ComponentVideos.Remove(video);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentAttachment(
            int attachmentId,
            [Service] ApplicationDbContext context)
        {
            var attachment = await context.ComponentAttachments.FindAsync(attachmentId);
            if (attachment == null)
                return false;

            context.ComponentAttachments.Remove(attachment);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentNote(
            int noteId,
            [Service] ApplicationDbContext context)
        {
            var note = await context.ComponentNotes.FindAsync(noteId);
            if (note == null)
                return false;

            context.ComponentNotes.Remove(note);
            await context.SaveChangesAsync();
            return true;
        }

        [UseDbContext(typeof(ApplicationDbContext))]
        public async Task<bool> DeleteComponentAlert(
            int alertId,
            [Service] ApplicationDbContext context)
        {
            var alert = await context.ComponentAlerts.FindAsync(alertId);
            if (alert == null)
                return false;

            context.ComponentAlerts.Remove(alert);
            await context.SaveChangesAsync();
            return true;
        }

        // Input types for mutations
        public record AddComponentDependencyInput(
            int ComponentId,
            int DependentComponentId,
            DependencyType Type,
            string Description,
            bool IsRequired,
            int Quantity);

        public record AddComponentCompatibilityInput(
            int ComponentId,
            int CompatibleComponentId,
            CompatibilityType Type,
            string Description);

        public record AddComponentSpecificationInput(
            int ComponentId,
            string Name,
            string Value,
            string Unit,
            string Description);

        public record AddComponentDocumentationInput(
            int ComponentId,
            string Title,
            string Content,
            DocumentationType Type,
            string FileUrl);

        public record AddComponentReviewInput(
            int ComponentId,
            string UserId,
            string UserName,
            int Rating,
            string Comment);

        public record AddComponentTagInput(
            int ComponentId,
            string Name,
            string Description);

        public record AddComponentImageInput(
            int ComponentId,
            string Url,
            string AltText,
            ImageType Type,
            int Width,
            int Height);

        public record AddComponentVideoInput(
            int ComponentId,
            string Url,
            string Title,
            string Description,
            VideoType Type);

        public record AddComponentAttachmentInput(
            int ComponentId,
            string Name,
            string FileUrl,
            string FileType,
            long FileSize);

        public record AddComponentNoteInput(
            int ComponentId,
            string Content,
            string UserId);

        public record AddComponentAlertInput(
            int ComponentId,
            string Message,
            AlertType Type,
            AlertSeverity Severity);

        public record AddComponentEventInput(
            int ComponentId,
            string EventType,
            string Description,
            string UserId);

        public record AddComponentAuditInput(
            int ComponentId,
            string Action,
            string UserId,
            string Details);
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