# EnglishTrainer — project context for Claude

## Communication
- Reply in **Ukrainian** unless I explicitly ask for another language.
- Keep explanations short; prefer bullets and concrete examples over prose.
- When suggesting changes, explain *why* briefly, then show the code.
- Before touching several files, lay out a short plan and wait for "так / продовжуй".

## What this project is
Small **Blazor Server** web app for practicing English vocabulary from a user-supplied CSV word list. Shows typed questions (translation, meaning, example gap-fill), tracks a session, exports results back to CSV.

## Stack
- **.NET 10**, `Microsoft.NET.Sdk.Web`
- **Blazor Server** (`AddRazorComponents().AddInteractiveServerComponents()`), not MVC
- **CsvHelper 33.1** — CSV import/export (no database, no EF Core)
- **Serilog** — console + rolling file sink in `logs/`
- **xUnit + FluentAssertions + coverlet** — tests in `EnglishTrainer.Tests/`
- Dependency Injection via `builder.Services.AddSingleton<...>` in `Program.cs`

## Layout
```
/Components
    App.razor, Routes.razor, _Imports.razor
    /Layout         — MainLayout, NavMenu
    /Pages          — Home, PracticeSummary, QuestionCard, PracticeHeader,
                      AnswerFooter, HelpDialog, Toast, Error
/Data
    CsvVocabularyImporter.cs  (implements IVocabularyImporter)
    CsvVocabularyExporter.cs
    PracticeQuestionFactory.cs
    StarterQuestions.cs
    VocabularyEntryMap.cs
/Models
    VocabularyEntry, PracticeQuestion, PracticeAnswer, PracticeSession
/SampleData         — sample CSVs to try the app
/EnglishTrainer.Tests
    /Data           — importer/exporter/factory tests
    /Models         — PracticeSession tests
```

## Conventions
- Services are **singletons** registered in `Program.cs`.
- Prefer **async/await** for I/O. Use `CancellationToken` where it already exists.
- Code and public identifiers are in **English**; UI strings can be Ukrainian.
- **No new dependencies** without asking me first.
- **No EF Core, no database** — data lives in CSV. Don't suggest a DB migration unless I ask.
- Don't add comments that just restate code. Keep comments for non-obvious *why*.
- Don't create README/docs files unless I ask.

## Testing
- Run tests: `dotnet test`
- Add a test alongside any non-trivial logic change in `Data/` or `Models/`.
- Tests use xUnit + FluentAssertions, see `EnglishTrainer.Tests/GlobalUsings.cs`.

## Run / build
- `dotnet run` from repo root starts the app (usually `https://localhost:5001`).
- Logs land in `logs/app-YYYY-MM-DD.log`.

## Current focus
I'm planning to turn this into a **PWA** (Progressive Web App) so it installs to iPhone/Android home screen and works offline. If a new chat opens on the `feature/pwa` branch, assume PWA work is the active task:
- Add `manifest.webmanifest`, icons, service worker
- Cache static assets + Blazor framework files for offline
- Adapt UI for small screens (touch targets, responsive layout)
- Verify "Add to Home Screen" on iOS Safari

## Git
- Main branch: `main`
- Work on feature branches; don't commit directly to `main`.
- Don't commit, push, or run destructive git commands without asking.
- Don't use `--no-verify`.

## What to ask me before doing
- Adding NuGet packages
- Changing the storage layer (CSV → anything else)
- Rewriting Blazor → MVC or similar architectural switch
- Deleting files or data in `SampleData/` or `logs/`
