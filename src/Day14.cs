namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day14 : Day
{
    List<bool[]> _blocked = new List<bool[]>();
    int part1Answer = 0;

    public Day14(AOCEnvironment env) : base("2022", 14, env) { }

    protected override string ExecutePart1()
    {
        var walls = ParseInput();

        int depth = walls.Select<Position, int>(wall => wall.Row).Max();
        int width = walls.Select<Position, int>(wall => wall.Col).Max() + depth;

        for (int row = 0; row <= depth; row++)
        {
            _blocked.Add(new bool[width]);
            for (int col = 0; col < width; col++)
            {
                _blocked[row][col] = false;
            }
        }

        foreach (Position wall in walls)
        {
            _blocked[wall.Row][wall.Col] = true;
        }

        // Walls are ready, let's do some sand!
        while (NewSandDropBlocked(_blocked, true))
        {
            part1Answer++;
        }

        return part1Answer.ToString();
    }

    protected override string ExecutePart2()
    {
        bool[] newRow = new bool[_blocked[0].Length];
        for (int col = 0; col < newRow.Length; col++)
        {
            newRow[col] = false;
        }
        _blocked.Add(newRow);

        int sandUnits = part1Answer;
        while (!_blocked[0][500] && NewSandDropBlocked(_blocked, false))
        {
            sandUnits++;
        }

        return sandUnits.ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "24";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "93";
    }

    private bool NewSandDropBlocked(List<bool[]> blocked, bool infinite)
    {
        int row = 0;
        int col = 500;
        while (true)
        {
            if (row == blocked.Count - 1)
            {
                // We're at the bottom.
                if (infinite)
                {
                    return false;
                } else {
                    blocked[row][col] = true;
                    return true;
                }
            } else if (!blocked[row + 1][col]) {
                row++;
            } else if (!blocked[row + 1][col - 1]) {
                row++;
                col--;
            } else if (!blocked[row + 1][col + 1]) {
                row++;
                col++;
            } else {
                // Blocked!
                blocked[row][col] = true;
                return true;
            }
        }
    }

    private List<Position> ParseInput()
    {
        var walls = new List<Position>();

        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            int[][] corners = line.Split(" -> ").Select<string, int[]>(pair => pair.Split(',').Select<string, int>(coord => int.Parse(coord)).ToArray()).ToArray();
            Position previous = new Position(corners[0][1], corners[0][0]);
            walls.Add(previous);
            for (int i = 1; i < corners.Length; i++)
            {
                Position next = new Position(corners[i][1], corners[i][0]);
                walls.Add(next);
                if (next.Row == previous.Row)
                {
                    // Row matches, line is horizontal
                    int lowerCol = Math.Min(next.Col, previous.Col);
                    int upperCol = Math.Max(next.Col, previous.Col);
                    for (int col = lowerCol + 1; col < upperCol; col++)
                    {
                        walls.Add(new Position(next.Row, col));
                    }
                } else {
                    // Line is vertical
                    int lowerRow = Math.Min(next.Row, previous.Row);
                    int upperRow = Math.Max(next.Row, previous.Row);
                    for (int row = lowerRow + 1; row < upperRow; row++)
                    {
                        walls.Add(new Position(row, next.Col));
                    }
                }
                previous = next;
            }
        }

        return walls;
    }
}
