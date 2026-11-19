using EnglishTrainer.Tests.TestData;

namespace EnglishTrainer.Tests.Data;

public class PracticeQuestionFactoryTests
{
    private readonly PracticeQuestionFactory _factory = new();

    [Fact]
    public void Create_UkrainianToEnglish_UsesTranslationAsPromptAndWordAsAnswer()
    {
        var entry = VocabularyBuilder.Entry("cat", "кіт");
        var pool = VocabularyBuilder.Nouns(5);
        pool.Add(entry);

        var question = _factory.Create(entry, pool, PracticeDirection.UkrainianToEnglish);

        question.PromptText.Should().Be("кіт");
        question.CorrectAnswer.Should().Be("cat");
        question.PromptLanguage.Should().Be("uk");
        question.OptionsLanguage.Should().Be("en");
    }

    [Fact]
    public void Create_EnglishToUkrainian_UsesWordAsPromptAndTranslationAsAnswer()
    {
        var entry = VocabularyBuilder.Entry("cat", "кіт");
        var pool = VocabularyBuilder.Nouns(5);
        pool.Add(entry);

        var question = _factory.Create(entry, pool, PracticeDirection.EnglishToUkrainian);

        question.PromptText.Should().Be("cat");
        question.CorrectAnswer.Should().Be("кіт");
        question.PromptLanguage.Should().Be("en");
        question.OptionsLanguage.Should().Be("uk");
    }

    [Fact]
    public void Create_ProducesExactlyFourUniqueOptionsContainingCorrectAnswer()
    {
        var entry = VocabularyBuilder.Entry("cat", "кіт");
        var pool = VocabularyBuilder.Nouns(10);
        pool.Add(entry);

        var question = _factory.Create(entry, pool);

        question.Options.Should().HaveCount(PracticeQuestionFactory.OptionsPerQuestion);
        question.Options.Should().OnlyHaveUniqueItems();
        question.Options.Should().Contain(question.CorrectAnswer);
    }

    [Fact]
    public void Create_PopulatesMetadataFromEntry()
    {
        var entry = new VocabularyEntry("cat", "кіт", "a small animal", "The cat sleeps.", "noun");
        var pool = VocabularyBuilder.Nouns(5);
        pool.Add(entry);

        var question = _factory.Create(entry, pool);

        question.Explanation.Should().Be("a small animal");
        question.Example.Should().Be("The cat sleeps.");
        question.EnglishWord.Should().Be("cat");
        question.PartOfSpeech.Should().Be("noun");
    }

    [Fact]
    public void Create_PrefersDistractorsWithSamePartOfSpeech()
    {
        var entry = VocabularyBuilder.Entry("cat", "кіт", partOfSpeech: "noun");
        var nouns = new List<VocabularyEntry>
        {
            entry,
            VocabularyBuilder.Entry("dog", "собака", "noun"),
            VocabularyBuilder.Entry("house", "дім", "noun"),
            VocabularyBuilder.Entry("book", "книга", "noun"),
        };
        var verbs = new List<VocabularyEntry>
        {
            VocabularyBuilder.Entry("run", "бігти", "verb"),
            VocabularyBuilder.Entry("jump", "стрибати", "verb"),
        };
        var pool = nouns.Concat(verbs).ToList();

        var question = _factory.Create(entry, pool);

        question.Options.Should().BeEquivalentTo(new[] { "cat", "dog", "house", "book" });
    }

    [Fact]
    public void Create_FallsBackToOtherPartsOfSpeech_WhenSameTypeIsInsufficient()
    {
        var entry = VocabularyBuilder.Entry("cat", "кіт", partOfSpeech: "noun");
        var pool = new List<VocabularyEntry>
        {
            entry,
            VocabularyBuilder.Entry("dog", "собака", "noun"),
            VocabularyBuilder.Entry("run", "бігти", "verb"),
            VocabularyBuilder.Entry("jump", "стрибати", "verb"),
        };

        var question = _factory.Create(entry, pool);

        question.Options.Should().HaveCount(4).And.OnlyHaveUniqueItems();
        question.Options.Should().Contain("cat");
    }

    [Fact]
    public void Create_Throws_WhenNotEnoughDistinctAnswersExist()
    {
        var entry = VocabularyBuilder.Entry("cat", "кіт");
        var pool = new List<VocabularyEntry> { entry, VocabularyBuilder.Entry("dog", "собака") };

        var act = () => _factory.Create(entry, pool);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*distinct answers*");
    }

    [Theory]
    [InlineData(PracticeDirection.UkrainianToEnglish)]
    [InlineData(PracticeDirection.EnglishToUkrainian)]
    public void Create_PreservesEntryIdentity_ForReviewExportRoundTrip(PracticeDirection direction)
    {
        var entry = new VocabularyEntry("cat", "кіт", "a small animal", "The cat sleeps.", "noun");
        var pool = VocabularyBuilder.Nouns(5);
        pool.Add(entry);

        var question = _factory.Create(entry, pool, direction);

        var promptIsUkrainian = string.Equals(
            question.PromptLanguage, "uk", StringComparison.OrdinalIgnoreCase);
        var word = question.EnglishWord
            ?? (promptIsUkrainian ? question.CorrectAnswer : question.PromptText);
        var translation = promptIsUkrainian ? question.PromptText : question.CorrectAnswer;

        word.Should().Be(entry.Word);
        translation.Should().Be(entry.Translation);
    }

    [Fact]
    public void Create_DeduplicatesCandidates_WhenPoolContainsRepeatedWords()
    {
        var entry = VocabularyBuilder.Entry("cat", "кіт");
        var pool = new List<VocabularyEntry>
        {
            entry,
            VocabularyBuilder.Entry("dog", "собака"),
            VocabularyBuilder.Entry("dog", "собака"),
            VocabularyBuilder.Entry("house", "дім"),
            VocabularyBuilder.Entry("book", "книга"),
        };

        var question = _factory.Create(entry, pool);

        question.Options.Should().OnlyHaveUniqueItems();
    }
}
