using DataServiceLayer;
using Mapster;

namespace WebServiceLayer;

public class Program
{
    public static DataService DataService { get; } = new DataService();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IDataService, DataService>();

        // Add services to the container.

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
