using System.Text;

namespace EnglishTrainer.Tests.Data;

public class ImportPipelineTests
{
    private static Stream StreamFrom(string csv, bool prependBom = false)
    {
        var bytes = Encoding.UTF8.GetBytes(csv);
        if (!prependBom)
        {
            return new MemoryStream(bytes);
        }

        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var combined = new byte[bom.Length + bytes.Length];
        Buffer.BlockCopy(bom, 0, combined, 0, bom.Length);
        Buffer.BlockCopy(bytes, 0, combined, bom.Length, bytes.Length);
        return new MemoryStream(combined);
    }

    private const string SampleCsv = """
        word,translation,explanation,example,type
        circumstance,обставина,A fact or condition that affects a situation.,A change in the schedule was the only circumstance that delayed the release.,noun
        contemplate,обмірковувати,To think carefully about a possible action.,The team began to contemplate a complete rewrite of the legacy code.,verb
        reliable,надійний,Consistently trustworthy or dependable.,A reliable deployment pipeline reduces release stress.,adjective
        thorough,ретельний,Complete with regard to every detail.,She gave the design document a thorough review.,adjective
        """;

    [Fact]
    public void FullPipeline_FromCsv_BuildsPlayableSessionThatCompletesCorrectly()
    {
        var importer = new CsvVocabularyImporter();
        var factory = new PracticeQuestionFactory();
        var controller = new PracticeController(factory);

        var entries = importer.Read(StreamFrom(SampleCsv));
        controller.LoadEntries(entries, PracticeDirection.UkrainianToEnglish);

        controller.Session.TotalQuestions.Should().Be(entries.Count);
        controller.HasImportedQuestions.Should().BeTrue();

        for (int i = 0; i < entries.Count; i++)
        {
            var current = controller.Session.CurrentQuestion;
            current.Options.Should().Contain(current.CorrectAnswer);
            current.Options.Should().OnlyHaveUniqueItems();

            controller.SelectAnswer(current.CorrectAnswer);
            controller.IsCorrect.Should().BeTrue();

            controller.GoToNextQuestion();
        }

        controller.IsComplete.Should().BeTrue();
        controller.Session.CorrectCount.Should().Be(entries.Count);
        controller.Session.IncorrectCount.Should().Be(0);
    }

    [Fact]
    public void Importer_AcceptsUtf8BomAtStartOfFile()
    {
        var importer = new CsvVocabularyImporter();

        var entries = importer.Read(StreamFrom(SampleCsv, prependBom: true));

        entries.Should().HaveCount(4);
        entries[0].Word.Should().Be("circumstance");
    }
}
