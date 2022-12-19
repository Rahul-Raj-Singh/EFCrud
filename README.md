# EFCrud

 - .NET-6 Web api project with EFCore.
 - Includes Global Error Handling Middleware.
 - Problem Statement
    - Expose APIs to perform CRUD operations on Product inventory.
- Design
    - *Table*: Product, ProductPrice. A product can have multiple prices.
    - Product: **PK**- ProductName
    - ProductPrice: **PK**- ProductName, Varaint. **FK**- ProductName
    - /GetProducts: Fetches all active products with their active prices. Supports filtering on ProductName and LaunchDate.
    - /PutProducts: Upserts products and their prices.


