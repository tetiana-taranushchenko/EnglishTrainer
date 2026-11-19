using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public class StarterQuestions
{
    private record StarterItem(
        string English,
        string Ukrainian,
        string PartOfSpeech,
        string[] UkrainianOptions,
        string[] EnglishOptions);

    private static readonly StarterItem[] Items =
    [
        new(
            "reliable",
            "надійний",
            "adjective",
            ["навмисний", "надійний", "потребує багато часу", "гнучкий"],
            ["intentional", "reliable", "time-consuming", "flexible"]),
        new(
            "contemplate",
            "розмірковувати",
            "verb",
            ["проводити", "підтримувати", "пропонувати", "розмірковувати"],
            ["conduct", "maintain", "suggest", "contemplate"])
    ];

    private static readonly PracticeQuestion[] UkToEn = Build(PracticeDirection.UkrainianToEnglish);
    private static readonly PracticeQuestion[] EnToUk = Build(PracticeDirection.EnglishToUkrainian);

    public static PracticeQuestion[] Create(
        PracticeDirection direction = PracticeDirection.UkrainianToEnglish) =>
        direction == PracticeDirection.EnglishToUkrainian ? EnToUk : UkToEn;

    private static PracticeQuestion[] Build(PracticeDirection direction)
    {
        bool enToUk = direction == PracticeDirection.EnglishToUkrainian;

        return Items.Select(item => new PracticeQuestion(
            promptText: enToUk ? item.English : item.Ukrainian,
            partOfSpeech: item.PartOfSpeech,
            options: enToUk ? item.UkrainianOptions : item.EnglishOptions,
            correctAnswer: enToUk ? item.Ukrainian : item.English,
            promptLanguage: enToUk ? "en" : "uk",
            optionsLanguage: enToUk ? "uk" : "en"
        ) { EnglishWord = item.English }).ToArray();
    }
}
