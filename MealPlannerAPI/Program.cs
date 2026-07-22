using FluentValidation;
using MealPlannerAPI.Context;
using MealPlannerAPI.Models.DTOs.Request;
using MealPlannerAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register a DbContext factory so callers can create short-lived
// PlannerContext instances (safe for background or parallel usage).
builder.Services.AddDbContextFactory<PlannerContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));

    // Enable sensitive data logging only in Development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Automatically scans and registers all validators from the assembly
builder.Services.AddValidatorsFromAssemblyContaining<IngredientRequestDTO>();
builder.Services.AddValidatorsFromAssemblyContaining<InventoryRequestDTO>();
builder.Services.AddValidatorsFromAssemblyContaining<RecipeRequestDTO>();
//builder.Services.AddValidatorsFromAssemblyContaining<UpdateIngredientDTO>();
//builder.Services.AddValidatorsFromAssemblyContaining<UpdateInventoryDTO>();

WebApplication app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<PlannerContext>();
//        context.Database.EnsureDeleted();
//        context.Database.EnsureCreated();
//        context.Database.Migrate();
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
//    }
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
