namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using System.Collections.Generic;

public class Day6 : Day
{
    public Day6(AOCEnvironment env) : base("2022", 6, env) { }

    protected override string ExecutePart1()
    {
        for (int index = 3; index < Input.Length; index++)
        {
            if (Input[index - 3] != Input[index - 2] &&
                Input[index - 3] != Input[index - 1] &&
                Input[index - 3] != Input[index] &&
                Input[index - 2] != Input[index - 1] &&
                Input[index - 2] != Input[index] &&
                Input[index - 1] != Input[index])
            {
                return (index + 1).ToString();
            }
        }
        return "error";
    }

    protected override string ExecutePart2()
    {
        for (int index = 13; index < Input.Length; index++)
        {
            HashSet<char> set = new HashSet<char>(Input.Substring(index - 13, 14));
            if (set.Count == 14)
            {
                return (index + 1).ToString();
            }
        }
        return "error";
    }

    protected override string? GetExampleInput()
    {
        return "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "11";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "26";
    }
}