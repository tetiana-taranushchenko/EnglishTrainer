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
                ) { EnglishWord = "reliable" },
                new PracticeQuestion(
                    "contemplate",
                    "verb",
                    ["проводити", "підтримувати", "пропонувати", "розмірковувати"],
                    "розмірковувати",
                    promptLanguage: "en",
                    optionsLanguage: "uk"
                ) { EnglishWord = "contemplate" }
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
            ) { EnglishWord = "reliable" },
            new PracticeQuestion(
                "розмірковувати",
                "verb",
                ["conduct", "maintain", "suggest", "contemplate"],
                "contemplate",
                promptLanguage: "uk",
                optionsLanguage: "en"
            ) { EnglishWord = "contemplate" }
        ];
    }
}