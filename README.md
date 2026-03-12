# Sibenice

Konzolova hra sibenice (Hangman) v C# — semestralni projekt na vysoke skole.

## O projektu

Klasicka hra sibenice, kde hadac po pismenech odhaluje skryte slovo. Hra nabizi tri urovne obtiznosti, system bodovani, zebricek nejlepsich hracu a sledovani statistik.

### Funkce

- **3 urovne obtiznosti** — lehka, stredni, tezka (s nasobitelem skore x1/x2/x3)
- **Bodovaci system** — skore zalozene na delce slova, zbylych zivotech a rychlosti
- **Napoveda** — odhaleni nahodneho pismene za cenu jednoho zivotu (zadej `?`)
- **Zebricek** — persistentni top 10 nejlepsich skore (ulozeno v `scores.json`)
- **Statistiky hrace** — vyher, proher, uspesnost, serie, high score
- **Vlastni slova** — moznost zadat vlastni slovnik pro kazdou obtiznost
- **Progresivni ASCII art** — 8 fazi sibenice od prazdne az po obeseneho

## Spusteni

Vyzaduje [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet run
```

## Struktura projektu

| Soubor | Popis |
|---|---|
| `Program.cs` | Vstupni bod, hlavni menu, orchestrace hernich kol |
| `HangmanGame.cs` | Herni logika — stav hry, hadani, skore, napoveda |
| `Renderer.cs` | Veskeré vykreslovani do konzole |
| `HangmanArt.cs` | ASCII art sibenice (8 progresivnich fazi) |
| `WordManager.cs` | Nacitani a sprava slovniku slov |
| `ScoreBoard.cs` | Zebricek a statistiky hracu |
| `Models.cs` | Datove modely a enumy |
| `default.json` | Vychozi slovnik — 20 slov na kazdou obtiznost |
