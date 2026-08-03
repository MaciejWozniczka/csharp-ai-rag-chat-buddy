using System.Diagnostics;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DataIngestion;
using Microsoft.Extensions.DataIngestion.Chunkers;
using Microsoft.Extensions.VectorData;
using Microsoft.ML.Tokenizers;

namespace AiChatBuddy.IngestionService;

public class Worker(ILoggerFactory loggerFactory, ILogger<Worker> logger,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, VectorStore vectorStore) : BackgroundService
{
    const string trackingFilePath = "tracking.txt";
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var datasetDirectory = new DirectoryInfo("Dataset");

        await File.Create(trackingFilePath).DisposeAsync();

        while (!cancellationToken.IsCancellationRequested)
        {
            var processedFiles = (await File.ReadAllLinesAsync(trackingFilePath, cancellationToken)).ToHashSet();

            var filesToProcess = datasetDirectory
                .EnumerateFiles("*.md")
                .Where(f => !processedFiles.Contains(f.Name));

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                logger.LogInformation("Files to process: {files}", string.Join(", ", filesToProcess.Select(f => f.Name)));
            }

            using var vectorStoreWriter = new VectorStoreWriter<string>(vectorStore, 384, new VectorStoreWriterOptions()
            {
                CollectionName = "incidents-chunks",
                DistanceFunction = DistanceFunction.CosineDistance,
                IncrementalIngestion = false
            });

            var pipeline = new IngestionPipeline<string>(
                reader: new IngestionReader(),
                chunker: new SemanticSimilarityChunker(embeddingGenerator, new IngestionChunkerOptions(TiktokenTokenizer.CreateForModel("gpt-4o"))),
                writer: vectorStoreWriter,
                loggerFactory: loggerFactory);

            await foreach(var result in pipeline.ProcessAsync(filesToProcess, cancellationToken))
            {
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to process file: {file}", result.DocumentId);
                }
            }

            await File.AppendAllLinesAsync(trackingFilePath, filesToProcess.Select(f => f.Name), cancellationToken);

            await Task.Delay(1000, cancellationToken);
        }
    }

    public class IngestionReader : IngestionDocumentReader
    {
        private readonly MarkdownReader _reader = new MarkdownReader();
        public override Task<IngestionDocument> ReadAsync(Stream source, string identifier, string mediaType, CancellationToken cancellationToken = default)
        {
            Debug.WriteLine(identifier);
            Debug.WriteLine(mediaType);

            return _reader.ReadAsync(source, identifier, mediaType, cancellationToken);
        }
    }
}
