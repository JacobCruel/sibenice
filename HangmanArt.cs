namespace Sibenice;

/// <summary>
/// ASCII art šibenice - 8 progresivních fází od prázdné šibenice po smrt.
/// </summary>
public static class HangmanArt
{
    /// <summary>Maximální počet fází (= maximální počet špatných pokusů před prohrou).</summary>
    public const int MaxStages = 7;

    // Každá fáze přidává jednu část těla: hlava, tělo, levá ruka, pravá ruka, levá noha, pravá noha, mrtvý
    private static readonly string[][] Stages =
    {
        // 0 - prázdná šibenice
        new[]
        {
            "  +---+  ",
            "  |   |  ",
            "  |      ",
            "  |      ",
            "  |      ",
            "  |      ",
            "=====    "
        },
        // 1 - hlava
        new[]
        {
            "  +---+  ",
            "  |   |  ",
            "  |   O  ",
            "  |      ",
            "  |      ",
            "  |      ",
            "=====    "
        },
        // 2 - tělo
        new[]
        {
            "  +---+  ",
            "  |   |  ",
            "  |   O  ",
            "  |   |  ",
            "  |      ",
            "  |      ",
            "=====    "
        },
        // 3 - levá ruka
        new[]
        {
            "  +---+  ",
            "  |   |  ",
            "  |   O  ",
            "  |  /|  ",
            "  |      ",
            "  |      ",
            "=====    "
        },
        // 4 - obě ruce
        new[]
        {
            "  +---+  ",
            "  |   |  ",
            "  |   O  ",
            @"  |  /|\ ",
            "  |      ",
            "  |      ",
            "=====    "
        },
        // 5 - levá noha
        new[]
        {
            "  +---+  ",
            "  |   |  ",
            "  |   O  ",
            @"  |  /|\ ",
            "  |  /   ",
            "  |      ",
            "=====    "
        },
        // 6 - obě nohy (kompletní postava)
        new[]
        {
            "  +---+  ",
            "  |   |  ",
            "  |   O  ",
            @"  |  /|\ ",
            @"  |  / \ ",
            "  |      ",
            "=====    "
        },
        // 7 - mrtvý (X místo očí)
        new[]
        {
            "  +---+  ",
            "  |   |  ",
            "  |   X  ",
            @"  |  /|\ ",
            @"  |  / \ ",
            "  |      ",
            "=====    "
        }
    };

    /// <summary>Vrátí ASCII art pro daný počet špatných pokusů.</summary>
    public static string[] GetStage(int wrongGuesses)
    {
        int index = Math.Clamp(wrongGuesses, 0, Stages.Length - 1);
        return Stages[index];
    }

    /// <summary>Art pro vítězství - panáček unikl ze šibenice a slaví.</summary>
    public static string[] GetWinArt()
    {
        return new[]
        {
            "  +---+  ",
            "  |      ",
            "  |      ",
            @"  | \o/  ",
            "  |  |   ",
            @"  | / \  ",
            "=====    "
        };
    }
}
