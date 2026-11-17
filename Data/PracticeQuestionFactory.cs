using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public class PracticeQuestionFactory
{
    public PracticeQuestion Create(
        VocabularyEntry entry,
        List<VocabularyEntry> entries,
        PracticeDirection direction = PracticeDirection.UkrainianToEnglish)
    {
        bool useUkrainianOptions = direction == PracticeDirection.EnglishToUkrainian;

        string promptText = useUkrainianOptions ? entry.Word : entry.Translation;
        string correctAnswer = useUkrainianOptions ? entry.Translation : entry.Word;

        var options = GenerateOptions(entry, entries, correctAnswer, useUkrainianOptions);

        return new PracticeQuestion(promptText, entry.PartOfSpeech, options, correctAnswer)
        {
            Explanation = entry.Explanation,
            Example = entry.Example
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
        var otherAnswers = candidates.Take(3).ToList();

        if (otherAnswers.Count < 3)
        {
            int missingCount = 3 - otherAnswers.Count;

            var fallbackAnswers = entries
                .Select(candidate => useUkrainianOptions ? candidate.Translation : candidate.Word)
                .Where(answer => answer != correctAnswer && !otherAnswers.Contains(answer))
                .Distinct()
                .ToArray();

            Random.Shared.Shuffle(fallbackAnswers);
            otherAnswers.AddRange(fallbackAnswers.Take(missingCount));
        }

        if (otherAnswers.Count < 3)
        {
            throw new InvalidOperationException("At least four distinct answers are required.");
        }

        string[] options = [correctAnswer, .. otherAnswers];
        Random.Shared.Shuffle(options);

        return options;
    }
}