using CsvHelper.Configuration;
using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public sealed class VocabularyEntryMap : ClassMap<VocabularyEntry>
{
    public VocabularyEntryMap()
    {
        Map(entry => entry.Word).Name("word");
        Map(entry => entry.Translation).Name("translation");
        Map(entry => entry.Explanation).Name("explanation");
        Map(entry => entry.Example).Name("example");
        Map(entry => entry.PartOfSpeech).Name("type", "partOfSpeech");
    }
}
