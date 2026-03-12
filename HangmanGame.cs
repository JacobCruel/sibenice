namespace Sibenice;

using System.Diagnostics;

/// <summary>
/// Jádro herní logiky šibenice.
/// Spravuje stav hry: hádané slovo, pokusy, životy, čas a výpočet skóre.
/// </summary>
public class HangmanGame
{
    /// <summary>Hádané slovo (malými písmeny).</summary>
    public string Word { get; }

    /// <summary>Zvolená obtížnost.</summary>
    public Difficulty Difficulty { get; }

    /// <summary>Maximální počet životů (= počet fází šibenice).</summary>
    public int MaxLives => HangmanArt.MaxStages;

    /// <summary>Počet špatných pokusů.</summary>
    public int WrongGuesses { get; private set; }

    /// <summary>Zbývající životy.</summary>
    public int LivesRemaining => MaxLives - WrongGuesses;

    /// <summary>Množina správně uhádnutých písmen.</summary>
    public HashSet<char> CorrectLetters { get; } = new();

    /// <summary>Seznam špatně hádaných písmen (v pořadí hádání).</summary>
    public List<char> WrongLetters { get; } = new();

    /// <summary>Hra je vyhraná, když jsou všechna písmena odhalena.</summary>
    public bool IsWon => Word.All(c => CorrectLetters.Contains(c));

    /// <summary>Hra je prohraná, když dojdou životy.</summary>
    public bool IsLost => LivesRemaining <= 0;

    /// <summary>Hra skončila (výhra nebo prohra).</summary>
    public bool IsOver => IsWon || IsLost;

    // Měření času hry
    private readonly Stopwatch _timer = Stopwatch.StartNew();

    /// <summary>Uplynulý čas od začátku hry.</summary>
    public TimeSpan Elapsed => _timer.Elapsed;

    public HangmanGame(string word, Difficulty difficulty)
    {
        Word = word.ToLower().Trim();
        Difficulty = difficulty;
    }

    /// <summary>
    /// Vrátí slovo se skrytými neuhádnutými písmeny (nahrazeny '_').
    /// </summary>
    public string GetMaskedWord()
    {
        return new string(Word.Select(c => CorrectLetters.Contains(c) ? c : '_').ToArray());
    }

    /// <summary>
    /// Zpracuje hádání jednoho písmene a vrátí výsledek.
    /// </summary>
    public GuessResult Guess(char letter)
    {
        letter = char.ToLower(letter);

        // Neplatný vstup
        if (!char.IsLetter(letter))
            return GuessResult.AlreadyGuessed;

        // Kontrola opakovaného hádání
        if (CorrectLetters.Contains(letter) || WrongLetters.Contains(letter))
            return GuessResult.AlreadyGuessed;

        // Písmeno je ve slově -> správně
        if (Word.Contains(letter))
        {
            CorrectLetters.Add(letter);
            if (IsWon) { _timer.Stop(); return GuessResult.Won; }
            return GuessResult.Correct;
        }

        // Písmeno není ve slově -> špatně, ztráta života
        WrongLetters.Add(letter);
        WrongGuesses++;

        if (IsLost) { _timer.Stop(); return GuessResult.Lost; }
        return GuessResult.Wrong;
    }

    /// <summary>
    /// Použije nápovědu - odhalí jedno náhodné písmeno za cenu jednoho života.
    /// Vrátí odhalené písmeno, nebo null pokud nápověda není dostupná.
    /// </summary>
    public char? UseHint()
    {
        // Nápověda vyžaduje alespoň 2 životy (stojí 1 život)
        if (LivesRemaining <= 1) return null;

        // Najdi dosud neodhalená písmena
        var unrevealed = Word.Distinct().Where(c => !CorrectLetters.Contains(c)).ToList();
        if (unrevealed.Count == 0) return null;

        // Náhodně vyber jedno a odhal ho
        var hint = unrevealed[Random.Shared.Next(unrevealed.Count)];
        CorrectLetters.Add(hint);
        WrongGuesses++; // Nápověda stojí jeden život
        return hint;
    }

    /// <summary>
    /// Vypočítá skóre: (základ + bonus za životy + bonus za čas) * násobitel obtížnosti.
    /// </summary>
    public int CalculateScore()
    {
        if (!IsWon) return 0;

        int multiplier = Difficulty switch
        {
            Difficulty.Lehka => 1,
            Difficulty.Stredni => 2,
            Difficulty.Tezka => 3,
            _ => 1
        };

        int baseScore = Word.Length * 10;           // Delší slovo = víc bodů
        int livesBonus = LivesRemaining * 15;       // Víc zbylých životů = víc bodů
        int timeBonus = Math.Max(0, 120 - (int)Elapsed.TotalSeconds); // Rychlejší = víc bodů

        return (baseScore + livesBonus + timeBonus) * multiplier;
    }
}
