namespace EnglishTrainer.Models;

public class VocabularyEntry
{
    public string Word { get; set; } = "";
    public string Translation { get; set; } = "";
    public string Explanation { get; set; } = "";
    public string Example { get; set; } = "";
    public string PartOfSpeech { get; set; } = "";

    public VocabularyEntry() { }

    public VocabularyEntry(string word, string translation, string explanation, string example, string partOfSpeech)
    {
        Word = word;
        Translation = translation;
        Explanation = explanation;
        Example = example;
        PartOfSpeech = partOfSpeech;
    }
}