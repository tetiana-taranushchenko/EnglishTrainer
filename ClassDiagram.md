# Схема класів — English Trainer

UML-схема класів, які використовуються у проєкті (папки `Models/` та `Data/`).

![Схема класів English Trainer](docs/class-diagram.svg)

> Якщо діаграма у попередньому перегляді виглядає маленькою — відкрий файл `docs/class-diagram.svg` напряму (подвійним кліком у IDE або у браузері): SVG векторний і масштабується без втрати якості.

## Пояснення зв'язків

- **PracticeSession → PracticeQuestion** — агрегація: сесія отримує масив запитань ззовні і зберігає посилання на них.
- **PracticeSession → PracticeAnswer** — композиція: об'єкти відповідей створюються та належать сесії, зберігаються у внутрішньому списку `_answers`.
- **PracticeAnswer → PracticeQuestion** — асоціація: кожна відповідь посилається на запитання, на яке вона дана.
- **PracticeQuestionFactory → VocabularyEntry / PracticeQuestion** — залежність: фабрика на основі словникових записів генерує запитання з варіантами відповідей.
- **CsvVocabularyImporter → VocabularyEntry** — залежність: імпортер читає CSV і створює колекцію словникових записів.
- **StarterQuestions** — статична утиліта, що повертає початковий набір запитань для обраного напрямку.
- **PracticeDirection** — перелік (enum), що визначає напрямок перекладу: з української на англійську або навпаки.

## Позначення

| Символ | Значення |
|---|---|
| `+` | публічний член (public) |
| `-` | приватний член (private) |
| `$` | статичний метод |
| `*--` | композиція |
| `o--` | агрегація |
| `-->` | асоціація (посилання) |
| `..>` | залежність (використання/створення) |

---

<details>
<summary>Початковий код Mermaid (для перегенерації SVG)</summary>

Файл з кодом діаграми: [`docs/class-diagram.mmd`](docs/class-diagram.mmd)

Перегенерувати зображення:

```bash
npx -y -p @mermaid-js/mermaid-cli mmdc \
  -i docs/class-diagram.mmd \
  -o docs/class-diagram.svg \
  -t dark -b transparent
```

</details>
