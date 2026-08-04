#pragma warning disable EXTEXP0001
using AiChatBuddy.Api.Models;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

string sqlConnectionString = builder.Configuration.GetConnectionString("vector-store")
    ?? throw new InvalidOperationException("Vector store connection string is not configured");

builder.Services
    .AddSqliteCollection<string, VectorChunk>("incidents-chunks", sqlConnectionString);

builder
    .AddOllamaApiClient("chat")
    .AddChatClient()
    .UseFunctionInvocation();

builder
    .AddOllamaApiClient("embedding")
    .AddEmbeddingGenerator();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOllamaResilienceHandlers();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
        options.DocumentPath = "/openapi/v1.json");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
