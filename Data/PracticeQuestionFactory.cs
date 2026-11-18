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
        var candidates = entries
            .Where(vocabularyEntry => vocabularyEntry.PartOfSpeech == entry.PartOfSpeech)
            .Select(vocabularyEntry => useUkrainianOptions
                ? vocabularyEntry.Translation
                : vocabularyEntry.Word)
            .Where(answer => answer != correctAnswer)
            .Distinct()
            .ToArray();

        Random.Shared.Shuffle(candidates);
        var otherAnswers = candidates.Take(OtherAnswersPerQuestion).ToList();

        if (otherAnswers.Count < OtherAnswersPerQuestion)
        {
            int missingCount = OtherAnswersPerQuestion - otherAnswers.Count;

            var fallbackAnswers = entries
                .Select(candidate => useUkrainianOptions ? candidate.Translation : candidate.Word)
                .Where(answer => answer != correctAnswer && !otherAnswers.Contains(answer))
                .Distinct()
                .ToArray();

            Random.Shared.Shuffle(fallbackAnswers);
            otherAnswers.AddRange(fallbackAnswers.Take(missingCount));
        }

        if (otherAnswers.Count < OtherAnswersPerQuestion)
        {
            throw new InvalidOperationException(
                $"At least {OptionsPerQuestion} distinct answers are required.");
        }

        string[] options = [correctAnswer, .. otherAnswers];
        Random.Shared.Shuffle(options);

        return options;
    }
}