namespace Patersoft.AOC.AOC22;

using Microsoft.Extensions.Logging;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;

using System.Collections.Generic;

public class Day2 : Day
{
    int scorePart1 = 0;
    int scorePart2 = 0;

    public Day2(AOCEnvironment env) : base("2022", 2, env) { }

    protected override string ExecutePart1()
    {
        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            Round round = new Round(line);
            scorePart1 += round.ScorePart1();
            scorePart2 += round.ScorePart2();
        }

        return scorePart1.ToString();
    }

    protected override string ExecutePart2()
    {
        return scorePart2.ToString();
    }
}

enum RPS
{
    Rock = 1,
    Paper = 2,
    Scissors = 3
}

enum RPSResult
{
    Win = 6,
    Draw = 3,
    Loss = 0
}

internal class Round
{
    private RPS opponent;
    private RPS youPart1;
    private RPSResult resultPart1;
    private int youPart2;
    private RPSResult resultPart2;

    internal Round(string input) {
        char[] chars = input.ToCharArray();
        opponent = chars[0] switch
        {
            'A' => RPS.Rock,
            'B' => RPS.Paper,
            'C' => RPS.Scissors,
            _ => throw new Exception($"Invalid input {input}")
        };

        youPart1 = chars[2] switch
        {
            'X' => RPS.Rock,
            'Y' => RPS.Paper,
            'Z' => RPS.Scissors,
            _ => throw new Exception($"Invalid input {input}")
        };

        resultPart2 = chars[2] switch
        {
            'X' => RPSResult.Loss,
            'Y' => RPSResult.Draw,
            'Z' => RPSResult.Win,
            _ => throw new Exception($"Invalid input {input}")
        };

        int resultPart1Int = (int)youPart1 - (int)opponent;
        resultPart1 = resultPart1Int switch
        {
            0 => RPSResult.Draw,
            1 => RPSResult.Win,
            -2 => RPSResult.Win,
            _ => RPSResult.Loss
        };

        youPart2 = (int)opponent + resultPart2 switch
        {
            RPSResult.Draw => 0,
            RPSResult.Win => 1,
            RPSResult.Loss => -1,
            _ => throw new Exception("Unreachable")
        };
        if (youPart2 == 0) { youPart2 = 3; }
        if (youPart2 == 4) { youPart2 = 1; }
    }

    internal int ScorePart1()
    {
        return (int)youPart1 + (int)resultPart1;
    }

    internal int ScorePart2()
    {
        return youPart2 + (int)resultPart2;
    }
}