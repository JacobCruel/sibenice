namespace Sibenice;

/// <summary>
/// Veškeré vykreslování do konzole - menu, herní stav, žebříček, statistiky.
/// Statická třída bez stavu, jen formátuje a vypisuje.
/// </summary>
public static class Renderer
{
    private const int BoxWidth = 44;

    // ── Pomocné metody pro barevný výpis ───────────────────────

    private static void Write(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }

    private static void WriteLine(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    private static void BlankLine() => Console.WriteLine();

    // ── Kreslení rámečků ───────────────────────────────────────

    private static void DrawBoxTop()
        => WriteLine($"  +{new string('-', BoxWidth)}+", ConsoleColor.DarkCyan);

    private static void DrawBoxBottom()
        => WriteLine($"  +{new string('-', BoxWidth)}+", ConsoleColor.DarkCyan);

    private static void DrawBoxSeparator()
        => WriteLine($"  +{new string('-', BoxWidth)}+", ConsoleColor.DarkCyan);

    /// <summary>Řádek rámečku s textem zarovnaným na střed.</summary>
    private static void DrawBoxLine(string text, ConsoleColor textColor = ConsoleColor.White)
    {
        Write("  |", ConsoleColor.DarkCyan);
        int padding = BoxWidth - text.Length;
        int left = padding / 2;
        int right = padding - left;
        Write(new string(' ', left), ConsoleColor.White);
        Write(text, textColor);
        Write(new string(' ', right), ConsoleColor.White);
        WriteLine("|", ConsoleColor.DarkCyan);
    }

    /// <summary>Řádek rámečku s textem zarovnaným vlevo.</summary>
    private static void DrawBoxLineLeft(string text, ConsoleColor textColor = ConsoleColor.White)
    {
        Write("  | ", ConsoleColor.DarkCyan);
        Write(text, textColor);
        int pad = BoxWidth - text.Length - 1;
        if (pad > 0) Write(new string(' ', pad), ConsoleColor.White);
        WriteLine("|", ConsoleColor.DarkCyan);
    }

    /// <summary>Prázdný řádek rámečku.</summary>
    private static void DrawBoxEmpty()
        => DrawBoxLine("", ConsoleColor.White);

    // ── Titulní obrazovka ──────────────────────────────────────

    public static void DrawTitle()
    {
        Console.Clear();
        BlankLine();
        DrawBoxTop();
        DrawBoxEmpty();
        DrawBoxLine("S I B E N I C E", ConsoleColor.Cyan);
        DrawBoxEmpty();
        DrawBoxBottom();
        BlankLine();
    }

    // ── Hlavní menu ────────────────────────────────────────────

    /// <summary>Zobrazí hlavní menu a vrátí volbu hráče (1-6).</summary>
    public static int MainMenu()
    {
        DrawBoxTop();
        DrawBoxLineLeft("[1]  Hrat sibenici", ConsoleColor.White);
        DrawBoxLineLeft("[2]  Vlastni slova", ConsoleColor.White);
        DrawBoxLineLeft("[3]  Zebricek", ConsoleColor.Yellow);
        DrawBoxLineLeft("[4]  Statistiky hrace", ConsoleColor.Magenta);
        DrawBoxLineLeft("[5]  Pravidla", ConsoleColor.DarkCyan);
        DrawBoxLineLeft("[6]  Konec", ConsoleColor.DarkRed);
        DrawBoxBottom();
        BlankLine();

        Write("  Volba: ", ConsoleColor.Gray);
        if (int.TryParse(Console.ReadLine(), out int choice))
            return choice;
        return 0;
    }

    // ── Zadání jména hráče ─────────────────────────────────────

    /// <summary>Požádá hráče o zadání jména. Pokud nic nezadá, vrátí "Hrac".</summary>
    public static string AskPlayerName()
    {
        Console.Clear();
        BlankLine();
        WriteLine("  Zadej jmeno hrace:", ConsoleColor.Cyan);
        Write("  > ", ConsoleColor.Gray);
        var name = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(name) ? "Hrac" : name;
    }

    // ── Výběr obtížnosti ───────────────────────────────────────

    /// <summary>Zobrazí menu obtížnosti a vrátí zvolenou obtížnost (nebo null při chybě).</summary>
    public static Difficulty? AskDifficulty()
    {
        Console.Clear();
        BlankLine();
        DrawBoxTop();
        DrawBoxLine("OBTIZNOST", ConsoleColor.Cyan);
        DrawBoxSeparator();
        DrawBoxLineLeft("[1]  Lehka       (x1 skore)", ConsoleColor.Green);
        DrawBoxLineLeft("[2]  Stredni     (x2 skore)", ConsoleColor.Yellow);
        DrawBoxLineLeft("[3]  Tezka       (x3 skore)", ConsoleColor.Red);
        DrawBoxBottom();
        BlankLine();
        Write("  Volba: ", ConsoleColor.Gray);

        return Console.ReadLine()?.Trim() switch
        {
            "1" => Difficulty.Lehka,
            "2" => Difficulty.Stredni,
            "3" => Difficulty.Tezka,
            _ => null
        };
    }

    // ── Vykreslení herního stavu ───────────────────────────────

    /// <summary>
    /// Vykreslí kompletní herní obrazovku - šibenice vlevo, info vpravo,
    /// špatná písmena a nápověda dole.
    /// </summary>
    public static void DrawGameState(HangmanGame game)
    {
        Console.Clear();
        BlankLine();

        // Záhlaví
        Write("  === ", ConsoleColor.DarkCyan);
        Write("SIBENICE", ConsoleColor.Cyan);
        WriteLine(" ===", ConsoleColor.DarkCyan);
        BlankLine();

        // Šibenice vlevo + informace vpravo vedle sebe
        var art = HangmanArt.GetStage(game.WrongGuesses);
        var masked = FormatMaskedWord(game.GetMaskedWord());
        var diffName = DifficultyLabel(game.Difficulty);
        var diffColor = DifficultyColor(game.Difficulty);
        var lives = FormatLives(game.LivesRemaining, game.MaxLives);
        var time = game.Elapsed.ToString(@"mm\:ss");

        string[] info =
        {
            "",
            $"Slovo:  {masked}",
            "",
            $"Obtiznost: {diffName}",
            $"Zivoty: {lives}",
            $"Cas: {time}",
            ""
        };

        int lines = Math.Max(art.Length, info.Length);
        for (int i = 0; i < lines; i++)
        {
            // Vykreslení ASCII artu (červeně při prohře, žlutě jinak)
            string artLine = i < art.Length ? art[i] : "         ";
            Console.ForegroundColor = game.IsLost ? ConsoleColor.Red : ConsoleColor.DarkYellow;
            Console.Write($"  {artLine}");
            Console.ResetColor();

            // Vykreslení info řádku s příslušnou barvou
            if (i < info.Length)
            {
                if (i == 1)      // Řádek se slovem
                    WriteLine($"    {info[i]}", ConsoleColor.White);
                else if (i == 3) // Řádek s obtížností
                    WriteLine($"    {info[i]}", diffColor);
                else
                    WriteLine($"    {info[i]}", ConsoleColor.Gray);
            }
            else
            {
                BlankLine();
            }
        }

        // Výpis špatně hádaných písmen
        BlankLine();
        if (game.WrongLetters.Count > 0)
        {
            Write("  Spatna pismena: ", ConsoleColor.DarkGray);
            WriteLine(string.Join(", ", game.WrongLetters), ConsoleColor.Red);
        }

        // Info o nápovědě (zobrazit jen pokud je hra aktivní a hráč má dost životů)
        if (!game.IsOver && game.LivesRemaining > 1)
        {
            Write("  Napoveda: ", ConsoleColor.DarkGray);
            WriteLine("zadej ? (-1 zivot)", ConsoleColor.DarkYellow);
        }

        BlankLine();
        Write("  ", ConsoleColor.DarkCyan);
        WriteLine(new string('-', 40), ConsoleColor.DarkCyan);
    }

    // ── Vstup hádání ───────────────────────────────────────────

    /// <summary>
    /// Požádá hráče o zadání písmene. Vrátí písmeno, "?" pro nápovědu, nebo null při chybě.
    /// </summary>
    public static string? AskGuess()
    {
        Write("  Hadej pismeno: ", ConsoleColor.Gray);
        var input = Console.ReadLine()?.Trim().ToLower();

        if (string.IsNullOrEmpty(input))
            return null;

        // Nápověda
        if (input == "?")
            return "?";

        // Validace - musí být právě jedno písmeno
        if (input.Length != 1 || !char.IsLetter(input[0]))
        {
            ShowMessage("Zadej pouze jedno pismeno!", ConsoleColor.Yellow);
            return null;
        }

        return input;
    }

    // ── Zprávy pro hráče ───────────────────────────────────────

    /// <summary>Zobrazí krátkou barevnou zprávu s pauzou pro přečtení.</summary>
    public static void ShowMessage(string message, ConsoleColor color)
    {
        BlankLine();
        WriteLine($"  >> {message}", color);
        Thread.Sleep(800);
    }

    // ── Obrazovka výhry ────────────────────────────────────────

    /// <summary>Zobrazí gratulaci, panáčka slavícího svobodu a souhrn hry.</summary>
    public static void DrawWin(HangmanGame game, int score)
    {
        Console.Clear();
        BlankLine();

        // Panáček slaví
        var art = HangmanArt.GetWinArt();
        foreach (var line in art)
            WriteLine($"  {line}", ConsoleColor.Green);

        BlankLine();
        WriteLine("  ================================", ConsoleColor.Green);
        WriteLine("        GRATULACE! VYHRAL(A) JSI!", ConsoleColor.Green);
        WriteLine("  ================================", ConsoleColor.Green);
        BlankLine();

        // Souhrn hry
        Write("  Slovo:       ", ConsoleColor.Gray);
        WriteLine(game.Word.ToUpper(), ConsoleColor.White);
        Write("  Obtiznost:   ", ConsoleColor.Gray);
        WriteLine(DifficultyLabel(game.Difficulty), DifficultyColor(game.Difficulty));
        Write("  Zivoty:      ", ConsoleColor.Gray);
        WriteLine($"{game.LivesRemaining}/{game.MaxLives}", ConsoleColor.White);
        Write("  Cas:         ", ConsoleColor.Gray);
        WriteLine(game.Elapsed.ToString(@"mm\:ss"), ConsoleColor.White);
        Write("  Skore:       ", ConsoleColor.Gray);
        WriteLine($"+{score}", ConsoleColor.Yellow);
        BlankLine();
    }

    // ── Obrazovka prohry ───────────────────────────────────────

    /// <summary>Zobrazí oběšeného panáčka a prozradí hádané slovo.</summary>
    public static void DrawLoss(HangmanGame game)
    {
        Console.Clear();
        BlankLine();

        // Mrtvý panáček
        var art = HangmanArt.GetStage(HangmanArt.MaxStages);
        foreach (var line in art)
            WriteLine($"  {line}", ConsoleColor.Red);

        BlankLine();
        WriteLine("  ================================", ConsoleColor.Red);
        WriteLine("        BYL(A) JSI OBESEN(A)!", ConsoleColor.Red);
        WriteLine("  ================================", ConsoleColor.Red);
        BlankLine();

        // Odhalení slova
        Write("  Hadane slovo bylo: ", ConsoleColor.Gray);
        WriteLine(game.Word.ToUpper(), ConsoleColor.Yellow);
        BlankLine();
    }

    // ── Pravidla ───────────────────────────────────────────────

    /// <summary>Zobrazí pravidla hry v rámečku.</summary>
    public static void DrawRules()
    {
        Console.Clear();
        BlankLine();
        DrawBoxTop();
        DrawBoxLine("PRAVIDLA", ConsoleColor.Cyan);
        DrawBoxSeparator();
        DrawBoxLineLeft("Cil: uhadnout slovo skryte v _", ConsoleColor.White);
        DrawBoxLineLeft("Hadej po jednom pismenu", ConsoleColor.White);
        DrawBoxLineLeft("Kazde spatne pismeno = -1 zivot", ConsoleColor.White);
        DrawBoxLineLeft("Mas 7 zivotu (= 7 casti tela)", ConsoleColor.White);
        DrawBoxLineLeft("Napoveda: zadej ? (stoji 1 zivot)", ConsoleColor.Yellow);
        DrawBoxSeparator();
        DrawBoxLineLeft("Skore = (delka + zivoty + cas) x", ConsoleColor.DarkGray);
        DrawBoxLineLeft("  nasobitel obtiznosti (1/2/3)", ConsoleColor.DarkGray);
        DrawBoxBottom();
        BlankLine();
    }

    // ── Žebříček ───────────────────────────────────────────────

    /// <summary>Zobrazí tabulku top 10 nejlepších skóre.</summary>
    public static void DrawLeaderboard(List<GameRecord> records)
    {
        Console.Clear();
        BlankLine();
        DrawBoxTop();
        DrawBoxLine("ZEBRICEK - TOP 10", ConsoleColor.Yellow);
        DrawBoxSeparator();

        if (records.Count == 0)
        {
            DrawBoxLine("Zatim zadne zaznamy!", ConsoleColor.DarkGray);
        }
        else
        {
            // Záhlaví tabulky
            DrawBoxLineLeft("#   Hrac           Skore  Sl.", ConsoleColor.DarkGray);
            for (int i = 0; i < records.Count; i++)
            {
                var r = records[i];

                // Zvýraznění medailových pozic
                var medal = i switch { 0 => ">>", 1 => "> ", 2 => "> ", _ => "  " };
                var color = i switch
                {
                    0 => ConsoleColor.Yellow,     // Zlato
                    1 => ConsoleColor.Gray,        // Stříbro
                    2 => ConsoleColor.DarkYellow,  // Bronz
                    _ => ConsoleColor.White
                };

                var line = $"{medal}{i + 1,2}. {r.Player,-15} {r.Score,5}  {r.Word}";
                if (line.Length > BoxWidth - 1) line = line[..(BoxWidth - 1)];
                DrawBoxLineLeft(line, color);
            }
        }

        DrawBoxBottom();
        BlankLine();
    }

    // ── Statistiky hráče ───────────────────────────────────────

    /// <summary>Zobrazí kompletní statistiky konkrétního hráče.</summary>
    public static void DrawPlayerStats(PlayerStats stats)
    {
        Console.Clear();
        BlankLine();
        DrawBoxTop();
        DrawBoxLine($"STATISTIKY: {stats.Player.ToUpper()}", ConsoleColor.Magenta);
        DrawBoxSeparator();

        if (stats.Games == 0)
        {
            DrawBoxLine("Zatim zadne hry!", ConsoleColor.DarkGray);
        }
        else
        {
            DrawBoxLineLeft($"Celkem her:      {stats.Games}", ConsoleColor.White);
            DrawBoxLineLeft($"Vyher:           {stats.Wins}", ConsoleColor.Green);
            DrawBoxLineLeft($"Proher:          {stats.Losses}", ConsoleColor.Red);
            DrawBoxLineLeft($"Uspesnost:       {stats.WinRate:F1}%", ConsoleColor.Cyan);
            DrawBoxLineLeft($"Aktualni serie:  {stats.CurrentStreak}", ConsoleColor.White);
            DrawBoxLineLeft($"Nejlepsi serie:  {stats.BestStreak}", ConsoleColor.Yellow);
            DrawBoxLineLeft($"Celkove skore:   {stats.TotalScore}", ConsoleColor.White);
            DrawBoxLineLeft($"Nejlepsi skore:  {stats.HighScore}", ConsoleColor.Yellow);
        }

        DrawBoxBottom();
        BlankLine();
    }

    // ── Nastavení vlastních slov ───────────────────────────────

    /// <summary>Zobrazí hlavičku sekce pro zadání vlastních slov.</summary>
    public static void DrawCustomWordsHeader()
    {
        Console.Clear();
        BlankLine();
        DrawBoxTop();
        DrawBoxLine("VLASTNI SLOVA", ConsoleColor.Cyan);
        DrawBoxSeparator();
        DrawBoxLineLeft("Zadej slova oddelena carkou", ConsoleColor.DarkGray);
        DrawBoxLineLeft("Prazdny vstup = ponechat vychozi", ConsoleColor.DarkGray);
        DrawBoxBottom();
        BlankLine();
    }

    /// <summary>Požádá o slova pro jednu obtížnost. Vrátí seznam, nebo null při prázdném vstupu.</summary>
    public static List<string>? AskWordsForDifficulty(string label, ConsoleColor color)
    {
        Write($"  {label}: ", color);
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return null;
        return input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    // ── Rozloučení ─────────────────────────────────────────────

    /// <summary>Zobrazí rozloučení při ukončení hry.</summary>
    public static void DrawGoodbye()
    {
        Console.Clear();
        BlankLine();
        WriteLine("  Diky za hru! Nashledanou.", ConsoleColor.Cyan);
        BlankLine();
    }

    // ── Utility ────────────────────────────────────────────────

    /// <summary>Čeká na stisknutí ENTER.</summary>
    public static void WaitForKey()
    {
        Write("  Stiskni ENTER pro pokracovani...", ConsoleColor.DarkGray);
        Console.ReadLine();
    }

    /// <summary>Formátuje skryté slovo s mezerami mezi znaky pro lepší čitelnost.</summary>
    private static string FormatMaskedWord(string masked)
        => string.Join(' ', masked.ToCharArray());

    /// <summary>Formátuje životy jako [***....] 3/7</summary>
    private static string FormatLives(int remaining, int max)
    {
        var filled = new string('*', remaining);
        var empty = new string('.', max - remaining);
        return $"[{filled}{empty}] {remaining}/{max}";
    }

    /// <summary>Vrátí český popis obtížnosti s násobitelem skóre.</summary>
    private static string DifficultyLabel(Difficulty d) => d switch
    {
        Difficulty.Lehka => "Lehka [x1]",
        Difficulty.Stredni => "Stredni [x2]",
        Difficulty.Tezka => "Tezka [x3]",
        _ => "?"
    };

    /// <summary>Vrátí barvu pro danou obtížnost (zelená/žlutá/červená).</summary>
    private static ConsoleColor DifficultyColor(Difficulty d) => d switch
    {
        Difficulty.Lehka => ConsoleColor.Green,
        Difficulty.Stredni => ConsoleColor.Yellow,
        Difficulty.Tezka => ConsoleColor.Red,
        _ => ConsoleColor.White
    };
}
