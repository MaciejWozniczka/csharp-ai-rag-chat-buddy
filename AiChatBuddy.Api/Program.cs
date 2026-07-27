var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddOllamaApiClient("chat")
    .AddChatClient();

// Add services to the container.

builder.Services.AddControllers();
#pragma warning disable EXTEXP0001
builder.Services.AddOllamaResilienceHandlers();
#pragma warning restore EXTEXP0001

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
