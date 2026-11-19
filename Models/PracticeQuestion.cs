namespace EnglishTrainer.Models;

public enum PracticeDirection
{
    UkrainianToEnglish,
    EnglishToUkrainian
}

public class PracticeQuestion
{
    public string PromptText { get; }
    public string PartOfSpeech { get; }
    public string[] Options { get; }
    public string CorrectAnswer { get; }
    public string PromptLanguage { get; }
    public string OptionsLanguage { get; }
    public string? Explanation { get; init; }
    public string? Example { get; init; }
    public string? EnglishWord { get; init; }

    public PracticeQuestion(
        string promptText,
        string partOfSpeech,
        string[] options,
        string correctAnswer,
        string promptLanguage = "uk",
        string optionsLanguage = "en")
    {
        PromptText = promptText;
        PartOfSpeech = partOfSpeech;
        Options = options;
        CorrectAnswer = correctAnswer;
        PromptLanguage = promptLanguage;
        OptionsLanguage = optionsLanguage;
    }
}