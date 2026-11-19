namespace EnglishTrainer.Models;

public class PracticeAnswer
{
    public PracticeQuestion Question { get; }
    public string SelectedAnswer { get; }
    public bool IsCorrect { get; }

    public PracticeAnswer(PracticeQuestion question, string selectedAnswer, bool isCorrect)
    {
        Question = question;
        SelectedAnswer = selectedAnswer;
        IsCorrect = isCorrect;
    }
}
