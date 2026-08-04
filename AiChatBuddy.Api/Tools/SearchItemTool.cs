using AiChatBuddy.Api.Models;
using Microsoft.Extensions.VectorData;
using System.ComponentModel;

namespace AiChatBuddy.Api.Tools;

public class SearchItemTool(VectorStoreCollection<string, VectorChunk> vectorCollection)
{
    [Description("Searches for items based on the provided query.")]
    public async Task<IEnumerable<string>> SearchItemAsync([Description("The query to search for.")] string searchQuery, CancellationToken cancellationToken)
    {
        var searchResults = vectorCollection
            .SearchAsync(searchQuery, top: 5, cancellationToken: cancellationToken);

        return await searchResults
            .Select(r => r.Record.Content)
            .ToListAsync(cancellationToken);
    }
}