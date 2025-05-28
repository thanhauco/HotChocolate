# Electronics Components GraphQL API

A GraphQL API for managing electronics components inventory built with ASP.NET Core, HotChocolate, and MySQL.

## Prerequisites

- .NET 7.0 SDK or later
- MySQL Server
- Docker (optional, for containerized setup)

## Setup and Run

### 1. Clone the Repository
```bash
git clone <repository-url>
cd ElectronicsComponents
```

### 2. Configure Database
1. Create a MySQL database named `electronics_components`
2. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=electronics_components;User=your_username;Password=your_password;"
  }
}
```

### 3. Run Database Migrations
```bash
cd src/ElectronicsComponents.GraphQL
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Run the Application
```bash
dotnet run
```

The GraphQL API will be available at:
- GraphQL endpoint: `http://localhost:5000/graphql`
- GraphQL IDE (Banana): `http://localhost:5000/graphql/`

## Sample Queries

### Get All Components
```graphql
query {
  getComponents {
    id
    name
    description
    category
    price
    stockQuantity
    manufacturer
    partNumber
  }
}
```

### Get Components by Category
```graphql
query {
  getComponentsByCategory(category: "Resistors") {
    id
    name
    price
    stockQuantity
  }
}
```

### Get Low Stock Components
```graphql
query {
  getLowStockAlertComponents(threshold: 10) {
    id
    name
    stockQuantity
  }
}
```

### Create New Component
```graphql
mutation {
  createComponent(input: {
    name: "Arduino Uno"
    description: "ATmega328P microcontroller board"
    category: "Microcontrollers"
    price: 19.99
    stockQuantity: 50
    manufacturer: "Arduino"
    partNumber: "A000066"
  }) {
    id
    name
    price
  }
}
```

### Get Component Analytics
```graphql
query {
  getComponentAnalytics {
    totalCount
    totalValue
    lowStockCount
    outOfStockCount
    averagePrice
    mostExpensiveComponent
    leastExpensiveComponent
    mostStockedComponent
    leastStockedComponent
  }
}
```

### Get Category Statistics
```graphql
query {
  getCategoryStatistics {
    totalCategories
    categoryBreakdown {
      category
      count
      totalValue
      averagePrice
      lowStockCount
      outOfStockCount
    }
  }
}
```

### Get Price Statistics
```graphql
query {
  getPriceStatistics {
    averagePrice
    minPrice
    maxPrice
    priceRanges {
      range
      count
    }
  }
}
```

### Get Stock Statistics
```graphql
query {
  getStockStatistics {
    totalStock
    averageStock
    lowStockCount
    outOfStockCount
    overstockedCount
    stockRanges {
      range
      count
    }
  }
}
```

### Get Components by Price Range
```graphql
query {
  getComponentsByPriceRange(minPrice: 10.0, maxPrice: 50.0) {
    id
    name
    price
    stockQuantity
  }
}
```

### Get Components by Manufacturer
```graphql
query {
  getComponentsByManufacturer(manufacturer: "Arduino") {
    id
    name
    price
    stockQuantity
  }
}
```

### Bulk Update Stock
```graphql
mutation {
  bulkUpdateStock(updates: [
    { componentId: 1, newStockQuantity: 100 },
    { componentId: 2, newStockQuantity: 50 }
  ]) {
    id
    name
    stockQuantity
  }
}
```

### Bulk Update Prices
```graphql
mutation {
  bulkUpdatePrices(updates: [
    { componentId: 1, newPrice: 19.99 },
    { componentId: 2, newPrice: 29.99 }
  ]) {
    id
    name
    price
  }
}
```

### Bulk Delete Components
```graphql
mutation {
  bulkDeleteComponents(componentIds: [1, 2, 3]) {
    id
    name
  }
}
```

### Get Component Trends
```graphql
query {
  getComponentTrends {
    priceTrends {
      category
      averagePrice
      minPrice
      maxPrice
    }
    stockTrends {
      category
      totalStock
      lowStockCount
      outOfStockCount
    }
  }
}
```

### Get Inventory Health
```graphql
query {
  getInventoryHealth {
    totalComponents
    totalValue
    lowStockItems
    outOfStockItems
    overstockedItems
    healthScore
  }
}
```

### Optimize Inventory
```graphql
mutation {
  optimizeInventory(input: {
    categories: ["Resistors", "Capacitors"],
    baseStockLevel: 50,
    priceThreshold: 100,
    categoryMultipliers: {
      "Resistors": 1.2,
      "Capacitors": 0.8
    }
  }) {
    recommendations {
      componentId
      componentName
      currentStock
      recommendedStock
      cost
    }
    totalCost
    totalSavings
    netCost
  }
}
```

### Get Supplier Analytics
```graphql
query {
  getSupplierAnalytics {
    totalSuppliers
    totalComponents
    averageComponentsPerSupplier
    topSuppliers {
      name
      componentCount
      totalValue
      averagePrice
    }
  }
}
```

### Get Component Lifecycle
```graphql
query {
  getComponentLifecycle(componentId: 1) {
    componentId
    componentName
    currentStatus
    priceHistory {
      oldPrice
      newPrice
      changedAt
      changedBy
      reason
    }
    stockHistory {
      oldQuantity
      newQuantity
      changedAt
      changedBy
      reason
    }
    lastRestockDate
    lastPriceUpdateDate
    daysSinceLastRestock
    daysSinceLastPriceUpdate
  }
}
```

### Get Supplier Performance
```graphql
query {
  getSupplierPerformance(supplierId: 1) {
    supplierId
    supplierName
    totalComponents
    totalValue
    averagePrice
    lowStockCount
    outOfStockCount
    overstockedCount
    healthScore
    componentBreakdown {
      byCategory {
        category
        count
        totalValue
      }
      byStatus {
        active
        discontinued
        outOfStock
        lowStock
        overstocked
      }
    }
  }
}
```

