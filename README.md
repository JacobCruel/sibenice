# Šibenice (Hangman)

Console-based Hangman game in C# — semester project for **Czech University of Life Sciences Prague** (Faculty of Economics and Management, Informatics).

**Author:** Jakub Siman

## About

A classic hangman game where the player guesses a hidden word letter by letter. The game features three difficulty levels, a scoring system, a persistent leaderboard, and player statistics. All in-game text is in Czech.

### Features

- **3 difficulty levels** — easy, medium, hard (with score multipliers x1/x2/x3)
- **Scoring system** — score based on word length, remaining lives, and speed
- **Hints** — reveal a random letter at the cost of one life (type `?`)
- **Leaderboard** — persistent top 10 high scores (saved to `scores.json`)
- **Player statistics** — wins, losses, win rate, streaks, high score
- **Custom words** — option to provide your own word lists per difficulty
- **Progressive ASCII art** — 8 hangman stages from empty gallows to game over

## Getting Started

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet run
```

## Project Structure

| File | Description |
|---|---|
| `Program.cs` | Entry point, main menu, game loop orchestration |
| `HangmanGame.cs` | Core game logic — state, guessing, scoring, hints |
| `Renderer.cs` | All console rendering and UI output |
| `HangmanArt.cs` | ASCII hangman art (8 progressive stages) |
| `WordManager.cs` | Word dictionary loading and management |
| `ScoreBoard.cs` | Leaderboard and player statistics |
| `Models.cs` | Data models and enums |
| `default.json` | Default word dictionary — 20 words per difficulty |
