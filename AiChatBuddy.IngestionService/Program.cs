using AiChatBuddy.IngestionService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder
    .AddOllamaApiClient("embedding")
    .AddEmbeddingGenerator();

var vectorStoreConnectionString = builder.Configuration.GetConnectionString("vector-store");

builder.Services.AddSqliteVectorStore(_=> vectorStoreConnectionString ?? throw new InvalidOperationException("Vector store connection string is not configured"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
