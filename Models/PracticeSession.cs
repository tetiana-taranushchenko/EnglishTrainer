namespace EnglishTrainer.Models;

public class PracticeSession
{
    private readonly PracticeQuestion[] _questions;
    private int _currentQuestionIndex;
    public PracticeQuestion CurrentQuestion => _questions[_currentQuestionIndex];
    private readonly List<PracticeAnswer> _answers = new();
    private readonly IReadOnlyList<PracticeAnswer> _answersView;
    public int TotalQuestions => _questions.Length;
    public int CorrectCount => _answers.Count(answer => answer.IsCorrect);
    public int IncorrectCount => _answers.Count - CorrectCount;
    public IReadOnlyList<PracticeAnswer> Answers => _answersView;

    public PracticeSession(PracticeQuestion[] questions)
    {
        ArgumentNullException.ThrowIfNull(questions);

        if (questions.Length == 0)
        {
            throw new ArgumentException("Practice session requires at least one question.", nameof(questions));
        }

        _questions = questions.ToArray();
        _answersView = _answers.AsReadOnly();
    }

    public bool IsCorrectAnswer(string answer)
    {
        return CurrentQuestion.CorrectAnswer == answer;
    }

    public bool MoveNext()
    {
        if (_currentQuestionIndex < _questions.Length - 1)
        {
            _currentQuestionIndex++;
            return true;
        }
        return false;
    }

    public bool SubmitAnswer(string selectedAnswer)
    {
        var isCorrect = IsCorrectAnswer(selectedAnswer);
        var answer = new PracticeAnswer(CurrentQuestion, selectedAnswer, isCorrect);
        _answers.Add(answer);
        return isCorrect;
    }
}
