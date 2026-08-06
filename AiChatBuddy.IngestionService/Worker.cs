using Microsoft.Extensions.AI;
using Microsoft.Extensions.DataIngestion;
using Microsoft.Extensions.DataIngestion.Chunkers;
using Microsoft.Extensions.VectorData;
using Microsoft.ML.Tokenizers;

namespace AiChatBuddy.IngestionService;

/// <summary>
/// Usługa w tle, która cyklicznie skanuje katalog Dataset, dzieli pliki markdown
/// na fragmenty i zapisuje je wraz z embeddingami do bazy wektorowej.
/// </summary>
public class Worker(ILoggerFactory loggerFactory, ILogger<Worker> logger, IConfiguration configuration,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, VectorStore vectorStore) : BackgroundService
{
    // Plik z nazwami już przetworzonych dokumentów — zapobiega ponownej ingestii w kolejnych iteracjach.
    const string trackingFilePath = "tracking.txt";

    // Kolekcja czytana przez API. Nazwa musi być identyczna po obu stronach.
    const string collectionName = "incidents-chunks";

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Katalog z dokumentami incydentów, kopiowany do katalogu wyjściowego przy budowaniu.
        var datasetDirectory = new DirectoryInfo("Dataset");

        // Czyszczenie pliku śledzącego przy każdym starcie — po restarcie usługi
        // wszystkie dokumenty są traktowane jako nieprzetworzone i wgrywane od nowa.
        await File.Create(trackingFilePath).DisposeAsync();

        // Kolekcja jest usuwana przed ingestią, bo VectorStoreWriter nadaje chunkom losowe klucze:
        // bez tego każdy start usługi dopisywałby kolejne kopie tych samych fragmentów, a wyszukiwanie
        // zwracałoby top-N duplikatów jednego chunku zamiast N różnych fragmentów.
        await vectorStore.EnsureCollectionDeletedAsync(collectionName, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var processedFiles = (await File.ReadAllLinesAsync(trackingFilePath, cancellationToken)).ToHashSet();

            // Zapytanie LINQ jest leniwe — kolejne użycia poniżej ponownie przeglądają katalog.
            var filesToProcess = datasetDirectory
                .EnumerateFiles("*.md")
                .Where(f => !processedFiles.Contains(f.Name));

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                logger.LogInformation("Files to process: {files}", string.Join(", ", filesToProcess.Select(f => f.Name)));
            }

            // Zapis do kolekcji czytanej przez API. Wymiar i miara odległości muszą być
            // identyczne jak w modelu VectorChunk, inaczej wyszukiwanie nie zadziała.
            using var vectorStoreWriter = new VectorStoreWriter<string>(vectorStore, 384, new VectorStoreWriterOptions()
            {
                CollectionName = collectionName,
                DistanceFunction = configuration.GetValue<string>("vectorFunction"),
                IncrementalIngestion = false
            });

            // Pipeline: odczyt markdownu → podział na fragmenty → zapis do bazy wektorowej.
            var pipeline = new IngestionPipeline<string>(
                reader: new IngestionReader(),
                // Chunker semantyczny grupuje sąsiadujące zdania o podobnym znaczeniu (na bazie
                // embeddingów), a tokenizer pilnuje limitu rozmiaru pojedynczego fragmentu.
                chunker: new SemanticSimilarityChunker(embeddingGenerator, new IngestionChunkerOptions(TiktokenTokenizer.CreateForModel("gpt-4o"))),
                writer: vectorStoreWriter,
                loggerFactory: loggerFactory);

            // Przetwarzanie jest strumieniowe — wynik pojawia się po każdym dokumencie.
            await foreach(var result in pipeline.ProcessAsync(filesToProcess, cancellationToken))
            {
                // Błąd jednego dokumentu nie przerywa całej ingestii, tylko trafia do logów.
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to process file: {file}", result.DocumentId);
                }
            }

            // Oznaczamy dokumenty jako przetworzone, by nie wgrywać ich w następnym obiegu pętli.
            await File.AppendAllLinesAsync(trackingFilePath, filesToProcess.Select(f => f.Name), cancellationToken);

            // Krótka pauza — pętla działa dalej, aby wychwycić pliki dodane do katalogu w trakcie pracy.
            await Task.Delay(1000, cancellationToken);
        }
    }

    /// <summary>
    /// Reader delegujący odczyt do wbudowanego czytnika markdownu — pipeline wymaga
    /// typu pochodnego od <see cref="IngestionDocumentReader"/>.
    /// </summary>
    public class IngestionReader : IngestionDocumentReader
    {
        private readonly MarkdownReader _reader = new MarkdownReader();

        public override Task<IngestionDocument> ReadAsync(Stream source, string identifier, string mediaType, CancellationToken cancellationToken = default)
        {
            return _reader.ReadAsync(source, identifier, mediaType, cancellationToken);
        }
    }
}
