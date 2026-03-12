namespace Sibenice;

using System.Text.Json;

/// <summary>
/// Žebříček a statistiky hráčů.
/// Ukládá záznamy her do scores.json a počítá statistiky (série, úspěšnost, high score).
/// </summary>
public class ScoreBoard
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private List<GameRecord> _records = new();

    public ScoreBoard(string filePath = "scores.json")
    {
        _filePath = filePath;
        Load();
    }

    /// <summary>Přidá nový záznam hry a uloží žebříček.</summary>
    public void AddRecord(GameRecord record)
    {
        _records.Add(record);
        Save();
    }

    /// <summary>Vrátí top N nejlepších skóre (jen výhry), seřazených sestupně.</summary>
    public List<GameRecord> TopScores(int count = 10)
    {
        return _records
            .Where(r => r.Won)
            .OrderByDescending(r => r.Score)
            .Take(count)
            .ToList();
    }

    /// <summary>Vypočítá souhrnné statistiky pro konkrétního hráče.</summary>
    public PlayerStats StatsFor(string player)
    {
        var records = _records.Where(r => r.Player == player).OrderBy(r => r.Date).ToList();

        // Výpočet aktuální a nejlepší série výher
        int currentStreak = 0, bestStreak = 0;
        foreach (var r in records)
        {
            if (r.Won)
            {
                currentStreak++;
                bestStreak = Math.Max(bestStreak, currentStreak);
            }
            else
            {
                currentStreak = 0;
            }
        }

        return new PlayerStats
        {
            Player = player,
            Games = records.Count,
            Wins = records.Count(r => r.Won),
            TotalScore = records.Sum(r => r.Score),
            HighScore = records.Any() ? records.Max(r => r.Score) : 0,
            CurrentStreak = currentStreak,
            BestStreak = bestStreak
        };
    }

    /// <summary>Načte záznamy ze souboru (pokud existuje).</summary>
    private void Load()
    {
        if (!File.Exists(_filePath)) return;
        try
        {
            var json = File.ReadAllText(_filePath);
            _records = JsonSerializer.Deserialize<List<GameRecord>>(json) ?? new();
        }
        catch { _records = new(); }
    }

    /// <summary>Uloží záznamy do souboru.</summary>
    private void Save()
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_records, JsonOptions));
    }
}
