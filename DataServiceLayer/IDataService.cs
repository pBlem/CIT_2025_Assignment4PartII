using DataServiceLayer.Entities;
using DataServiceLayer.Dtos;

namespace DataServiceLayer;

public interface IDataService
{
    /////////////////////////////////////////////////
    // Categories
    /////////////////////////////////////////////////

    List<Category> GetCategories();
    Category? GetCategory(int id);
    // void CreateCategory(Category category);
    // bool UpdateCategory(Category category);
    bool DeleteCategory(int id);
    // List<Category> GetCategoriesByName(string name);

    /////////////////////////////////////////////////
    // Products
    /////////////////////////////////////////////////

    // int GetProductCount();
    Product? GetProduct(int id);
    // List<Product> GetProducts(int page, int pageSize);
    List<ProductByName> GetProductByName(string search);
}
