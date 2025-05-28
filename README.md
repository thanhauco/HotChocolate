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
    totalComponents
    totalInventoryValue
    lowStockCount
    outOfStockCount
    categoryStatistics {
      category
      count
      totalValue
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