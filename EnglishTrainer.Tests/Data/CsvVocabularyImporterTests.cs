using System.Text;

namespace EnglishTrainer.Tests.Data;

public class CsvVocabularyImporterTests
{
    private readonly CsvVocabularyImporter _importer = new();

    private static Stream StreamFrom(string csv) => new MemoryStream(Encoding.UTF8.GetBytes(csv));

    private const string ValidHeader = "word,translation,explanation,example,type";

    [Fact]
    public void Read_ParsesAllColumnsCorrectly()
    {
        var csv = $"""
            {ValidHeader}
            cat,кіт,small animal,The cat sleeps.,noun
            dog,собака,domestic animal,The dog barks.,noun
            house,дім,place to live,A big house.,noun
            book,книга,reading,A good book.,noun
            """;

        var entries = _importer.Read(StreamFrom(csv));

        entries.Should().HaveCount(4);
        entries[0].Should().BeEquivalentTo(new
        {
            Word = "cat",
            Translation = "кіт",
            Explanation = "small animal",
            Example = "The cat sleeps.",
            PartOfSpeech = "noun"
        });
    }

    [Fact]
    public void Read_SupportsPartOfSpeechColumnAlias()
    {
        var csv = """
            word,translation,explanation,example,partOfSpeech
            cat,кіт,,,noun
            dog,собака,,,noun
            house,дім,,,noun
            book,книга,,,noun
            """;

        var entries = _importer.Read(StreamFrom(csv));

        entries.Should().AllSatisfy(e => e.PartOfSpeech.Should().Be("noun"));
    }

    [Fact]
    public void Read_TrimsWhitespaceFromFields()
    {
        var csv = $"""
            {ValidHeader}
              cat  ,  кіт  ,  small animal  ,  ex  ,  noun
            dog,собака,,,noun
            house,дім,,,noun
            book,книга,,,noun
            """;

        var entries = _importer.Read(StreamFrom(csv));

        entries[0].Word.Should().Be("cat");
        entries[0].Translation.Should().Be("кіт");
    }

    [Fact]
    public void Read_Throws_WhenCsvIsEmpty()
    {
        var act = () => _importer.Read(StreamFrom(""));

        act.Should().Throw<InvalidDataException>();
    }

    [Fact]
    public void Read_Throws_WhenHeaderIsMissingRequiredColumns()
    {
        var csv = "word,translation\ncat,кіт";

        var act = () => _importer.Read(StreamFrom(csv));

        act.Should().Throw<InvalidDataException>()
            .WithMessage("*missing required columns*");
    }

    [Fact]
    public void Read_Throws_WhenRowHasEmptyWord()
    {
        var csv = $"""
            {ValidHeader}
            ,кіт,,,noun
            dog,собака,,,noun
            house,дім,,,noun
            book,книга,,,noun
            """;

        var act = () => _importer.Read(StreamFrom(csv));

        act.Should().Throw<InvalidDataException>()
            .WithMessage("*Row 2*missing the word or translation*");
    }

    [Fact]
    public void Read_Throws_WhenRowHasEmptyTranslation()
    {
        var csv = $"""
            {ValidHeader}
            cat,,,,noun
            dog,собака,,,noun
            house,дім,,,noun
            book,книга,,,noun
            """;

        var act = () => _importer.Read(StreamFrom(csv));

        act.Should().Throw<InvalidDataException>()
            .WithMessage("*Row 2*");
    }

    [Fact]
    public void Read_Throws_WhenFewerThanMinimumEntries()
    {
        var csv = $"""
            {ValidHeader}
            cat,кіт,,,noun
            dog,собака,,,noun
            """;

        var act = () => _importer.Read(StreamFrom(csv));

        act.Should().Throw<InvalidDataException>()
            .WithMessage($"*at least {CsvVocabularyImporter.MinEntryCount} words*");
    }
}
