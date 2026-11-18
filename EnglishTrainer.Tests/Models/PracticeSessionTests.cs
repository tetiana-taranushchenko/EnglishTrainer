namespace EnglishTrainer.Tests.Models;

public class PracticeSessionTests
{
    private static PracticeQuestion Question(string correct = "cat")
        => new("кіт", "noun", new[] { "cat", "dog", "house", "book" }, correct);

    [Fact]
    public void Constructor_Throws_WhenQuestionsIsNull()
    {
        var act = () => new PracticeSession(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Throws_WhenQuestionsIsEmpty()
    {
        var act = () => new PracticeSession(Array.Empty<PracticeQuestion>());

        act.Should().Throw<ArgumentException>()
            .WithMessage("*at least one question*");
    }

    [Fact]
    public void CurrentQuestion_StartsAtFirstQuestion()
    {
        var q1 = Question("cat");
        var q2 = Question("dog");
        var session = new PracticeSession(new[] { q1, q2 });

        session.CurrentQuestion.Should().BeSameAs(q1);
        session.TotalQuestions.Should().Be(2);
    }

    [Fact]
    public void IsCorrectAnswer_ReturnsTrue_ForMatchingAnswer()
    {
        var session = new PracticeSession(new[] { Question("cat") });

        session.IsCorrectAnswer("cat").Should().BeTrue();
        session.IsCorrectAnswer("dog").Should().BeFalse();
    }

    [Fact]
    public void SubmitAnswer_RecordsAnswerAndReturnsCorrectness()
    {
        var session = new PracticeSession(new[] { Question("cat") });

        var result = session.SubmitAnswer("cat");

        result.Should().BeTrue();
        session.Answers.Should().HaveCount(1);
        session.Answers[0].SelectedAnswer.Should().Be("cat");
        session.Answers[0].IsCorrect.Should().BeTrue();
    }

    [Fact]
    public void SubmitAnswer_IncorrectAnswer_IsRecordedAsIncorrect()
    {
        var session = new PracticeSession(new[] { Question("cat") });

        var result = session.SubmitAnswer("dog");

        result.Should().BeFalse();
        session.Answers[0].IsCorrect.Should().BeFalse();
    }

    [Fact]
    public void MoveNext_AdvancesToNextQuestion_AndReturnsTrue()
    {
        var q1 = Question("cat");
        var q2 = Question("dog");
        var session = new PracticeSession(new[] { q1, q2 });

        var moved = session.MoveNext();

        moved.Should().BeTrue();
        session.CurrentQuestion.Should().BeSameAs(q2);
    }

    [Fact]
    public void MoveNext_ReturnsFalse_OnLastQuestion()
    {
        var session = new PracticeSession(new[] { Question() });

        session.MoveNext().Should().BeFalse();
    }

    [Fact]
    public void CorrectCount_And_IncorrectCount_ReflectSubmittedAnswers()
    {
        var session = new PracticeSession(new[] { Question("cat"), Question("dog"), Question("house") });

        session.SubmitAnswer("cat");
        session.MoveNext();
        session.SubmitAnswer("wrong");
        session.MoveNext();
        session.SubmitAnswer("house");

        session.CorrectCount.Should().Be(2);
        session.IncorrectCount.Should().Be(1);
    }
}
