# EFCrud

 - .NET-6 Web api project with EFCore and XUnit tests.
 - Includes Global Error Handling Middleware.
 - Problem Statement
    - Expose APIs to perform CRUD operations on Product inventory.
- Design
    - *Table*: Product, ProductPrice. A product can have multiple prices.
    - Product: **PK**- ProductName
    - ProductPrice: **PK**- ProductName, Varaint. **FK**- ProductName
    - /GetProducts: Fetches all active products with their active prices. Supports filtering on ProductName and LaunchDate.
    - /PutProducts: Upserts products and their prices.
- Testing
    - Used Moq and XUnit to write unit tests.
    - Repository layer is tested using in-memory SQLite db. 


<details>

<summary>To generate test code coverage:</summary>

```
dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/Program.cs
```

```
reportgenerator -reports:"TestResults\*\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```
</details>