### Get Inventory Predictions
```graphql
query {
  getInventoryPredictions {
    predictions {
      componentId
      componentName
      currentStock
      averageDailyConsumption
      daysUntilLowStock
      recommendedOrderQuantity
      urgencyLevel
      confidenceScore
    }
    highUrgencyCount
    mediumUrgencyCount
    lowUrgencyCount
  }
}
```

### Get Price Predictions
```graphql
query {
  getPricePredictions {
    predictions {
      componentId
      componentName
      currentPrice
      predictedPrice
      priceTrend
      priceVolatility
      confidenceScore
      recommendation
    }
    increasingPriceCount
    decreasingPriceCount
    stablePriceCount
  }
}
```

### Get Inventory Recommendations
```graphql
query {
  getInventoryRecommendations {
    recommendations {
      componentId
      componentName
      priority
      action
      reasoning
      stockPrediction {
        currentStock
        averageDailyConsumption
        daysUntilLowStock
        recommendedOrderQuantity
        urgencyLevel
        confidenceScore
      }
      pricePrediction {
        currentPrice
        predictedPrice
        priceTrend
        priceVolatility
        confidenceScore
        recommendation
      }
    }
    highPriorityCount
    mediumPriorityCount
    lowPriorityCount
  }
}
```

### Get ML Predictions
```graphql
query {
  getMLPredictions {
    predictions {
      componentId
      componentName
      seasonalPattern {
        dailyPattern
      }
      demandForecast {
        averageDailyDemand
        dailyForecasts
        weeklyForecast
        monthlyForecast
      }
      priceForecast {
        currentPrice
        predictedChange
        predictedVolatility
        confidenceInterval {
          lowerBound
          upperBound
        }
      }
      optimalPrice
      optimalStock
      confidenceScore
      recommendations
    }
    highConfidenceCount
    mediumConfidenceCount
    lowConfidenceCount
  }
}
```

### Get Optimization Results
```graphql
query {
  getOptimizationResults {
    results {
      componentId
      componentName
      currentValue
      optimizedValue
      potentialSavings
      impactLevel
      recommendations
    }
    totalPotentialSavings
    highImpactCount
    mediumImpactCount
    lowImpactCount
  }
}
```

### Get Real-Time Monitoring
```graphql
query {
  getRealTimeMonitoring {
    components {
      componentId
      componentName
      currentStock
      currentPrice
      stockVelocity
      priceVelocity
      stockTrend
      priceTrend
      alerts {
        type
        message
        severity
        timestamp
      }
      lastUpdated
    }
    criticalAlerts
    warningAlerts
    infoAlerts
  }
}
```

### Get Automated Alerts
```graphql
query {
  getAutomatedAlerts {
    alerts {
      componentId
      componentName
      type
      message
      severity
      timestamp
    }
    criticalCount
    warningCount
    infoCount
  }
}
```

## Features

### Queries
- Get all components with pagination, filtering, and sorting
- Get components by ID, category, manufacturer
- Search components by name, description, or part number
- Get components by price range or stock range
- Get components by inventory status (in stock, low stock, out of stock)
- Get components by price category (budget, mid-range, premium)
- Get analytics and statistics
- Get alerts for low stock, high price, etc.

### Mutations
- Create new components
- Update component details
- Delete components
- Update stock quantities
- Update prices with validation
- Bulk operations

## Development

### Project Structure
```
src/
  ElectronicsComponents.GraphQL/
    Data/              # Database context and configurations
    DataLoaders/       # DataLoader implementations
    Models/           # Entity models
    Query.cs          # GraphQL queries
    Mutation.cs       # GraphQL mutations
```

### Adding New Features
1. Add new models in the `Models` directory
2. Update `ApplicationDbContext` if needed
3. Add new queries in `Query.cs`
4. Add new mutations in `Mutation.cs`
5. Create new DataLoaders if needed

## Docker Support

### Build and Run with Docker
```bash
docker build -t electronics-components .
docker run -p 5000:5000 electronics-components
```

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License
MIT License 

## Advanced Features

### Machine Learning & Predictive Analytics
- Seasonal pattern detection for demand forecasting
- Price trend analysis and volatility prediction
- Optimal price calculation based on demand elasticity
- Optimal stock level recommendations
- Confidence scoring for predictions
- Automated recommendations for price and stock adjustments

### Inventory Optimization
- Component-level optimization analysis
- Potential savings calculation
- Impact assessment for optimization recommendations
- Automated stock level optimization
- Price optimization based on demand patterns
- Seasonal demand forecasting
- Lead time consideration in stock calculations

### Advanced Analytics
- Price elasticity analysis
- Demand forecasting with seasonal patterns
- Confidence intervals for predictions
- Impact level assessment
- Automated recommendation generation
- Historical pattern analysis
- Volatility monitoring 

### Real-Time Monitoring
- Live stock level monitoring
- Price change velocity tracking
- Stock movement velocity tracking
- Trend analysis for stock and prices
- Real-time alert generation
- Component status tracking
- Last update timestamps

### Automated Alerts
- Critical stock level alerts
- Price volatility alerts
- Trend-based alerts
- Multi-factor alert generation
- Severity-based alert categorization
- Timestamp tracking
- Alert history

### Alert Types
- Stock alerts (low stock, rapid decrease)
- Price alerts (volatility, trends)
- Trend alerts (combined stock and price analysis)
- Severity levels (Info, Warning, Critical)
- Real-time notifications
- Historical alert tracking 