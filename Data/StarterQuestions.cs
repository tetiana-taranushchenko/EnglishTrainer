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
                    "надійний",
                    promptLanguage: "en",
                    optionsLanguage: "uk"
                ),
                new PracticeQuestion(
                    "contemplate",
                    "verb",
                    ["проводити", "підтримувати", "пропонувати", "розмірковувати"],
                    "розмірковувати",
                    promptLanguage: "en",
                    optionsLanguage: "uk"
                )
            ];
        }

        return
        [
            new PracticeQuestion(
                "надійний",
                "adjective",
                ["intentional", "reliable", "time-consuming", "flexible"],
                "reliable",
                promptLanguage: "uk",
                optionsLanguage: "en"
            ),
            new PracticeQuestion(
                "розмірковувати",
                "verb",
                ["conduct", "maintain", "suggest", "contemplate"],
                "contemplate",
                promptLanguage: "uk",
                optionsLanguage: "en"
            )
        ];
    }
}