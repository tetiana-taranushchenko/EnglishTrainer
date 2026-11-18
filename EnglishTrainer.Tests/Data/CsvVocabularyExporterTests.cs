using System.Text;

namespace EnglishTrainer.Tests.Data;

public class CsvVocabularyExporterTests
{
    private readonly CsvVocabularyExporter _exporter = new();
    private readonly CsvVocabularyImporter _importer = new();

    [Fact]
    public void Write_ProducesHeaderRowWithExpectedColumns()
    {
        var csv = _exporter.Write(new[]
        {
            new VocabularyEntry("cat", "кіт", "", "", "noun")
        });

        var firstLine = csv.Split('\n')[0].Trim();
        firstLine.Should().Be("word,translation,explanation,example,type");
    }

    [Fact]
    public void Write_SerializesAllEntries()
    {
        var entries = new[]
        {
            new VocabularyEntry("cat", "кіт", "small animal", "The cat sleeps.", "noun"),
            new VocabularyEntry("run", "бігти", "move fast", "They run.", "verb"),
        };

        var csv = _exporter.Write(entries);

        csv.Should().Contain("cat").And.Contain("кіт").And.Contain("run").And.Contain("бігти");
    }

    [Fact]
    public void Write_Then_Read_RoundTripsEntries()
    {
        var original = new[]
        {
            new VocabularyEntry("cat", "кіт", "small animal", "The cat sleeps.", "noun"),
            new VocabularyEntry("dog", "собака", "domestic", "The dog barks.", "noun"),
            new VocabularyEntry("house", "дім", "place", "A house.", "noun"),
            new VocabularyEntry("book", "книга", "reading", "A book.", "noun"),
        };

        var csv = _exporter.Write(original);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var restored = _importer.Read(stream);

        restored.Should().BeEquivalentTo(original);
    }
}
