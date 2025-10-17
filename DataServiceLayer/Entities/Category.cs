namespace DataServiceLayer.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<Product> Products { get; set; } // NOTE: might need initializion as shown in Perez p. 24 (Unsure)
}
