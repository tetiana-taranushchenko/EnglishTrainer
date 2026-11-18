using EnglishTrainer.Data;
using EnglishTrainer.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace EnglishTrainer.Components.Pages;

public partial class Home
{
    [Inject] public ILogger<Home> Logger { get; set; } = default!;
    [Inject] public PracticeQuestionFactory Factory { get; set; } = default!;
    [Inject] public CsvVocabularyImporter Importer { get; set; } = default!;

    private PracticeSession _session = new(StarterQuestions.Create());
    private PracticeQuestion[]? _importedQuestions;
    private List<VocabularyEntry>? _importedEntries;
    private bool? _isCorrect;
    private string? _selectedAnswer;
    private bool _isComplete;
    private PracticeDirection _direction = PracticeDirection.UkrainianToEnglish;
    private string? _errorMessage;
    private bool _isHelpOpen;

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
        _errorMessage = null;

        if (!file.Name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            Logger.LogWarning("Rejected upload {FileName}: wrong extension.", file.Name);
            _errorMessage = "Please upload a file with a .csv extension.";
            return;
        }

        try
        {
            using var browserStream = file.OpenReadStream(CsvVocabularyImporter.MaxSizeBytes);
            using var csvStream = new MemoryStream();

            await browserStream.CopyToAsync(csvStream);
            csvStream.Position = 0;

            var entries = Importer.Read(csvStream);
            RebuildPractice(entries, _direction);

            Logger.LogInformation(
                "CSV import succeeded for {FileName}: {EntryCount} entries, direction {Direction}.",
                file.Name, entries.Count, _direction);
        }
        catch (IOException ex)
        {
            var maxMb = CsvVocabularyImporter.MaxSizeBytes / 1024 / 1024;
            Logger.LogWarning(ex, "Rejected upload {FileName}: exceeds {MaxMb} MB limit.", file.Name, maxMb);
            _errorMessage = $"This file is too large. The maximum size is {maxMb} MB.";
        }
        catch (InvalidDataException ex)
        {
            Logger.LogWarning(ex, "Rejected upload {FileName}: invalid CSV content.", file.Name);
            _errorMessage = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogError(ex, "Failed to build practice session from {FileName}.", file.Name);
            _errorMessage = "Could not build a practice session from this file. Please check the content.";
        }
    }

    private void GoToNextQuestion()
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

        if (_importedEntries is not null)
        {
            RebuildPractice(_importedEntries, newDirection);
        }
        else
        {
            _direction = newDirection;
            RestartPractice();
        }
    }

    private void RebuildPractice(List<VocabularyEntry> entries, PracticeDirection direction)
    {
        var questions = entries
            .Select(entry => Factory.Create(entry, entries, direction))
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
