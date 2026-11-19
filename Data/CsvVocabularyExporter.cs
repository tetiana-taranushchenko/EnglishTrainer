using System.Globalization;
using CsvHelper;
using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public static class CsvVocabularyExporter
{
    public static string Write(IEnumerable<VocabularyEntry> entries)
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<VocabularyEntryMap>();

        csv.WriteRecords(entries);

        return writer.ToString();
    }
}
