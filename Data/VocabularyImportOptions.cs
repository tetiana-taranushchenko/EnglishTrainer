namespace EnglishTrainer.Data;

public class VocabularyImportOptions
{
    public const string SectionName = "VocabularyImport";

    public long MaxSizeBytes { get; set; } = 5 * 1024 * 1024;
}
