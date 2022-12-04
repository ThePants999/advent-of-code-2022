namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day3 : Day
{
    private List<Rucksack> _rucksacks = new List<Rucksack>();
    public Day3(AOCEnvironment env) : base("2022", 3, env) { }

    protected override string ExecutePart1()
    {
        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            _rucksacks.Add(new Rucksack(line));
        }

        return _rucksacks
            .Select<Rucksack,int>(rs => SymbolToPriority(rs.FindDuplicatedSymbol()))
            .Sum()
            .ToString();
    }

    protected override string ExecutePart2()
    {
        return _rucksacks
            .Chunk<Rucksack>(3)
            .Select<Rucksack[],char>(group => group[0].Contents.Intersect(group[1].Contents).Intersect(group[2].Contents).First())
            .Select<char,int>(symbol => SymbolToPriority(symbol))
            .Sum()
            .ToString();
    }

    private static int SymbolToPriority(char symbol)
    {
        if (symbol <= 'Z')
        {
            return symbol - 'A' + 27;
        } else {
            return symbol - 'a' + 1;
        }
    }
}

internal class Rucksack
{
    internal HashSet<char> Contents { get; init; }
    private HashSet<char> _compartmentOne = new HashSet<char>();
    private HashSet<char> _compartmentTwo = new HashSet<char>();

    internal Rucksack(string contents)
    {
        var contentsSet = new HashSet<char>();
        contentsSet.UnionWith(contents.ToCharArray());
        Contents = contentsSet;

        int halfLength = contents.Length / 2;
        _compartmentOne.UnionWith(contents.Substring(0, halfLength).ToCharArray());
        _compartmentTwo.UnionWith(contents.Substring(halfLength).ToCharArray());
    }

    internal char FindDuplicatedSymbol()
    {
        return _compartmentOne.Intersect<char>(_compartmentTwo).First<char>();
    }
}