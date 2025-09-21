using AIDBDataFeatch.AIOperations;
using AIDBDataFeatch.DataAccessLayer;
using AIDBDataFeatch.DataAccessLayer.GetDataByQuery;
using AIDBDataFeatch.DataAccessLayer.GetSchemaDetails;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;


var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

//builder.Services.AddDbContext<DBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DBConnection"))
           ); // ✅ Use compiled model

builder.Services.TryAddScoped<ISchemaDetails, SchemaDetails>();

builder.Services.TryAddScoped<IGetDataByQuery, GetDataByQuery>();

builder.Services.TryAddScoped<IAIOperations, AIOpeartions>();



// ✅ Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});
builder.Services.AddControllers();

// ✅ Register the regex constraint
builder.Services.Configure<RouteOptions>(options =>
{
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    // ✅ Enable Swagger in development
    app.UseSwagger();
    //app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API V1");
    c.RoutePrefix = ""; // <- Open Swagger at root URL (optional)
});
//}


var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

//var todosApi = app.MapGroup("/todos");
//todosApi.MapGet("/", () => sampleTodos);
//todosApi.MapGet("/{id}", (int id) =>
//    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
//        ? Results.Ok(todo)
//        : Results.NotFound());

app.MapPost("/GetData", async (string text, ISchemaDetails schemaDetails, IAIOperations aIOperations, IGetDataByQuery getDataByQuery) =>
{
    var schema = await schemaDetails.GetSchema();
    var query = await aIOperations.GetDataFromLLM(schema, text);
    await getDataByQuery.GetDatafromDB(query);
});
app.UseRouting();
app.MapControllers();

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

