namespace Sibenice;

using System.Text;

/// <summary>
/// Vstupní bod aplikace - hlavní menu a orchestrace herních kol.
/// </summary>
class Program
{
    static readonly WordManager Words = new();
    static readonly ScoreBoard Scores = new();
    static string? PlayerName;

    static void Main()
    {
        // Nastavení kódování pro správné zobrazení češtiny
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        Console.Title = "Sibenice v2.0";

        // Načtení výchozího slovníku
        Words.LoadFromFile("default.json");

        // Hlavní smyčka menu
        bool running = true;
        while (running)
        {
            Renderer.DrawTitle();
            int choice = Renderer.MainMenu();

            switch (choice)
            {
                case 1: // Hrát šibenici
                    EnsurePlayerName();
                    PlayRound();
                    break;
                case 2: // Vlastní slova
                    SetupCustomWords();
                    break;
                case 3: // Žebříček
                    Renderer.DrawLeaderboard(Scores.TopScores());
                    Renderer.WaitForKey();
                    break;
                case 4: // Statistiky hráče
                    EnsurePlayerName();
                    Renderer.DrawPlayerStats(Scores.StatsFor(PlayerName!));
                    Renderer.WaitForKey();
                    break;
                case 5: // Pravidla
                    Renderer.DrawRules();
                    Renderer.WaitForKey();
                    break;
                case 6: // Konec
                    running = false;
                    break;
                default:
                    Renderer.ShowMessage("Neplatna volba! Zadej 1-6.", ConsoleColor.Red);
                    break;
            }
        }

        Renderer.DrawGoodbye();
    }

    /// <summary>Zajistí, že hráč zadal své jméno (zeptá se jen jednou).</summary>
    static void EnsurePlayerName()
    {
        PlayerName ??= Renderer.AskPlayerName();
    }

    /// <summary>
    /// Odehraje jedno herní kolo - výběr obtížnosti, hádání písmen,
    /// vyhodnocení výhry/prohry a uložení záznamu.
    /// </summary>
    static void PlayRound()
    {
        // Výběr obtížnosti
        var difficulty = Renderer.AskDifficulty();
        if (difficulty == null)
        {
            Renderer.ShowMessage("Neplatna obtiznost.", ConsoleColor.Red);
            return;
        }

        // Vylosování slova a vytvoření nové hry
        var word = Words.GetRandomWord(difficulty.Value);
        var game = new HangmanGame(word, difficulty.Value);

        // Herní smyčka - hádání písmen dokud hra neskončí
        while (!game.IsOver)
        {
            Renderer.DrawGameState(game);
            var input = Renderer.AskGuess();

            if (input == null) continue;

            // Zpracování nápovědy
            if (input == "?")
            {
                var hint = game.UseHint();
                if (hint == null)
                    Renderer.ShowMessage("Nemas dost zivotu na napovedu!", ConsoleColor.Red);
                else
                    Renderer.ShowMessage($"Napoveda: pismeno '{hint}' odhaleno! (-1 zivot)", ConsoleColor.DarkYellow);

                if (game.IsOver) break;
                continue;
            }

            // Zpracování hádání písmene
            var result = game.Guess(input[0]);
            switch (result)
            {
                case GuessResult.Correct:
                    Renderer.ShowMessage("Spravne!", ConsoleColor.Green);
                    break;
                case GuessResult.Wrong:
                    Renderer.ShowMessage($"Spatne! Zbyva {game.LivesRemaining} zivotu.", ConsoleColor.Red);
                    break;
                case GuessResult.AlreadyGuessed:
                    Renderer.ShowMessage($"Pismeno '{input[0]}' jsi uz hadal(a)!", ConsoleColor.Yellow);
                    break;
                case GuessResult.Won:
                case GuessResult.Lost:
                    break; // Konec hry se zpracuje níže
            }
        }

        // Zobrazení finálního stavu šibenice
        Renderer.DrawGameState(game);
        Thread.Sleep(500);

        // Uložení záznamu do žebříčku
        var record = new GameRecord
        {
            Player = PlayerName!,
            Word = game.Word,
            Difficulty = game.Difficulty,
            Won = game.IsWon,
            Score = game.CalculateScore(),
            RemainingLives = game.LivesRemaining,
            TimeSeconds = game.Elapsed.TotalSeconds,
            Date = DateTime.Now
        };
        Scores.AddRecord(record);

        // Zobrazení obrazovky výhry nebo prohry
        if (game.IsWon)
            Renderer.DrawWin(game, record.Score);
        else
            Renderer.DrawLoss(game);

        Renderer.WaitForKey();
    }

    /// <summary>Umožní hráči zadat vlastní slova pro jednotlivé obtížnosti.</summary>
    static void SetupCustomWords()
    {
        Renderer.DrawCustomWordsHeader();

        var easy = Renderer.AskWordsForDifficulty("Lehka", ConsoleColor.Green);
        var medium = Renderer.AskWordsForDifficulty("Stredni", ConsoleColor.Yellow);
        var hard = Renderer.AskWordsForDifficulty("Tezka", ConsoleColor.Red);

        // Nastavit jen ty obtížnosti, kde hráč něco zadal
        if (easy != null) Words.SetCustomWords(Difficulty.Lehka, easy);
        if (medium != null) Words.SetCustomWords(Difficulty.Stredni, medium);
        if (hard != null) Words.SetCustomWords(Difficulty.Tezka, hard);

        Words.SaveToFile("custom_slova.json");
        Renderer.ShowMessage("Slova ulozena!", ConsoleColor.Green);
        Renderer.WaitForKey();
    }
}
