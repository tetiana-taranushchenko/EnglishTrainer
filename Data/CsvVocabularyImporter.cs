using EnglishTrainer.Models;
using Microsoft.VisualBasic.FileIO;

namespace EnglishTrainer.Data;

public class CsvVocabularyImporter
{
    public List<VocabularyEntry> Read(Stream stream)
    {
        using var parser = new TextFieldParser(
            stream, System.Text.Encoding.UTF8,
            detectEncoding: true, leaveOpen: true);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        var entries = new List<VocabularyEntry>();
        var header = parser.ReadFields();

        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields();

            if (fields is { Length: 5 })
            {
                var entry = new VocabularyEntry(fields[0], fields[1], fields[2], fields[3], fields[4]);
                entries.Add(entry);
            }
        }
        return entries;
    }
 }