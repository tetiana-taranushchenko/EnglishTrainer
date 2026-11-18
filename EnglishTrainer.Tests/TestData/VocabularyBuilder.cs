namespace EnglishTrainer.Tests.TestData;

internal static class VocabularyBuilder
{
    public static VocabularyEntry Entry(
        string word = "cat",
        string translation = "кіт",
        string partOfSpeech = "noun",
        string explanation = "small animal",
        string example = "The cat sleeps.")
        => new(word, translation, explanation, example, partOfSpeech);

    public static List<VocabularyEntry> Nouns(int count)
    {
        var words = new[] { "cat", "dog", "house", "car", "book", "tree", "river", "chair" };
        var translations = new[] { "кіт", "собака", "дім", "авто", "книга", "дерево", "річка", "стілець" };
        var list = new List<VocabularyEntry>();
        for (int i = 0; i < count; i++)
        {
            list.Add(Entry(words[i % words.Length] + i, translations[i % translations.Length] + i));
        }
        return list;
    }
}
