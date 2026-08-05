using Microsoft.Extensions.VectorData;

namespace AiChatBuddy.Api.Models;

/// <summary>
/// Pojedynczy fragment (chunk) dokumentu incydentu zapisany w bazie wektorowej.
/// Atrybuty opisują mapowanie właściwości na kolumny kolekcji wektorowej.
/// </summary>
public class VectorChunk
{
    // Liczba wymiarów wektora — musi odpowiadać modelowi embeddingów (all-minilm = 384)
    // oraz wartości ustawionej we VectorStoreWriter w IngestionService.
    public const int VectorDimension = 384;

    // Miara podobieństwa używana przy wyszukiwaniu; ta sama musi być użyta przy zapisie.
    public const string VectorDistanceFunction = DistanceFunction.CosineDistance;

    // Klucz główny rekordu w kolekcji.
    [VectorStoreKey]
    public required string Key { get; set; }

    // Treść fragmentu — to ona jest zwracana modelowi jako kontekst odpowiedzi.
    [VectorStoreData]
    public required string Content { get; set; }

    // Opcjonalny kontekst fragmentu (np. nagłówek sekcji, z której pochodzi).
    [VectorStoreData]
    public string? Context { get; set; }

    // Identyfikator dokumentu źródłowego, pozwala powiązać chunk z konkretnym incydentem.
    [VectorStoreData]
    public required string DocumentId { get; set; }

    // Wektor embeddingu. Tylko do odczytu — wyliczany i zapisywany przez warstwę
    // vector store na podstawie właściwości Content, nie ustawiamy go ręcznie.
    [VectorStoreVector(VectorDimension, DistanceFunction = VectorDistanceFunction)]
    public float[]? Embedding { get; }
}