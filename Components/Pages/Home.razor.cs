using EnglishTrainer.Data;
using EnglishTrainer.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace EnglishTrainer.Components.Pages;

public partial class Home
{
    private PracticeSession _session = new(StarterQuestions.Create());
    private PracticeQuestion[]? _importedQuestions;
    private bool? _isCorrect;
    private string? _selectedAnswer;
    private bool _isComplete;
        
    private void SelectAnswer(string option)
    {
        if (_isCorrect.HasValue)
        {
            return;
        }
        _selectedAnswer = option;
        _isCorrect = _session.IsCorrectAnswer(option);
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
        var questions = entries
            .Select(entry => factory.Create(entry, entries))
            .ToArray();

        _session = new PracticeSession(questions);
        _importedQuestions = questions;
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
        _session = new PracticeSession(StarterQuestions.Create());
        _isComplete = false;
        _isCorrect = null;
        _selectedAnswer = null;
    }
}

