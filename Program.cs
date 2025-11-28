using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;
using PizzaStore.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger (API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register PizzaDb with SQLite provider
builder.Services.AddDbContext<PizzaDb>(options =>
    options.UseSqlite("Data Source=pizzastore.db"));

var app = builder.Build();

// Enable Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
    c.RoutePrefix = "swagger"; // Swagger UI available at /swagger
});

// Make sure the database file and tables exist on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaDb>();
    db.Database.EnsureCreated();

    // Seed initial pizzas if table is empty
    if (!db.Pizzas.Any())
    {
        db.Pizzas.AddRange(
            new Pizza { Name = "Margherita", Description = "Classic cheese and tomato pizza" },
            new Pizza { Name = "Pepperoni", Description = "Spicy pepperoni with cheese" },
            new Pizza { Name = "Hawaiian", Description = "Ham and pineapple" }
        );
        db.SaveChanges();
    }
}

app.MapGet("/", () => "PizzaStore - SQLite");

// Get all pizzas
app.MapGet("/pizzas", async (PizzaDb db) =>
    await db.Pizzas.ToListAsync());

// Create a new pizza
app.MapPost("/pizzas", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizzas/{pizza.Id}", pizza);
});

// Update a pizza
app.MapPut("/pizzas/{id}", async (PizzaDb db, int id, Pizza updated) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();
    pizza.Name = updated.Name;
    pizza.Description = updated.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete a pizza
app.MapDelete("/pizzas/{id}", async (PizzaDb db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
