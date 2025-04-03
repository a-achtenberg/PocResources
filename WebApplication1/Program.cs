using Management.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Revit;
using Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Minha API", Version = "v1" });

    // Configurar enum como string no Swagger
    options.MapType<Brand>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(Brand)).Select(name => new OpenApiString(name)).ToList<IOpenApiAny>()
    });
    
    options.MapType<GeneralPlatForm>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(GeneralPlatForm)).Select(name => new OpenApiString(name)).ToList<IOpenApiAny>()
    });
});
builder.Services.AddSingleton<ManagementResourceManager>();
builder.Services.AddSingleton<RevitResourceManager>();
builder.Services.AddScoped<IManagementResourceProvider, ManagementProvider>();
builder.Services.AddScoped<IRevitResourceProvider, RevitResourceProvider>();
builder.Services.AddScoped<IUser, UserContext>();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MapGet("/get-message/management/{brand}/{platform}/{key}", ( [FromRoute] Brand brand,  [FromRoute] GeneralPlatForm platForm, [FromRoute] string key, [FromServices] IManagementResourceProvider resourceManagerLoader, [FromServices] IUser user) =>
{
    user.SetPlatform(platForm);
    user.SetBrand(brand);
    var result = resourceManagerLoader.GetResourceString(key);
    return Results.Ok(new {text = result});
});

app.MapGet("/get-message/revit/{brand}/{platform}/{key}", ([FromRoute] Brand brand,  [FromRoute] GeneralPlatForm platForm, [FromRoute] string key, [FromServices] IRevitResourceProvider resourceManagerLoader, [FromServices] IUser user) =>
{
    user.SetPlatform(platForm);
    user.SetBrand(brand);
    var result = resourceManagerLoader.GetResourceString(key);
    return Results.Ok(new {text = result});
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();