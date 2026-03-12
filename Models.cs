namespace Sibenice;

/// <summary>Úroveň obtížnosti hry - ovlivňuje výběr slov a násobitel skóre.</summary>
public enum Difficulty
{
    Lehka = 1,
    Stredni = 2,
    Tezka = 3
}

/// <summary>Výsledek jednoho hádání písmene.</summary>
public enum GuessResult
{
    Correct,        // Písmeno je ve slově
    Wrong,          // Písmeno není ve slově
    AlreadyGuessed, // Písmeno už bylo hádáno dříve
    Won,            // Poslední správné písmeno -> výhra
    Lost            // Poslední špatné písmeno -> prohra
}

/// <summary>Záznam jedné odehrané hry pro žebříček a statistiky.</summary>
public class GameRecord
{
    public string Player { get; set; } = "";
    public string Word { get; set; } = "";
    public Difficulty Difficulty { get; set; }
    public bool Won { get; set; }
    public int Score { get; set; }
    public int RemainingLives { get; set; }
    public double TimeSeconds { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}

/// <summary>Souhrnné statistiky jednoho hráče napříč všemi hrami.</summary>
public class PlayerStats
{
    public string Player { get; set; } = "";
    public int Games { get; set; }
    public int Wins { get; set; }
    public int Losses => Games - Wins;
    public double WinRate => Games > 0 ? (double)Wins / Games * 100 : 0;
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public int TotalScore { get; set; }
    public int HighScore { get; set; }
}
