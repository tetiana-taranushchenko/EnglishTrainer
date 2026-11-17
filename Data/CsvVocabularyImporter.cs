using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public class CsvVocabularyImporter
{
    public const long MaxSizeBytes = 5 * 1024 * 1024;
    public const int MinEntryCount = 4;

    public List<VocabularyEntry> Read(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<VocabularyEntryMap>();

        List<VocabularyEntry> entries;
        try
        {
            entries = csv.GetRecords<VocabularyEntry>().ToList();
        }
        catch (HeaderValidationException ex)
        {
            throw new InvalidDataException(
                "CSV is missing required columns (word, translation, explanation, example, type).", ex);
        }
        catch (CsvHelperException ex)
        {
            throw new InvalidDataException("CSV file is malformed.", ex);
        }

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (string.IsNullOrWhiteSpace(entry.Word) || string.IsNullOrWhiteSpace(entry.Translation))
            {
                throw new InvalidDataException(
                    $"Row {i + 2} is missing the word or translation.");
            }
        }

        if (entries.Count == 0)
        {
            throw new InvalidDataException("CSV file is empty.");
        }

        if (entries.Count < MinEntryCount)
        {
            throw new InvalidDataException(
                $"Word list needs at least {MinEntryCount} words to generate questions.");
        }

        return entries;
    }
}
