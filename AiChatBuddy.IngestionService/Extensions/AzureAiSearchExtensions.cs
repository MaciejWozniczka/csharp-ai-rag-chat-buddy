using System;
using System.Collections.Generic;
using System.Text;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;

namespace AiChatBuddy.IngestionService.Extensions;

public static class AzureAiSearchExtensions
{
    public static IServiceCollection AddAzureAiSearchVectorStore(this IServiceCollection services)
    {
        services.AddSingleton<VectorStore>( sp =>
        {
            var indexClient = sp.GetRequiredService<SearchIndexClient>();
            return new AzureAISearchVectorStore(indexClient);
        });

        return services;
    }
}