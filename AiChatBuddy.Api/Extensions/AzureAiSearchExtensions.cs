// Cały plik jest kompilowany tylko w wariancie Azure — bez pakietu konektora
// (patrz UseAzure w Directory.Build.props) użyte tu typy nie istnieją.
#if USE_AZURE
using AiChatBuddy.Api.Models;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;

namespace AiChatBuddy.Api.Extensions;

public static class AzureAiSearchExtensions
{
    public static IServiceCollection AddAzureSearchCollection(this IServiceCollection services, string name)
    {
        services.AddSingleton<VectorStoreCollection<string, VectorChunk>>(sp =>
        {
            var indexClient = sp.GetRequiredService<SearchIndexClient>();

            return new AzureAISearchCollection<string, VectorChunk>(indexClient, name, new AzureAISearchCollectionOptions()
            {
                EmbeddingGenerator = sp.GetRequiredService<IEmbeddingGenerator>()
            });
        });

        return services;
    }
}
#endif