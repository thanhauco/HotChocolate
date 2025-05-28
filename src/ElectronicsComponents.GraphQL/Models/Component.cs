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