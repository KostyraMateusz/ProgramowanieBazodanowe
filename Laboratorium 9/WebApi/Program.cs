using BLL.ServiceInterfaces;
using BLL_MongoDb.ServiceMongo;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using BLL_EF.Services;
using BLL_DB;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================
            // Serwisy EF
            // ============================

            // Serwisy bazuj¹ce na Entity Framework
            // builder.Services.AddScoped<IProductService, ProductService>();
            // builder.Services.AddScoped<IBasketService, BasketService>();
            // builder.Services.AddScoped<IOrderService, OrderService>();
            // builder.Services.AddScoped<IUserService, UserService>();

            // Serwisy bazuj¹ce na bazie danych EF (przyk³adowe)
            // builder.Services.AddScoped<IProductService, ProductServiceDB>();
            // builder.Services.AddScoped<IBasketService, BasketServiceDB>();
            // builder.Services.AddScoped<IOrderService, OrderServiceDB>();
            // builder.Services.AddScoped<IUserService, UserServiceDB>();

            // builder.Services.AddDbContext<WebStoreContext>();


            // ============================
            // Serwisy MongoDB
            // ============================

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var mongoDatabase = mongoClient.GetDatabase("YourDatabaseName");

            builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

            builder.Services.AddScoped<IProductService, ProductServiceMongo>();
            builder.Services.AddScoped<IBasketService, BasketServiceMongo>();
            builder.Services.AddScoped<IOrderService, OrderServiceMongo>();
            builder.Services.AddScoped<IUserService, UserServiceMongo>();

            // ============================


            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
