using Microsoft.Extensions.VectorData;

namespace AiChatBuddy.Api.Models;

public class VectorChunk
{
    public const int VectorDimension = 384;
    public const string VectorDistanceFunction = DistanceFunction.CosineDistance;
    [VectorStoreKey]
    public required Guid Key { get; set; }
    [VectorStoreData]
    public required string Content { get; set; }
    [VectorStoreData]
    public string? Context { get; set; }
    [VectorStoreData]
    public required string DocumentId { get; set; }
    [VectorStoreVector(VectorDimension, DistanceFunction = VectorDistanceFunction)]
    public string Embedding { get; }
}