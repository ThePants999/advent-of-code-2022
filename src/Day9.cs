namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
//using System.Linq;

public class Day9 : Day
{
    Rope _rope = new Rope();

    public Day9(AOCEnvironment env) : base("2022", 9, env) { }

    protected override string ExecutePart1()
    {
        _rope.ApplyInstructions(Input);
        return _rope.Knot1VisitedCount.ToString();
    }

    protected override string ExecutePart2()
    {
        return _rope.Knot9VisitedCount.ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "13";
    }

    protected override string? GetExamplePart2Answer()
    {
        return null;
    }
}

internal enum Directions
{
    Up,
    Down,
    Left,
    Right
}

internal class MoveablePosition : Position
{
    internal MoveablePosition(int row, int col) : base(row, col) {}

    internal MoveablePosition() : base(0, 0) {}

    internal void Move(Directions dir)
    {
        switch (dir)
        {
            case Directions.Up:
                _row -= 1;
                break;

            case Directions.Down:
                _row += 1;
                break;

            case Directions.Left:
                _col -= 1;
                break;

            case Directions.Right:
                _col += 1;
                break;
        }
    }

    internal void MoveTowards(MoveablePosition other)
    {
        if (Math.Abs(_row - other._row) > 1 || Math.Abs(_col - other._col) > 1)
        {
            // Not adjacent, move
            if (_row < other._row)
            {
                _row += 1;
            } else if (_row > other._row)
            {
                _row -= 1;
            }
            if (_col < other._col)
            {
                _col += 1;
            } else if (_col > other._col)
            {
                _col -= 1;
            }
        }
    }

    new internal MoveablePosition Clone()
    {
        return new MoveablePosition(_row, _col);
    }
}

internal class Rope
{
    private MoveablePosition[] _knots = new MoveablePosition[10];
    private HashSet<MoveablePosition> _knot1Visited = new HashSet<MoveablePosition>();
    internal int Knot1VisitedCount { get { return _knot1Visited.Count; } }
    private HashSet<MoveablePosition> _knot9Visited = new HashSet<MoveablePosition>();
    internal int Knot9VisitedCount { get { return _knot9Visited.Count; } }

    internal Rope()
    {
        for (int i = 0; i < _knots.Length; i++)
        {
            _knots[i] = new MoveablePosition();
        }
        _knot1Visited.Add(new MoveablePosition());
        _knot9Visited.Add(new MoveablePosition());
    }

    internal void ApplyInstructions(string instructions)
    {
        foreach (string line in new LineReader(() => new StringReader(instructions)))
        {
            string[] chunks = line.Split(' ');
            Directions dir = chunks[0][0] switch
            {
                'U' => Directions.Up,
                'D' => Directions.Down,
                'L' => Directions.Left,
                'R' => Directions.Right,
                _ => throw new InputException($"Invalid direction {chunks[0]}"),
            };
            int count = int.Parse(chunks[1]);
            for (int i = 0; i < count; i++)
            {
                _knots[0].Move(dir);
                for (int knot = 1; knot < _knots.Length; knot++)
                {
                    _knots[knot].MoveTowards(_knots[knot - 1]);
                }
                _knot1Visited.Add(_knots[1].Clone());
                _knot9Visited.Add(_knots[9].Clone());
            }
        }
    }
}