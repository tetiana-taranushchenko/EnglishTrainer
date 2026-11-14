namespace EnglishTrainer.Data;
using EnglishTrainer.Models;

public class StarterQuestions
{
    public static PracticeQuestion[] Create()
    {
        return
        [
            new PracticeQuestion(
                "надійний",
                "adjective",
                ["intentional", "reliable", "time-consuming", "flexible"],
                "reliable"
            ),
            new PracticeQuestion(
                "розмірковувати",
                "verb",
                ["conduct", "maintain", "suggest", "contemplate"],
                "contemplate"
            )
        ];
    }

}