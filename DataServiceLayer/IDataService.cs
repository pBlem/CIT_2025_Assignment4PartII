using DataServiceLayer.Entities;
using DataServiceLayer.Dtos;

namespace DataServiceLayer;

public interface IDataService
{
    List<Category> GetCategories();
    Category? GetCategory(int id);
    List<Product> GetProducts();
    Product? GetProduct(int id);
    void CreateCategory(Category category);
    bool UpdateCategory(Category category);
    bool DeleteCategory(int id);

    List<ProductByName> GetProductByName(string search);
    List<Category> GetCategoriesByName(string name);
}
