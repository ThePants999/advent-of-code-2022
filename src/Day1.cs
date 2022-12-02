namespace Patersoft.AOC.AOC22;

using Microsoft.Extensions.Logging;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;

using System.Collections.Generic;

public class Day1 : Day
{
    List<int> elves = new List<int>();

    public Day1(AOCEnvironment env) : base("2022", 1, env) { }

    protected override string ExecutePart1()
    {
        int calories = 0;

        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            if (line.Length == 0)
            {
                // Blank line - new Elf time
                elves.Add(calories);
                calories = 0;
            } else {
                try
                {
                    int newCals = Int32.Parse(line);
                    calories += newCals;
                }
                catch (FormatException e)
                {
                    Env.Logger.LogError($"Invalid input: {line}", e);
                    throw e;
                }
            }
        }
        elves.Add(calories);

        elves.Sort();

        return elves.Last<int>().ToString();
    }

    protected override string ExecutePart2()
    {
        return (elves[elves.Count - 1] + elves[elves.Count - 2] + elves[elves.Count - 3]).ToString();
    }
}