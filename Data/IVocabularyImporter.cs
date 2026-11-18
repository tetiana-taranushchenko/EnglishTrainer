using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public interface IVocabularyImporter
{
    long MaxSizeBytes { get; }
    List<VocabularyEntry> Read(Stream stream);
}
