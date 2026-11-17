using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public class StarterQuestions
{
    public static PracticeQuestion[] Create(
        PracticeDirection direction = PracticeDirection.UkrainianToEnglish)
    {
        if (direction == PracticeDirection.EnglishToUkrainian)
        {
            return
            [
                new PracticeQuestion(
                    "reliable",
                    "adjective",
                    ["навмисний", "надійний", "потребує багато часу", "гнучкий"],
                    "надійний"
                ),
                new PracticeQuestion(
                    "contemplate",
                    "verb",
                    ["проводити", "підтримувати", "пропонувати", "розмірковувати"],
                    "розмірковувати"
                )
            ];
        }

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