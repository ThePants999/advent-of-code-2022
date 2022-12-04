namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day4 : Day
{
    List<ElfPair> pairs = new List<ElfPair>();

    public Day4(AOCEnvironment env) : base("2022", 4, env) { }

    protected override string ExecutePart1()
    {
        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            pairs.Add(new ElfPair(line));
        }

        return pairs.Where<ElfPair>(pair => pair.HasFullOverlap()).Count().ToString();
    }

    protected override string ExecutePart2()
    {
        return pairs.Where<ElfPair>(pair => pair.HasPartialOverlap()).Count().ToString();
    }
}

internal class ElfPair
{
    private int _firstElfStart;
    private int _firstElfEnd;
    private int _secondElfStart;
    private int _secondElfEnd;

    internal ElfPair(string input)
    {
        string[] elves = input.Split(',');
        string[] firstElf = elves[0].Split('-');
        _firstElfStart = int.Parse(firstElf[0]);
        _firstElfEnd = int.Parse(firstElf[1]);
        string[] secondElf = elves[1].Split('-');
        _secondElfStart = int.Parse(secondElf[0]);
        _secondElfEnd = int.Parse(secondElf[1]);
    }

    internal bool HasFullOverlap()
    {
        return (_secondElfStart >= _firstElfStart && _secondElfEnd <= _firstElfEnd) || (_firstElfStart >= _secondElfStart && _firstElfEnd <= _secondElfEnd);
    }

    internal bool HasPartialOverlap()
    {
        return (_secondElfStart >= _firstElfStart && _secondElfStart <= _firstElfEnd) || (_firstElfStart >= _secondElfStart && _firstElfStart <= _secondElfEnd);
    }
}