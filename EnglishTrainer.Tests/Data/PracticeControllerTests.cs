using EnglishTrainer.Tests.TestData;

namespace EnglishTrainer.Tests.Data;

public class PracticeControllerTests
{
    private readonly PracticeQuestionFactory _factory = new();

    private PracticeController NewController() => new(_factory);

    [Fact]
    public void Constructor_StartsWithStarterSessionInUkrainianToEnglish()
    {
        var controller = NewController();

        controller.Direction.Should().Be(PracticeDirection.UkrainianToEnglish);
        controller.HasImportedQuestions.Should().BeFalse();
        controller.IsComplete.Should().BeFalse();
        controller.IsCorrect.Should().BeNull();
        controller.Session.Answers.Should().BeEmpty();
        controller.CanSwitchDirection.Should().BeTrue();
    }

    [Fact]
    public void SelectAnswer_RecordsAnswerAndUpdatesCorrectness()
    {
        var controller = NewController();
        var correct = controller.Session.CurrentQuestion.CorrectAnswer;

        controller.SelectAnswer(correct);

        controller.IsCorrect.Should().BeTrue();
        controller.SelectedAnswer.Should().Be(correct);
        controller.Session.Answers.Should().HaveCount(1);
    }

    [Fact]
    public void SelectAnswer_IgnoresSecondAttemptForSameQuestion()
    {
        var controller = NewController();
        var current = controller.Session.CurrentQuestion;
        var wrong = current.Options.First(opt => opt != current.CorrectAnswer);

        controller.SelectAnswer(wrong);
        controller.SelectAnswer(current.CorrectAnswer);

        controller.SelectedAnswer.Should().Be(wrong);
        controller.IsCorrect.Should().BeFalse();
        controller.Session.Answers.Should().HaveCount(1);
    }

    [Fact]
    public void GoToNextQuestion_ClearsPerQuestionState_WhenMoreQuestionsRemain()
    {
        var controller = NewController();
        controller.SelectAnswer(controller.Session.CurrentQuestion.CorrectAnswer);

        controller.GoToNextQuestion();

        controller.IsCorrect.Should().BeNull();
        controller.SelectedAnswer.Should().BeNull();
        controller.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void GoToNextQuestion_MarksComplete_WhenNoQuestionsRemain()
    {
        var controller = NewController();
        var total = controller.Session.TotalQuestions;

        for (int i = 0; i < total; i++)
        {
            controller.SelectAnswer(controller.Session.CurrentQuestion.CorrectAnswer);
            controller.GoToNextQuestion();
        }

        controller.IsComplete.Should().BeTrue();
    }

    [Fact]
    public void CanSwitchDirection_IsFalse_AfterFirstAnswerSubmitted()
    {
        var controller = NewController();
        controller.SelectAnswer(controller.Session.CurrentQuestion.CorrectAnswer);

        controller.CanSwitchDirection.Should().BeFalse();
    }

    [Fact]
    public void SwitchDirection_FromFreshStarter_FlipsDirectionAndResetsSession()
    {
        var controller = NewController();

        controller.SwitchDirection();

        controller.Direction.Should().Be(PracticeDirection.EnglishToUkrainian);
        controller.Session.Answers.Should().BeEmpty();
        controller.Session.CurrentQuestion.PromptLanguage.Should().Be("en");
    }

    [Fact]
    public void SwitchDirection_DoesNothing_WhenAnswersAlreadySubmitted()
    {
        var controller = NewController();
        var originalDirection = controller.Direction;
        controller.SelectAnswer(controller.Session.CurrentQuestion.CorrectAnswer);

        controller.SwitchDirection();

        controller.Direction.Should().Be(originalDirection);
    }

    [Fact]
    public void SwitchDirection_WithImportedEntries_RebuildsQuestionsForNewDirection()
    {
        var controller = NewController();
        var entries = VocabularyBuilder.Nouns(6);
        controller.LoadEntries(entries, PracticeDirection.UkrainianToEnglish);

        controller.SwitchDirection();

        controller.Direction.Should().Be(PracticeDirection.EnglishToUkrainian);
        controller.HasImportedQuestions.Should().BeTrue();
        controller.Session.CurrentQuestion.PromptLanguage.Should().Be("en");
        controller.Session.TotalQuestions.Should().Be(entries.Count);
    }

    [Fact]
    public void Restart_WithoutImport_RebuildsStarterSessionForCurrentDirection()
    {
        var controller = NewController();
        controller.SwitchDirection();
        controller.SelectAnswer(controller.Session.CurrentQuestion.CorrectAnswer);
        controller.GoToNextQuestion();

        controller.Restart();

        controller.Direction.Should().Be(PracticeDirection.EnglishToUkrainian);
        controller.Session.Answers.Should().BeEmpty();
        controller.IsComplete.Should().BeFalse();
        controller.IsCorrect.Should().BeNull();
        controller.SelectedAnswer.Should().BeNull();
    }

    [Fact]
    public void Restart_AfterImport_RebuildsFromImportedQuestions()
    {
        var controller = NewController();
        var entries = VocabularyBuilder.Nouns(5);
        controller.LoadEntries(entries, PracticeDirection.UkrainianToEnglish);
        controller.SelectAnswer(controller.Session.CurrentQuestion.CorrectAnswer);

        controller.Restart();

        controller.Session.Answers.Should().BeEmpty();
        controller.Session.TotalQuestions.Should().Be(entries.Count);
        controller.HasImportedQuestions.Should().BeTrue();
    }

    [Fact]
    public void LoadEntries_ReplacesSessionAndClearsTransientState()
    {
        var controller = NewController();
        controller.SelectAnswer(controller.Session.CurrentQuestion.CorrectAnswer);
        var entries = VocabularyBuilder.Nouns(5);

        controller.LoadEntries(entries, PracticeDirection.EnglishToUkrainian);

        controller.IsCorrect.Should().BeNull();
        controller.SelectedAnswer.Should().BeNull();
        controller.IsComplete.Should().BeFalse();
        controller.Direction.Should().Be(PracticeDirection.EnglishToUkrainian);
        controller.Session.Answers.Should().BeEmpty();
        controller.Session.TotalQuestions.Should().Be(entries.Count);
    }
}
