using EnglishTrainer.Data;
using EnglishTrainer.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace EnglishTrainer.Components.Pages;

public partial class Home
{
    private PracticeSession _session = new(StarterQuestions.Create());
    private PracticeQuestion[]? _importedQuestions;
    private List<VocabularyEntry>? _importedEntries;
    private bool? _isCorrect;
    private string? _selectedAnswer;
    private bool _isComplete;
    private PracticeDirection _direction = PracticeDirection.UkrainianToEnglish;
        
    private void SelectAnswer(string option)
    {
        if (_isCorrect.HasValue)
        {
            return;
        }
        _selectedAnswer = option;
        _isCorrect = _session.SubmitAnswer(option);
    }
    
    private async Task HandleCsvSelected(IBrowserFile file)
    {
        using var browserStream = file.OpenReadStream();
        using var csvStream = new MemoryStream();

        await browserStream.CopyToAsync(csvStream);
        csvStream.Position = 0;

        var entries = new CsvVocabularyImporter().Read(csvStream);
        if (entries.Count == 0)
        {
            return;
        }

        var factory = new PracticeQuestionFactory();
        var questions = entries.Select(entry => factory.Create(entry, entries, _direction))
            .ToArray();

        _importedEntries = entries;
        _importedQuestions = questions;
        _session = new PracticeSession(questions);
        _isCorrect = null;
        _selectedAnswer = null;
        _isComplete = false;    }
        
    private void GoToNextQuestion()
    {
        if (_session.MoveNext())
        {
            _isCorrect = null;
            _selectedAnswer = null;
        } else
        {
            _isComplete = true;
        }
    }
        
    private void RestartPractice()
    {
        var questions = _importedQuestions ?? StarterQuestions.Create(_direction);
        _session = new PracticeSession(questions);
        _isComplete = false;
        _isCorrect = null;
        _selectedAnswer = null;
    }
    
    private bool CanSwitchDirection => _session.Answers.Count == 0;

    private void SwitchDirection()
    {
        if (!CanSwitchDirection)
        {
            return;
        }

        var newDirection = _direction == PracticeDirection.UkrainianToEnglish
            ? PracticeDirection.EnglishToUkrainian
            : PracticeDirection.UkrainianToEnglish;

        var entries = _importedEntries;
        if (entries is not null)
        {
            var factory = new PracticeQuestionFactory();
            _importedQuestions = entries
                .Select(entry => factory.Create(entry, entries, newDirection))
                .ToArray();
        }

        _direction = newDirection;
        RestartPractice();
    }
}

