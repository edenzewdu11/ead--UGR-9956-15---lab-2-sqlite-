# PizzaStore (SQLite) 

Files:
- Program.cs: Minimal API using EF Core SQLite
- Models/Pizza.cs: Entity class
- Data/PizzaDb.cs: DbContext

Program.cs (high-level)
1. using Microsoft.EntityFrameworkCore;
   - Import EF Core types (DbContext, UseSqlite, ToListAsync, etc.)
2. using PizzaStore.Data; using PizzaStore.Models;
   - Import our project namespaces for DbContext and Pizza entity.

Builder and services:
3. var builder = WebApplication.CreateBuilder(args);
   - Create the web application builder.
4. builder.Services.AddEndpointsApiExplorer();
   - Adds minimal API endpoint metadata used by Swagger.
5. builder.Services.AddSwaggerGen();
   - Registers Swagger generator for API docs.
6. builder.Services.AddDbContext<PizzaDb>(options => options.UseSqlite("Data Source=pizzastore.db"));
   - Registers the PizzaDb DbContext and configures it to use SQLite.
   - 'pizzastore.db' will be created in the app working directory when EnsureCreated or migrations run.

Build and ensure DB:
7. var app = builder.Build();
   - Build the app pipeline.
8. if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
   - Enable Swagger only in development.
9. using (var scope = app.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<PizzaDb>(); db.Database.EnsureCreated(); }
   - Creates the database file and tables if they don't exist. (Simpler than migrations for examples.)

Endpoints:
10. app.MapGet("/", () => "PizzaStore - SQLite");
   - Root endpoint.
11. app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());
   - Query all pizzas.
12. app.MapPost("/pizzas", async (PizzaDb db, Pizza pizza) => { await db.Pizzas.AddAsync(pizza); await db.SaveChangesAsync(); return Results.Created($"/pizzas/{pizza.Id}", pizza); });
   - Insert pizza; SQLite will persist data to the .db file.
13. app.MapPut("/pizzas/{id}", ...)
   - Update by id.
14. app.MapDelete("/pizzas/{id}", ...)
   - Delete by id.
15. app.Run();
   - Run the web application.

Notes:
- SQLite is file-based and persists between restarts.
- For production or more advanced scenarios use EF Core migrations instead of EnsureCreated.
