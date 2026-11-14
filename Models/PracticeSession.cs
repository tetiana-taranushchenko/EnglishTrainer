namespace EnglishTrainer.Models;

public class PracticeSession
{
    PracticeQuestion[] _questions;
    private int _currentQuestionIndex = 0;
    public PracticeQuestion CurrentQuestion => _questions[_currentQuestionIndex];
    
    public PracticeSession(PracticeQuestion[] questions)
    {
        _questions = questions;
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
    
}