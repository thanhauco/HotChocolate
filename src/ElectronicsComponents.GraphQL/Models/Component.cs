using System;
using System.Collections.Generic;

namespace ElectronicsComponents.GraphQL.Models
{
    public class Component
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Manufacturer { get; set; } = null!;
        public string PartNumber { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumStockLevel { get; set; }
        public int MaximumStockLevel { get; set; }
        public string Location { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PriceHistory> PriceHistory { get; set; } = new();
        public List<StockHistory> StockHistory { get; set; } = new();
        public Supplier? Supplier { get; set; }
        public int? SupplierId { get; set; }
        public ComponentStatus Status { get; set; }
        public DateTime? LastRestockDate { get; set; }
        public DateTime? LastPriceUpdateDate { get; set; }
        public string? Notes { get; set; }
        public List<ComponentDependency> Dependencies { get; set; } = new();
        public List<ComponentDependency> DependentComponents { get; set; } = new();
        public List<ComponentCompatibility> CompatibleComponents { get; set; } = new();
        public List<ComponentSpecification> Specifications { get; set; } = new();
        public List<ComponentDocumentation> Documentation { get; set; } = new();
        public List<ComponentReview> Reviews { get; set; } = new();
        public ComponentRating Rating { get; set; }
        public List<ComponentTag> Tags { get; set; } = new();
        public List<ComponentImage> Images { get; set; } = new();
        public List<ComponentVideo> Videos { get; set; } = new();
        public List<ComponentAttachment> Attachments { get; set; } = new();
        public List<ComponentNote> Notes { get; set; } = new();
        public List<ComponentAlert> Alerts { get; set; } = new();
        public List<ComponentEvent> Events { get; set; } = new();
        public List<ComponentAudit> AuditTrail { get; set; } = new();
    }

    public class PriceHistory
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; } = null!;
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }
        public string? Reason { get; set; }
    }

    public class StockHistory
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; } = null!;
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }
        public string? Reason { get; set; }
    }

    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ContactPerson { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public List<Component> Components { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Notes { get; set; }
    }

    public enum ComponentStatus
    {
        Active,
        Discontinued,
        OutOfStock,
        LowStock,
        Overstocked
    }

    public class ComponentDependency
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public int DependentComponentId { get; set; }
        public Component DependentComponent { get; set; }
        public DependencyType Type { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentCompatibility
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public int CompatibleComponentId { get; set; }
        public Component CompatibleComponent { get; set; }
        public CompatibilityType Type { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentSpecification
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentDocumentation
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DocumentationType Type { get; set; }
        public string FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentReview
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentRating
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ComponentTag
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentImage
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Url { get; set; }
        public string AltText { get; set; }
        public ImageType Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentVideo
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public VideoType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentAttachment
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentNote
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentAlert
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Message { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentEvent
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentAudit
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string Action { get; set; }
        public string UserId { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum DependencyType
    {
        Required,
        Optional,
        Alternative,
        Compatible
    }

    public enum CompatibilityType
    {
        Direct,
        Indirect,
        Partial,
        Conditional
    }

    public enum DocumentationType
    {
        Manual,
        Datasheet,
        Tutorial,
        FAQ,
        Guide
    }

    public enum ImageType
    {
        Main,
        Thumbnail,
        Gallery,
        Diagram,
        Schematic
    }

    public enum VideoType
    {
        Tutorial,
        Review,
        Demonstration,
        Installation
    }

    public enum AlertType
    {
        Stock,
        Price,
        Quality,
        Discontinuation,
        Recall
    }

    public enum AlertSeverity
    {
        Info,
        Warning,
        Critical
    }
} 