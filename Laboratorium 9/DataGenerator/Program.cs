using Bogus;
using DAL;
using Model;
using Microsoft.EntityFrameworkCore;
using DAL.Model;

Console.WriteLine("Generowanie danych...");


using var context = new WebStoreContext();


context.BasketPositions.ExecuteDelete();
context.OrderPositions.ExecuteDelete();
context.Orders.ExecuteDelete();
context.Products.ExecuteDelete();
context.ProductGroups.ExecuteDelete();
context.Users.ExecuteDelete();
context.UserGroups.ExecuteDelete();


var userGroupFaker = new Faker<UserGroup>()
    .RuleFor(ug => ug.Name, f => f.Commerce.Department());

var userGroups = userGroupFaker.Generate(3);
context.UserGroups.AddRange(userGroups);
await context.SaveChangesAsync();


var userFaker = new Faker<User>()
    .RuleFor(u => u.Login, f => f.Internet.UserName())
    .RuleFor(u => u.Password, f => f.Internet.Password())
    .RuleFor(u => u.Type, f => f.PickRandom<UserType>())
    .RuleFor(u => u.IsActive, f => f.Random.Bool())
    .RuleFor(u => u.GroupID, f => f.PickRandom(userGroups).ID);

var users = userFaker.Generate(10);
context.Users.AddRange(users);
await context.SaveChangesAsync();


var parentGroups = new Faker<ProductGroup>()
    .RuleFor(g => g.Name, f => f.Commerce.Categories(1)[0])
    .Generate(3);

context.ProductGroups.AddRange(parentGroups);
await context.SaveChangesAsync();

var childGroups = new Faker<ProductGroup>()
    .RuleFor(g => g.Name, f => f.Commerce.Categories(1)[0])
    .RuleFor(g => g.ParentID, f => f.PickRandom(parentGroups).ID)
    .Generate(6);

context.ProductGroups.AddRange(childGroups);
await context.SaveChangesAsync();

var allGroups = parentGroups.Concat(childGroups).ToList();


var productFaker = new Faker<Product>()
    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
    .RuleFor(p => p.Price, f => Math.Round(f.Random.Double(10, 500), 2))
    .RuleFor(p => p.Image, f => f.Image.PicsumUrl())
    .RuleFor(p => p.IsActive, f => f.Random.Bool())
    .RuleFor(p => p.GroupID, f => f.PickRandom(allGroups).ID);

var products = productFaker.Generate(20);
context.Products.AddRange(products);
await context.SaveChangesAsync();


var orderFaker = new Faker<Order>()
    .RuleFor(o => o.UserID, f => f.PickRandom(users).ID)
    .RuleFor(o => o.Date, f => f.Date.Past(1))
    .RuleFor(o => o.IsPaid, f => f.Random.Bool());

var orders = orderFaker.Generate(10);
context.Orders.AddRange(orders);
await context.SaveChangesAsync();


var orderPositions = new List<OrderPosition>();
foreach (var order in orders)
{
    var orderProducts = new List<OrderPosition>();

    foreach (var product in products.OrderBy(p => Guid.NewGuid()).Take(3))
    {
        if (!context.OrderPositions.Any(op => op.OrderID == order.ID && op.ProductId == product.ID))
        {
            orderProducts.Add(new OrderPosition
            {
                OrderID = order.ID,
                ProductId = product.ID,
                Amount = new Random().Next(1, 5),
                Price = product.Price
            });
        }
    }

    context.OrderPositions.AddRange(orderProducts);
}

await context.SaveChangesAsync();


Console.WriteLine("Dane wygenerowane!");
