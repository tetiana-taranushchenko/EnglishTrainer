using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public class PracticeController
{
    private readonly PracticeQuestionFactory _factory;
    private PracticeSession _session;
    private PracticeQuestion[]? _importedQuestions;
    private List<VocabularyEntry>? _importedEntries;
    private PracticeDirection _direction;
    private bool? _isCorrect;
    private string? _selectedAnswer;
    private bool _isComplete;

    public PracticeController(PracticeQuestionFactory factory)
    {
        _factory = factory;
        _direction = PracticeDirection.UkrainianToEnglish;
        _session = new PracticeSession(StarterQuestions.Create(_direction));
    }

    public PracticeSession Session => _session;
    public PracticeDirection Direction => _direction;
    public bool? IsCorrect => _isCorrect;
    public string? SelectedAnswer => _selectedAnswer;
    public bool IsComplete => _isComplete;
    public bool HasImportedQuestions => _importedQuestions is not null;
    public bool CanSwitchDirection => _session.Answers.Count == 0;

    public void SelectAnswer(string option)
    {
        if (_isCorrect.HasValue)
        {
            return;
        }

        _selectedAnswer = option;
        _isCorrect = _session.SubmitAnswer(option);
    }

    public void GoToNextQuestion()
    {
        if (_session.MoveNext())
        {
            _isCorrect = null;
            _selectedAnswer = null;
        }
        else
        {
            _isComplete = true;
        }
    }

    public void Restart()
    {
        var questions = _importedQuestions ?? StarterQuestions.Create(_direction);
        _session = new PracticeSession(questions);
        _isComplete = false;
        _isCorrect = null;
        _selectedAnswer = null;
    }

    public void SwitchDirection()
    {
        if (!CanSwitchDirection)
        {
            return;
        }

        var newDirection = _direction == PracticeDirection.UkrainianToEnglish
            ? PracticeDirection.EnglishToUkrainian
            : PracticeDirection.UkrainianToEnglish;

        if (_importedEntries is not null)
        {
            LoadEntries(_importedEntries, newDirection);
        }
        else
        {
            _direction = newDirection;
            Restart();
        }
    }

    public void LoadEntries(List<VocabularyEntry> entries, PracticeDirection direction)
    {
        var questions = entries
            .Select(entry => _factory.Create(entry, entries, direction))
            .ToArray();

        _importedEntries = entries;
        _importedQuestions = questions;
        _direction = direction;
        _session = new PracticeSession(questions);
        _isCorrect = null;
        _selectedAnswer = null;
        _isComplete = false;
    }
}
