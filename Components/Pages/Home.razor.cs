using EnglishTrainer.Data;
using EnglishTrainer.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace EnglishTrainer.Components.Pages;

public partial class Home
{
    [Inject] public ILogger<Home> Logger { get; set; } = default!;
    [Inject] public PracticeQuestionFactory Factory { get; set; } = default!;
    [Inject] public IVocabularyImporter Importer { get; set; } = default!;

    private PracticeController _controller = default!;
    private string? _errorMessage;
    private bool _isHelpOpen;

    protected override void OnInitialized()
    {
        _controller = new PracticeController(Factory);
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
            using var browserStream = file.OpenReadStream(Importer.MaxSizeBytes);
            using var csvStream = new MemoryStream();

            await browserStream.CopyToAsync(csvStream);
            csvStream.Position = 0;

            var entries = Importer.Read(csvStream);
            _controller.LoadEntries(entries, _controller.Direction);

            Logger.LogInformation(
                "CSV import succeeded for {FileName}: {EntryCount} entries, direction {Direction}.",
                file.Name, entries.Count, _controller.Direction);
        }
        catch (IOException ex)
        {
            var maxMb = Importer.MaxSizeBytes / 1024 / 1024;
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
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while importing {FileName}.", file.Name);
            _errorMessage = "Something went wrong while reading this file. Please try another one.";
        }
    }
}
