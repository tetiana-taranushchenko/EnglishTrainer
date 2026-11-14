namespace EnglishTrainer.Models;

public class PracticeQuestion
{
    public string UkrainianText { get; set; }
    public string PartOfSpeech { get; set; }
    public string[] Options { get; set; }
    public string CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    public string? Example { get; set; }
    
    public PracticeQuestion(string ukrainianText, string partOfSpeech, string[] options, string correctAnswer)
    {
        UkrainianText = ukrainianText;
        PartOfSpeech = partOfSpeech;
        Options = options;
        CorrectAnswer = correctAnswer;
    }
}