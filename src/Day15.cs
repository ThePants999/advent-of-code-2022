namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day15 : Day
{
    List<Sensor> _sensors = new List<Sensor>();

    public Day15(AOCEnvironment env) : base("2022", 15, env) { }

    protected override string ExecutePart1()
    {
        var beacons = new HashSet<Position>();

        // Hackery to tell whether we're running with real or sample input
        bool sample = (Input.Length < 800);
        int targetRow = sample ? 10 : 2000000;

        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            string[] chunks = line.Split(' ');
            int sensorX = int.Parse(chunks[2].Substring(2, chunks[2].Length - 3));
            int sensorY = int.Parse(chunks[3].Substring(2, chunks[3].Length - 3));
            int beaconX = int.Parse(chunks[8].Substring(2, chunks[8].Length - 3));
            int beaconY = int.Parse(chunks[9].Substring(2));
            Position sensorPos = new Position(sensorY, sensorX);
            Position beacon = new Position(beaconY, beaconX);
            beacons.Add(beacon);
            Sensor sensor = new Sensor(sensorPos, beacon);
            _sensors.Add(sensor);
        }

        Line targetLine = new Line();
        foreach (Sensor sensor in _sensors)
        {
            Range? newRange = sensor.Coverage(targetRow, Restrictions.None);
            if (newRange != null)
            {
                targetLine.AddRange(newRange);
            }
        }

        int nopeCount = targetLine.Ranges[0].Size - beacons.Where(beacon => beacon.Row == targetRow).Count();
        return nopeCount.ToString();
    }

    protected override string ExecutePart2()
    {
        // Hackery to tell whether we're running with real or sample input
        bool sample = (Input.Length < 800);
        int limit = sample ? 20 : 4000000;
        Restrictions restrictions = sample ? Restrictions.Sample : Restrictions.Full;

        long answer = 0;
        for (int row = 0; row <= limit; row++)
        {
            Line targetLine = new Line();
            foreach (Sensor sensor in _sensors)
            {
                Range? newRange = sensor.Coverage(row, restrictions);
                if (newRange != null)
                {
                    targetLine.AddRange(newRange);
                }
            }
            if (targetLine.Ranges.Count == 2)
            {
                int col = Math.Max(targetLine.Ranges[0].Min, targetLine.Ranges[1].Min) - 1;
                answer = (long)row + (long)((long)col * (long)4000000);
                break;
            }
        }

        return answer.ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "26";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "56000011";
    }
}

internal class Line
{
    internal List<Range> Ranges = new List<Range>();

    internal Line() {}

    internal void AddRange(Range range)
    {
        Range? mergedRange = null;
        foreach (Range existing in Ranges)
        {
            bool merged = existing.Merge(range);
            if (merged)
            {
                mergedRange = existing;
                break;
            }
        }

        if (mergedRange != null)
        {
            bool merged = false;
            do
            {
                merged = false;
                foreach (Range other in Ranges)
                {
                    if (mergedRange != other)
                    {
                        if (mergedRange.Merge(other))
                        {
                            merged = true;
                            Ranges.Remove(other);
                            break;
                        }
                    }
                }
            } while (merged);
        } else {
            Ranges.Add(range);
        }
    }
}

internal enum Restrictions
{
    None,
    Sample,
    Full
}

internal class Range
{
    private int _min;
    private int _max;
    private int _minLimit;
    private int _maxLimit;
    internal int Min { get { return _min; } }
    internal int Max { get { return _max; } }
    internal int Size { get { return _max - _min + 1; } }

    internal Range (int min, int max, Restrictions restrictions)
    {
        _min = min;
        _max = max;
        switch (restrictions)
        {
            case Restrictions.None:
                _minLimit = int.MinValue;
                _maxLimit = int.MaxValue;
                break;

            case Restrictions.Sample:
                _minLimit = 0;
                _maxLimit = 20;
                break;

            case Restrictions.Full:
                _minLimit = 0;
                _maxLimit = 4000000;
                break;
        }
    }

    internal bool Merge(Range other)
    {
        if ((_min <= other._min && _max >= other._min) || (other._min <= _min && other._max >= _min))
        {
            _min = Math.Max(_minLimit, Math.Min(_min, other._min));
            _max = Math.Min(_maxLimit, Math.Max(_max, other._max));
            return true;
        } else {
            return false;
        }
    }
}

internal class Sensor
{
    internal Position Position { get; init; }
    internal Position Beacon { get; init; }
    internal int Range { get; init; }

    internal Sensor(Position pos, Position beacon)
    {
        Position = pos;
        Beacon = beacon;
        Range = Math.Abs(pos.Row - beacon.Row) + Math.Abs(pos.Col - beacon.Col);
    }

    internal Range? Coverage(int row, Restrictions restrictions)
    {
        int distanceToRow = Math.Abs(Position.Row - row);
        int remainingDistance = Range - distanceToRow;
        if (remainingDistance >= 0)
        {
            return new Range(Position.Col - remainingDistance, Position.Col + remainingDistance, restrictions);
        } else {
            return null;
        }
    }
}