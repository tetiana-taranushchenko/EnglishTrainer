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
    public string? Explanation { get; set; }
    public string? Example { get; set; }
    
    public PracticeQuestion(string promptText, string partOfSpeech, string[] options, string correctAnswer)
    {
        PromptText = promptText;
        PartOfSpeech = partOfSpeech;
        Options = options;
        CorrectAnswer = correctAnswer;
    }
}