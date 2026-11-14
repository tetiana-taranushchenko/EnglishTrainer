using EnglishTrainer.Models;

namespace EnglishTrainer.Data;

public class PracticeQuestionFactory
{
    public PracticeQuestion Create(VocabularyEntry entry, List<VocabularyEntry> entries)
    {
        var options = GenerateOptions(entry, entries);
        return new PracticeQuestion(
            entry.Translation,
            entry.PartOfSpeech,
            options,
            entry.Word)
        {
            Explanation = entry.Explanation,
            Example = entry.Example
        };
    }
    
    private string[] GenerateOptions(VocabularyEntry entry, List<VocabularyEntry> entries)
    {
        var candidates = entries
            .Where(e => e.PartOfSpeech == entry.PartOfSpeech)
            .Select(e => e.Word)
            .Where(word => word != entry.Word)
            .Distinct()
            .ToArray();

        Random.Shared.Shuffle(candidates);
        var otherWords = candidates.Take(3).ToList();

        if (otherWords.Count < 3)
        {
            int missingCount = 3 - otherWords.Count;

            var fallbackWords = entries
                .Select(candidate => candidate.Word)
                .Where(word => word != entry.Word && !otherWords.Contains(word))
                .Distinct()
                .ToArray();

            Random.Shared.Shuffle(fallbackWords);
            otherWords.AddRange(fallbackWords.Take(missingCount));
        }
        
        if (otherWords.Count < 3)
        {
            throw new InvalidOperationException("At least four distinct words are required.");
        }

        string[] options = [entry.Word, .. otherWords];
        Random.Shared.Shuffle(options);

        return options;
    }
}