using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

var allowSpecifications = "allowSpecifications";

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowSpecifications, policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var categories = new List<Category>
{
    new(1, "Informática", "Produtos de informática"),
    new(2, "Limpeza", "Produtos de limpeza"),
    new(3, "Cozinha", "Produtos de cozinha"),
    new(4, "Decoração", "Produtos de decoração"),
};

app.MapGet("/v1/categories", () => categories);
app.MapGet("/v1/categories/{id:int}", (int id) => categories.FirstOrDefault(c => c.Id == id));
app.MapPost("/v1/categories", (Category model) =>
{
    model.Id = categories.Count + 1;
    categories.Add(model);

    return Results.Created($"/v1/categories/{model.Id}", model);
});
app.MapPut("/v1/categories/{id:int}", (int id, Category model) =>
{
    var category = categories.FirstOrDefault(c => c.Id == id);
    if (category is null) return Results.NotFound($"A categoria com id {id} não foi encontrada");

    category.Title = model.Title;
    category.Description = model.Description;

    return Results.Ok(category);
});
app.MapDelete("/v1/categories/{id:int}", (int id) =>
{
    var category = categories.FirstOrDefault(c => c.Id == id);
    if (category is null) return Results.NotFound($"A categoria com id {id} não foi encontrada");

    categories.Remove(category);

    return Results.Ok();
});

app.UseCors(allowSpecifications);

app.Run();

class Category(int id, string title, string description)
{
    public int Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
}
