namespace DataServiceLayer.Dtos;

public sealed record ProductByCategory(
    int Id,
    string Name,
    decimal UnitPrice,
    string QuantityPerUnit,
    int UnitsInStock,
    string CategoryName
);
