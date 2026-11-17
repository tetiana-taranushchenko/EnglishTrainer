using EnglishTrainer.Models;
using Microsoft.VisualBasic.FileIO;

namespace EnglishTrainer.Data;

public class CsvVocabularyImporter
{
    public const long MaxSizeBytes = 5 * 1024 * 1024;
    public const int MinEntryCount = 4;
    private const int ExpectedColumnCount = 5;

    public List<VocabularyEntry> Read(Stream stream)
    {
        using var parser = new TextFieldParser(
            stream, System.Text.Encoding.UTF8,
            detectEncoding: true, leaveOpen: true);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        var entries = new List<VocabularyEntry>();

        try
        {
            var header = parser.ReadFields();
            int rowNumber = 1;

            while (!parser.EndOfData)
            {
                rowNumber++;
                var fields = parser.ReadFields();

                if (fields is not { Length: ExpectedColumnCount })
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(fields[0]) || string.IsNullOrWhiteSpace(fields[1]))
                {
                    throw new InvalidDataException(
                        $"Row {rowNumber} is missing the word or translation.");
                }

                var entry = new VocabularyEntry(fields[0], fields[1], fields[2], fields[3], fields[4]);
                entries.Add(entry);
            }
        }
        catch (MalformedLineException ex)
        {
            throw new InvalidDataException("CSV file is malformed.", ex);
        }

        if (entries.Count == 0)
        {
            throw new InvalidDataException(
                $"CSV file is empty or malformed. Expected {ExpectedColumnCount} columns per row.");
        }

        if (entries.Count < MinEntryCount)
        {
            throw new InvalidDataException(
                $"Word list needs at least {MinEntryCount} words to generate questions.");
        }

        return entries;
    }
 }