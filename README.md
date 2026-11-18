# EnglishTrainer

A small Blazor web app for practicing English vocabulary from your own word list.

## What it does

- Loads your words from a CSV file.
- Quizzes you with different question types (translation, meaning, example gap-fill, etc.).
- Shows your progress and a summary at the end of each practice session.
- Lets you export your word list back to CSV.

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download)

## Run the app

```bash
dotnet run
```

Then open the URL shown in the terminal (usually `https://localhost:5001`) in your browser.

## Using your own word list

1. Prepare a CSV file with this header:

   ```
   word,translation,explanation,example,type
   ```

2. Add your words, one per row. Example:

   ```
   circumstance,обставина,A fact or condition that affects a situation.,A change in the schedule was the only circumstance that delayed the release.,noun
   ```

3. Import the file from the app's home page.

Sample files are in the `SampleData/` folder if you want to try the app first.

## Tips

- Separate multiple translations with `;` (e.g. `обмірковувати; розглядати`).
- Wrap any field that contains a comma in double quotes.
- Your session logs are stored in the `logs/` folder.
