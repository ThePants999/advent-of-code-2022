namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day12 : Day
{
    private Location[][]? _maze;
    private Location? _top;

    public Day12(AOCEnvironment env) : base("2022", 12, env) { }

    protected override string ExecutePart1()
    {
        Location start = CreateMaze();

        BFS(start, loc => loc == _top!, (to, from) => to.Height - from.Height <= 1);

        return _top!.Distance.ToString();
    }

    protected override string ExecutePart2()
    {
        ResetMaze();

        Location? bottom = BFS(_top!, loc => loc.Height == 0, (to, from) => from.Height - to.Height <= 1);

        return bottom!.Distance.ToString();
    }

    private Location CreateMaze()
    {
        string fixedInput = Input.Replace("\r", null).Trim();
        string[] lines = fixedInput.Split("\n");
        int mazeHeight = lines.Length;
        int mazeWidth = lines[0].Length;

        Location? start = null;

        _maze = new Location[mazeHeight][];
        for (int row = 0; row < mazeHeight; row++)
        {
            _maze[row] = new Location[mazeWidth];
            char[] chars = lines[row].ToCharArray();
            for (int col = 0; col < mazeWidth; col++)
            {
                if (chars[col] == 'S')
                {
                    Location loc = new Location(row, col, 0);
                    _maze[row][col] = loc;
                    start = loc;
                } else if (chars[col] == 'E') {
                    Location loc = new Location(row, col, 25);
                    _maze[row][col] = loc;
                    _top = loc;
                } else {
                    _maze[row][col] = new Location(row, col, chars[col] - 'a');
                }
            }
        }

        return start!;
    }

    private void ResetMaze()
    {
        foreach (Location[] row in _maze!)
        {
            foreach (Location loc in row)
            {
                loc.Reset();
            }
        }
    }

    private Location? BFS(Location start, Predicate<Location> locationIsEnd, Func<Location, Location, bool> canReachFrom)
    {
        Queue<Location> q = new Queue<Location>();

        start.Distance = 0;
        start.Visited = true;
        q.Enqueue(start);

        while (q.Count > 0)
        {
            Location current = q.Dequeue();

            List<Location> candidates = new List<Location>();
            if (current.Row > 0)
            {
                candidates.Add(_maze![current.Row - 1][current.Col]);
            }
            if (current.Row < _maze!.Length - 1)
            {
                candidates.Add(_maze[current.Row + 1][current.Col]);
            }
            if (current.Col > 0)
            {
                candidates.Add(_maze[current.Row][current.Col - 1]);
            }
            if (current.Col < _maze[current.Row].Length - 1)
            {
                candidates.Add(_maze[current.Row][current.Col + 1]);
            }

            foreach (Location candidate in candidates)
            {
                if ((!candidate.Visited) && canReachFrom(candidate, current))
                {
                    candidate.Visited = true;
                    candidate.Distance = current.Distance + 1;
                    if (locationIsEnd(candidate))
                    {
                        return candidate;
                    }
                    q.Enqueue(candidate);
                }
            }
        }

        return null;
    }

    protected override string? GetExampleInput()
    {
        return @"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "31";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "29";
    }
}

internal class Location
{
    internal int Row { get; init; }
    internal int Col { get; init; }
    internal bool Visited { get; set; }
    internal int Height { get; init; }
    internal int Distance { get; set; }

    internal Location(int row, int col, int height)
    {
        this.Row = row;
        this.Col = col;
        this.Height = height;
        Reset();
    }

    internal void Reset()
    {
        Visited = false;
        Distance = int.MaxValue;
    }
}