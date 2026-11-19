using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public class PracticeQuestionFactory
{
    public const int OptionsPerQuestion = 4;
    private const int OtherAnswersPerQuestion = OptionsPerQuestion - 1;

    public PracticeQuestion Create(
        VocabularyEntry entry,
        List<VocabularyEntry> entries,
        PracticeDirection direction = PracticeDirection.UkrainianToEnglish)
    {
        bool useUkrainianOptions = direction == PracticeDirection.EnglishToUkrainian;

        string promptText = useUkrainianOptions ? entry.Word : entry.Translation;
        string correctAnswer = useUkrainianOptions ? entry.Translation : entry.Word;
        string promptLanguage = useUkrainianOptions ? "en" : "uk";
        string optionsLanguage = useUkrainianOptions ? "uk" : "en";

        var options = GenerateOptions(entry, entries, correctAnswer, useUkrainianOptions);

        return new PracticeQuestion(
            promptText,
            entry.PartOfSpeech,
            options,
            correctAnswer,
            promptLanguage,
            optionsLanguage)
        {
            Explanation = entry.Explanation,
            Example = entry.Example,
            EnglishWord = entry.Word
        };
    }

    private string[] GenerateOptions(
        VocabularyEntry entry,
        List<VocabularyEntry> entries,
        string correctAnswer,
        bool useUkrainianOptions)
    {
        var sameType = PickDistractors(
            entries.Where(candidate => candidate.PartOfSpeech == entry.PartOfSpeech),
            correctAnswer,
            useUkrainianOptions,
            excluded: null);

        var distractors = sameType.Take(OtherAnswersPerQuestion).ToList();

        if (distractors.Count < OtherAnswersPerQuestion)
        {
            var fallback = PickDistractors(entries, correctAnswer, useUkrainianOptions, distractors);
            distractors.AddRange(fallback.Take(OtherAnswersPerQuestion - distractors.Count));
        }

        if (distractors.Count < OtherAnswersPerQuestion)
        {
            throw new InvalidOperationException(
                $"At least {OptionsPerQuestion} distinct answers are required.");
        }

        var options = new string[OptionsPerQuestion];
        options[0] = correctAnswer;
        for (int i = 0; i < distractors.Count; i++)
        {
            options[i + 1] = distractors[i];
        }

        Random.Shared.Shuffle(options);
        return options;
    }

    private static List<string> PickDistractors(
        IEnumerable<VocabularyEntry> source,
        string correctAnswer,
        bool useUkrainianOptions,
        IReadOnlyCollection<string>? excluded)
    {
        var pool = source
            .Select(candidate => useUkrainianOptions ? candidate.Translation : candidate.Word)
            .Where(answer => answer != correctAnswer)
            .Where(answer => excluded is null || !excluded.Contains(answer))
            .Distinct()
            .ToList();

        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        return pool;
    }
}