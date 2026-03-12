namespace Sibenice;

using System.Text.Json;

/// <summary>
/// Správce slovníku - načítá, ukládá a spravuje slova pro jednotlivé obtížnosti.
/// Podporuje výchozí slova z default.json i vlastní slova od hráče.
/// </summary>
public class WordManager
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    // Slovník slov seřazený podle obtížnosti
    private Dictionary<Difficulty, List<string>> _words = new();

    /// <summary>Vrátí true, pokud je načteno alespoň jedno slovo.</summary>
    public bool HasWords => _words.Values.Any(w => w.Count > 0);

    /// <summary>Načte slova ze souboru JSON (klíče: lehka, stredni, tezka).</summary>
    public void LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Soubor se slovy nenalezen: {path}");

        var json = File.ReadAllText(path);
        var raw = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
            ?? throw new InvalidOperationException("Chyba při načítání slov ze souboru.");

        _words = new Dictionary<Difficulty, List<string>>
        {
            [Difficulty.Lehka] = raw.GetValueOrDefault("lehka", new()),
            [Difficulty.Stredni] = raw.GetValueOrDefault("stredni", new()),
            [Difficulty.Tezka] = raw.GetValueOrDefault("tezka", new())
        };
    }

    /// <summary>Nastaví vlastní slova pro danou obtížnost (ořízne mezery, převede na malá písmena).</summary>
    public void SetCustomWords(Difficulty difficulty, List<string> words)
    {
        _words[difficulty] = words
            .Select(w => w.Trim().ToLower())
            .Where(w => w.Length > 0)
            .ToList();
    }

    /// <summary>Uloží aktuální slovník do souboru JSON.</summary>
    public void SaveToFile(string path)
    {
        var raw = new Dictionary<string, List<string>>
        {
            ["lehka"] = _words.GetValueOrDefault(Difficulty.Lehka, new()),
            ["stredni"] = _words.GetValueOrDefault(Difficulty.Stredni, new()),
            ["tezka"] = _words.GetValueOrDefault(Difficulty.Tezka, new())
        };

        File.WriteAllText(path, JsonSerializer.Serialize(raw, JsonOptions));
    }

    /// <summary>Vrátí seznam slov pro danou obtížnost.</summary>
    public List<string> GetWords(Difficulty difficulty)
        => _words.GetValueOrDefault(difficulty, new());

    /// <summary>Vylosuje náhodné slovo z dané obtížnosti.</summary>
    public string GetRandomWord(Difficulty difficulty)
    {
        var words = GetWords(difficulty);
        if (words.Count == 0)
            throw new InvalidOperationException($"Žádná slova pro obtížnost {difficulty}.");
        return words[Random.Shared.Next(words.Count)];
    }

    /// <summary>Vrátí počet slov pro danou obtížnost.</summary>
    public int WordCount(Difficulty difficulty) => GetWords(difficulty).Count;
}
